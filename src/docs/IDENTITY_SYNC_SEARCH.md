This document serves as the definitive architectural blueprint for the GaDeMa API. It defines how we handle **Identity**, **Domain Data**, and **Discovery** to ensure that as the system grows, it remains consistent, type-safe, and easy to search.

# GaDeMa Architecture: Identity Sync & Search Orchestration

## 1. The Core Hierarchy: Root vs. Content
We distinguish between two types of "Anchors" in our world. This distinction dictates where data is stored and how it is searched.

| Feature | **Root Anchors (The Universe)** | **Content Anchors (The Data)** |
| :--- | :--- | :--- |
| **Examples** | `Project` | `Character`, `Scene`, `LoreEntry` |
| **Identity Layer** | `ProjectMetaInfo` | `MetaInfo` |
| **Search Scope** | Global/Universe Discovery | Scoped to a Project |
| **Lifecycle** | Defines the workspace | Lives within a workspace |

---

## 2. The Identity Sync Pattern (The "Write" Side)
To prevent "Identity Bloat" and ensure data integrity, we separate an entity's **Identity** (Title, Slug, Status) from its **Domain Data** (Genre, Age, Role).

### The Mechanism: `ApplyIdentitySyncAsync`
Instead of manual updates, all services use a standardized strategy-based approach. This ensures that when a user updates an identity, both the properties and the associated tags are synchronized atomically.

#### The Interface: `IIdentitySyncStrategy`
Every anchor type must have a strategy that defines how its specific identity is updated in the database.
```csharp
public interface IIdentitySyncStrategy
{
    // Handles Title, Slug, Status, Visibility, ViewMode, and Tag synchronization
    Task SyncAsync(Guid identityId, MetaInfoUpdateData updateData);
}
```

#### The Workflow (The "Sync" Logic)
1.  **Client Sends**: A `UpdateDto` containing both Domain data and a `MetaInfoUpdateDto` (which includes the full list of current `TagIds`).
2.  **Service Orchestrates**: The domain service updates its own properties, then calls `ApplyIdentitySyncAsync`.
3.  **Strategy Executes**: The strategy performs a **Delta Calculation** (calculates which tags to add and which to remove) and executes the changes within a transaction.

---

## 3. The Search Orchestrator Pattern (The "Read" Side)
To prevent "Search Bloat" and fragmented endpoints, we use an orchestration layer that aggregates results from multiple domain providers into a single, unified stream.

### A. The Contract: `ISearchableProvider`
Every domain service (Project, Character, etc.) must implement this interface to be "discoverable."
```csharp
public interface ISearchableProvider
{
    // Returns the raw matches for a query within this provider's scope
    Task<IEnumerable<SearchHitDto>> GetMatchesAsync(string query, Guid? projectId);
}
```

### B. The Universal Response: `SearchHitDto`
All search results must be mapped to this shape. This allows the client to handle any result type (Project or Character) using a single UI component.
```csharp
public class SearchHitDto 
{
    public Guid ResourceId { get; set; }      // The actual entity ID
    public string DisplayName { get; set; }   // Title/Name for display
    public string Slug { get; set; }          // URL identifier
    public string ResourceType { get { ... } } // "Project", "Character", etc.
    public Guid? ScopeId { get; set; }        // Null for Root, ProjectId for Content
    public string ResourceLink { get { ... } } // API endpoint to fetch details
}
```

### C. The Engine: `SearchOrchestrator`
The Orchestrator is the single entry point for all search queries. It follows this pipeline:
1.  **Gather**: Calls all registered `ISearchableProvider` implementations in parallel.
2.  **Merge**: Combably aggregates all hits into one master list.
3.  **Paginate**: Performs the final `Skip` and `Take` math on the merged collection.

---

## 4. Summary of Responsabilities

| Component | Responsibility | Knowledge |
| :--- | :--- | :--- |
| **Domain Service** | Manages Domain Data + Orchestrates Identity Sync. | Knows its own properties & DB. |
| **Identity Strategy** | Handles the "Delta" logic for Title, Slug, and Tags. | Knows how to write to identity tables. |
| **Search Provider** | Provides raw matches from a specific domain. | Knows how to query its specific table. |
| **Search Orchestrator**| Merges results and applies global pagination. | Knows nothing of domains; only knows `ISearchableProvider`. |

---

## 5. Developer Checklist for New Entities
When adding a new entity (e.g., `WorldLocation`):
1.  [ ] Create the **Model** (Domain Data).
2.  [ ] Create/Update the **MetaInfo** (Identity Data).
3.  [ ] Implement `ISearchableProvider` in your service.
4.  [ ] Implement `IIdentitySyncStrategy` for its identity sync.
5.  [ ] Add it to the `SearchOrchestrator`'s provider list.
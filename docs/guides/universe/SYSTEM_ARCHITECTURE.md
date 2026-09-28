# GaDeMa System Architecture & Patterns

This document defines the core architectural patterns for the GaDeMa API. These patterns are designed to ensure scalability, maintainable search/discovery, and a clear distinction between different levels of data hierarchy.

---

## 1. The Hierarchy Distinction: Root vs. Content

A fundamental principle of this system is the semantic distinction between "The Environment" and "The Data within it."

### A. Root Anchors (The Universe)
*   **Examples**: `Project`, `Community` (future).
*   **Role**: These are the top-level containers/workspaces. They define the scope for all other data.
*   **Identity**: They have their own unique identity properties (Title, Slug, Status, etc.) that are **not** part of a shared content pool.
*   **Discovery**: Searched at the "Universe" level. A user discovers a Project to begin working within it.

### B. Content Anchors & Components (The Data)
*   **Examples**: `Character`, `Scene`, `LoreEntry` (Anchors); `DialogueNode`, `Comment`, `ReviewStatus` (Components).
*   **Role**: These are the entities that live *inside* a Root Anchor. 
*   **Identity**: They use the **MetaInfo Pattern**. They are "anonymous" until attached to a Project via a `ProjectId`.
*   **Discovery**: Searched within the scope of a specific project.

---

## 2. The Search Engine Architecture

To prevent API bloat and ensure a unified experience for consumers, we use an **Orchestrator-based Search Pattern**.

### A. The Contract: `SearchHitDto`
All search operations—regardless of whether they are searching for Projects or Characters—must return this standardized shape to the consumer.

```csharp
public class SearchHitDto 
{
    public Guid ResourceId { get; set; }      // The actual ID of the resource
    public string DisplayName { get; set; }   // For display in lists (Title/Name)
    public string Slug { get; set; }          // URL-friendly identifier
    public string ResourceType { get; set; }  // "Project", "Character", etc.
    public Guid? ScopeId { get; set; }        // Null for Projects, ProjectId for Content
    public string ResourceLink { get; set; }  // The API endpoint to fetch details
}
```

### B. The Implementation: Orchestrator Pattern
We avoid "Service Bloat" by separating **Data Retrieval** from **Search Logic**.

1.  **Data Providers (The Services)**: 
    *   Services like `CharacterService` or `ProjectService` are responsible *only* for finding matches in the database.
    *   They do **not** handle pagination or complex search merging.
    *   They return a raw collection of results (or a simple count).

2.  **The Orchestrator (The Search Engine)**: 
    *   A single `SearchOrchestrator` service manages the complexity of merging multiple streams.
    *   It calls the various Data Providers, gathers their results, and handles the "Global" logic.

---

## 3. The Pagination Pattern: Centralized Paging

To prevent redundant paging logic across every controller/service, we use **Orchestrator-Level Pagination**.

### The Workflow:
1.  **Providers**: `GetMatchesAsync(query)` returns all matching items from the database (no `Skip` or `Take`).
2.  **Orchestrator**: 
    *   Calls multiple providers to gather a master list of results.
    *   Performs the pagination math (`Skip` and `Take`) in **one single place**.
    *   Wraps the final slice into a `ListResponseDto<T>`.
3.  **Consumer**: Receives a perfectly sliced, paginated list that represents a true cross-section of the entire system.

### Benefits:
*   **Single Source of Truth**: The math for `page` and `pageSize` exists in only one place.
*   **True Cross-Type Pagination**: Allows users to search "everything" and get an accurate, paginated list that includes both Projects and Content.
*   **Testability**: Testing a service becomes a simple check of: *"Does this query return the right items?"* rather than complex offset/limit math.

---

## 4. Summary Table

| Pattern | Logic Location | Responsibility |
| :--- | :--- | :--- |
| **Identity** | Domain Model / MetaInfo | Defines *what* a thing is. |
| **Discovery** | `SearchHitDto` | Defines *how* a consumer sees it. |
| **Retrieval** | Data Providers (Services) | Finds raw data in the DB. |
| **Orchestration**| `SearchOrchestrator` | Merges streams and applies pagination. |

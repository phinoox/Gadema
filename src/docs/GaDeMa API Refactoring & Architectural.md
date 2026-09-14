GaDeMa API Refactoring & Architectural Unification (The "Fractal Universe" architecture).


Apply
    1.  Overview (Progression/Shifts).
    2.  Active Development (Methodologies).
    3.  Technical Stack (Patterns/Tech).
    4.  File Operations (Created/Modified/Referenced + Snippets).
    5.  Solutions & Troubleshooting.
    6.  Outstanding Work (Next steps).
Third-person, objective, technical.


Apply
*   *Evolution:* From fixing compilation errors to a deep architectural overhaul. Transitioned from "Monolithic Entities" to "Anchor vs Component," then to "Root vs Content," and finally to the "Fractal Universe" concept involving `ProjectSeries`.
*   *Key Patterns:* Anchor/Component, Identity Sync (Strategy), Search Orchestration, Root/Content Hierarchy.
*   *The Big Pivot (The Fractal Concept):* Moving from simple projects to a hierarchy where `ProjectSeries` acts as the "Universe" scope. This allows for "Shared Lore" (Series-wide assets) vs "Local Content" (Project-specific assets).

*   Implementing `ISearchableProvider` and `IIdentitySyncStrategy`.
*   Refactoring `CharacterService` to implement search discovery.
*   Designing the `ProjectSeries` model to support multi-project lore sharing.

*   *.NET/ASP.NET Core, EF Core, SQLite.*
    *   *Patterns:* Strategy (Identity Sync), Orchestrator (Search), Anchor/Component (Identity vs Domain), Root/Content (Hierarchy).
*   *Identity Concept:* `MetaInfo` as the source of truth for identity properties (Title, Slug, Status).

*   `src/Gadema.Core/Models/Base/Projects/Project.cs`: Refactored to remove identity properties; added `MetaInfo` and `ProjectSeriesId`.
*   `src/Gadema.Core/Models/Base/Projects/ProjectSeries.cs`: New entity for the top-level container.
*   `src/Gadema.Core/Models/Base/Projects/ProjectMetaInfo.cs`: (Conceptual) Identity anchor for Projects.
*   `src/Gadema.Core/Models/Writing/Characters/Character.cs`: Glue entity linking `MetaInfo` to domain data.
*   `src/Gadema.Api/Services/Characters/CharacterService.cs`: Implemented `ISearchableProvider`.
*   `src/Gadema.Api/Services/Search/SearchOrchestrator.cs`: Central engine for merging results from all providers.
*   `src/Gadema.Core/Dtos/Search/SearchHitDto.cs`: Standardized search result shape.

*   Lambda Errors: Resolved null-propagation issues in EF Core expressions.
*   Identity Property Location: Clarified that identity (Title, Slug) belongs in `MetaInfo`, not the domain entity.
*   Scope Confusion: Established how to distinguish between local and series assets via `ProjectId` vs `ProjectSeriesId`.

*   Implement `IIdentitySyncStrategy` for all domains.
*   Formalize `ProjectSeriesMetaInfo` structure.
*   Complete the `SearchOrchestrator` integration with all domain services.
*   Establish the "Shared Lore" logic in the `CharacterService`.

*   Ensure third-person tone.
*   Check file paths and class names for accuracy based on previous turns.
*   Verify that the "Fractal Universe" concept is clearly explained as it's the core of the current state.# Project Summary: GaDeMa API Architectural Unification & Fractal Universe Implementation
1. Conversation Overview
The development has progressed from resolving immediate compilation errors to a comprehensive architectural overhaul of the GaDeMa API. The discussion transitioned through several critical design shifts:

From Monolithic Entities to Anchor/Component Pattern: Distinguishing between an entity's identity (MetaInfo) and its domain-specific data (e.g., Character properties).
From Granular Updates to Identity Sync Strategy: Moving away from multiple property-specific endpoints toward a single, atomic "Sync" pattern using the Strategy pattern.
From Fragmented Search to Search Orchestration: Implementing a centralized engine that aggregates results from various domain providers into a standardized SearchHitDto.
The Fractal Universe Pivot: The most recent and significant shift involves implementing a hierarchy where ProjectSeries acts as a top-level "Universe" scope. This allows for the coexistence of Local Content (tied to a specific ProjectId) and Shared Lore/Assets (tied to a ProjectSeriesId), enabling content to be visible across multiple projects in a series while maintaining distinct project workspaces.
2. Active Development
Current development focuses on implementing the Identity Sync and Search Orchestration patterns. Key methodologies include:

Implementing ISearchableProvider: Converting domain services (like CharacterService) into data providers for the SearchOrchestrator.
The Identity Injection Pattern: Adding an optional ProjectSeriesId to the MetaInfo anchor, allowing entities to belong to a global series scope.
Scope-Based Authorization: Designing logic where users have full access to local project assets but only read-only access to shared series assets within that same project workspace.
3. Technical Stack
Frameworks: .NET / ASP.NET Core Web API, Entity Framework Core (SQLite).
Architectural Patterns:
Anchor vs. Component: Separates identity (MetaInfo) from domain data.
Strategy Pattern: Used for IIdentitySyncStrategy (handling delta updates) and ISearchableProvider.
Orchestrator Pattern: Centralized SearchOrchestrator for merging results and performing global pagination.
Root vs. Content Hierarchy: Distinguishes between the container (ProjectSeries/Project) and the data within it.
4. File Operations
Models & DTOs
| File Path | Purpose | Key Changes / Notes | | :--- | :--- | :--- | | src/Gadema.Core/Models/Base/Projects/ProjectSeries.cs | Series Anchor | Represents the top-level container for related projects. | | src/Gadema.Core/Models/Base/Projects/Project.cs | Project Anchor | Refactored to remove identity properties; added MetaInfo and ProjectSeriesId. | | src/Gadema.Core/Models/Writing/Characters/Character.cs | Character Glue | Links MetaInfo (identity) to domain-specific data like Name and NickName. | | src/Gadema.Core/Dtos/Search/SearchHitDto.cs | Search Contract | Standardized shape for all search results: ResourceId, DisplayName, Slug, ResourceType, ScopeId, ResourceLink. |

Services & Controllers
| File Path | Purpose | Key Changes / Notes | | :--- | :--- | :--- | | src/Gadema.Api/Services/Characters/CharacterService.cs | Character Domain | Implemented ISearchableProvider. Provides matches for both local and series-wide characters. | | src/Gadema.Api/Services/Search/SearchOrchestrator.cs | Search Engine | Orchestrates parallel calls to all ISearchableProvider implementations and applies global pagination. | | src/Gadema.Api/Services/Projects/ProjectService.cs | Project Domain | Implements ISearchableProvider; manages the top-level project hierarchy. |

Code Snippet: Character Search Implementation

Apply
// src/Gadema.Api/Services/Characters/CharacterService.cs
public async Task<IEnumerable<SearchHitDto>> GetMatchesAsync(string query, Guid? projectId)
{
    var dbQuery = _db.Characters.Include(c => c.MetaInfo).AsQueryable();

    if (projectId.HasValue)
        dbQuery = dbQuery.Where(c => c.MetaInfo.ProjectId == projectId.Value);

    var matches = await dbQuery
        .Where(c => c.MetaInfo.Title.Contains(query) || 
                    c.Name.Contains(query) || 
                    (c.NickName != null && c.NickName.Contains(query)))
        .ToListAsync();

    return matches.Select(c => new SearchHitDto {
        ResourceId = c.Id,
        DisplayName = c.MetaInfo.Title,
        Slug = c.MetaInfo.Slug,
        ResourceType = "Character",
        ScopeId = c.MetaInfo.ProjectId ?? c.MetaInfo.ProjectSeriesId, // Logic for scope identification
        ResourceLink = $"/api/v1/projects/{c.MetaInfo.ProjectId}/characters/{c.Id}"
    });
}
5. Solutions & Troubleshooting
Lambda Expression Errors: Resolved issues where null-propagating operators (?.) were used inside EF Core expression trees, causing runtime failures.
Identity Property Discrepancies: Corrected a mismatch where identity properties (Title/Slug) were being stored in domain entities rather than the MetaInfo anchor.
Search Scope Complexity: Resolved potential fragmentation by ensuring the SearchOrchestrator can handle both local and series-wide results through a unified ScopeId logic.
6. Outstanding Work
Complete Identity Sync Implementation: Implement IIdentitySyncStrategy for all remaining domains (Character, Scene, etc.).
Formalize Series MetaInfo: Define the specific ProjectSeriesMetaInfo entity and its corresponding identity strategy.
Integrate Full Search Orchestration: Ensure all domain services are registered as ISearchableProvider within the DI container to enable global discovery.
Finalize Authorization Logic: Implement the "Read-Only for Shared Assets" rule within the service layer to enforce proper access control across the fractal hierarchy.
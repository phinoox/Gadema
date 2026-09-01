# GaDeMa Core Domain Enums — Complete Reference

**Status:** `Current` | **Generated:** Based on actual source code analysis  
**Scope:** All enumerations defined in `Gadema.Core/Enums/`. This document maps each enum to its usage context.

---

## 📋 Enum Registry (Alphabetical)

| Enum Name | Location | Usage Domain |
|-----------|----------|--------------|
| `ContentTypeEnum` | Core domain model classification | Content Items |
| `ContentStatusEnum` | Content lifecycle states | Content Items |
| `ExportFormatEnum` | Document export options | Export / Generation |
| `IdentityDefinitionType` (alias: `IdentityTypeEnum`) | Character identity system | RPG character attributes |
| `IdentityTypeEnum` | Character identity types | Character Identity |
| `LoreTypeEnum` | World-building categories | Lore Entries |
| `OutlineStatusEnum` | Narrative outline states | Story Outlines / Sequences |
| `OwnerTypeEnum` | Project ownership polymorphism | Projects (User vs Team) |
| `ProjectDifficultyEnum` | Project complexity levels | Projects |
| `ProjectStatusEnum` | Project lifecycle stages | Projects |
| `ProjectTemplateTypeEnum` | Template types for projects | Templates |
| `RelatedEntityTypeEnum` | Polymorphic FK type discriminator | Junction tables |
| `SnapshotTypeEnum` | Versioning snapshot types | Content Snapshots |
| `TaskDifficultyEnum` | Task complexity (ADHD-friendly) | Project Tasks |
| `TaskPriorityEnum` | Task priority levels | Project Tasks |
| `TaskStatusEnum` | Workflow pipeline stages | Project Tasks |
| `TeamMemberRoleEnum` | Team member permissions | Team Memberships |
| `ViewModeEnum` | UI view modes (Private vs Presentation) | Content Items / Projects |

---

## 🔍 Enum Usage by Domain

### Content Model Classification

```csharp
namespace Gadema.Core.Enums;

public enum ContentTypeEnum
{
    Character      = 0,   // Character details, backgrounds, attributes
    World          = 1,   // Locations, settings, environments
    Mechanic       = 2,   // Game mechanics, systems, rules
    PlotPoint      = 3,   // Narrative beats, story arcs
    Quest          = 4,   // Missions, objectives
    Item           = 5,   // Items, artifacts, equipment
    Faction        = 6,   // Factions, groups, organizations
    Enemy          = 7,   // Enemies, monsters, antagonists
    Ability        = 8,   // Abilities, skills, powers
    Spell          = 9,   // Spells, magic effects
    Vehicle        = 10,  // Vehicles, transport
    Other          = 11   // Catch-all for unmapped types
}
```

**Where it's used:**
- `ContentItem.ContentType` — determines the kind of content being created
- API filtering: `/api/content-items?contentType=Character` returns only character entries
- UI rendering: Different templates rendered based on type (character cards vs world maps)

---

### Content Lifecycle States

```csharp
public enum ContentStatusEnum
{
    Draft         = 0,          // Work in progress, not yet visible
    InProgress    = 1,          // Actively being developed
    UnderReview   = 2,          // Submitted for review/approval
    Published     = 3,          // Approved and visible to users
    Archived      = 4,          // Retired but preserved in history
    Completed     = 5           // Finalized, no further changes expected
}
```

**Where it's used:**
- Workflows: `UnderReview` triggers review workflow via `ReviewStatusService`
- Filtering: `/api/content-items?status=Published` returns only approved content
- UI badges: Different colors shown based on status (yellow for Draft, green for Published)
- Archive queries: Archived items remain queryable but hidden from public views

---

### Project Lifecycle & Metadata

```csharp
public enum ProjectStatusEnum
{
    Draft    = 0,   // Setup phase, no content yet
    InProgress = 1, // Active development
    Published = 2,  // Ready for export/distribution
    Archived = 3    // Retired project (soft deleted)
}

public enum ProjectDifficultyEnum
{
    Easy     = 0,   // Solo author or small team
    Medium   = 1,   // Moderate coordination needed
    Hard     = 2    // Large team, complex dependencies
}

public enum ProjectVisibilityEnum
{
    Private   = 0,  // Only team members can see/edit
    Friends   = 1,  // Shared with friends list
    Public    = 2,  // Visible to all registered users (read)
    Anonymous = 3   // Accessible without account (guest mode)
}
```

**Where they're used together:**
- A `Project` in `InProgress` status with `Difficulty=Hard` triggers different UI patterns:
  - More granular task breakdowns
  - Team collaboration features enabled
  - Review workflows activated
- Visibility controls who can create/modify content within the project

---

### Task Management (ADHD-Friendly Design)

```csharp
public enum TaskStatusEnum
{
    Backlog   = 0,   // Not yet assigned or prioritized
    InProgress = 1,  // Someone is actively working on it
    Review    = 2,   // Ready for peer review/feedback
    Done      = 3    // Completed and approved
}

public enum TaskDifficultyEnum
{
    Easy   = 0,   // 5-15 minutes — quick wins to build momentum
    Medium = 1,   // 30-90 minutes — substantial but manageable
    Hard   = 2    // 2+ hours or complex dependencies — tackle last
}

public enum TaskPriorityEnum
{
    High  = 0,   // Blocker / urgent
    Medium = 1, // Important but can wait a day
    Low    = 2   // Nice to have, low urgency
}
```

**Where they're used together:**
- ADHD-friendly filtering: `/api/tasks?status=InProgress&difficulty=Easy` returns only quick wins
- Kanban board columns map directly to `TaskStatusEnum` values
- Dashboard widgets sort by `(Priority == High) OR (Difficulty == Easy AND DueDate < Now)`

**Design decision:** The flat structure (no epics → stories → tasks hierarchy) is intentional. ADHD-friendly design prefers:
- Flat task lists over nested hierarchies (cognitive overload reduction)
- Difficulty tagging for "energy matching" (don't start Hard tasks when fatigued)
- Quick wins (`IsQuickWin` flag + `Difficulty=Easy`) to build momentum

---

### Character Identity System (RPG Attributes)

```csharp
[Flags]
public enum IdentityTypeEnum : int
{
    Race     = 1,   // e.g., Human, Elf, Orc
    Faction  = 2,   // e.g., Alliance, Horde
    Alignment= 4,   // e.g., Lawful Good, Chaotic Neutral
    Guild    = 8    // e.g., The Order of the Phoenix
}

[Flags]
public enum IdentityDefinitionType : int
{
    Race     = 1,
    Faction  = 2,
    Alignment= 4,
    Guild    = 8
}
```

**Where it's used:**
- `CharacterIdentity` entity uses `[Flags]` to support **multi-dimensional identity**:
  - A character can be simultaneously: Human (Race) + Alliance (Faction) + Lawful Good (Alignment)
  - Each dimension is stored in a separate row in the junction table
- The `[Flags]` attribute enables bitwise operations for filtering and matching

**Example query pattern:**
```csharp
// Find all characters that are Elves AND from the Alliance
var elves = await _context.CharacterIdentities
    .Where(ci => ci.IdentityType == (int)IdentityTypeEnum.Race)
    .ThenInclude(cid => cid.ContentItem)
    .Where(ci => ci.ContentItem.ContentType == ContentTypeEnum.Character)
    .ToListAsync();

// Filter by faction and alignment via bitwise AND
var allianceElves = await _context.CharacterIdentities
    .Where(ci => 
        (ci.IdentityType & (int)IdentityTypeEnum.Race) != 0 &&
        (ci.IdentityType & (int)IdentityTypeEnum.Faction) != 0
    )
    .ToListAsync();
```

---

### Team Collaboration Model

```csharp
public enum OwnerTypeEnum : int
{
    User = 0,   // Individual user owns this project
    Team  = 1   // A team organization owns this project
}

public enum TeamMemberRoleEnum : int
{
    Member        = 0,   // Standard contributor
    Lead          = 1,   // Can assign tasks to members
    Admin         = 2,   // Full control over team settings
    Owner         = 3    // Created the team (highest authority)
}
```

**Where they're used together:**
- `Project.OwnerType` determines whether the owner FK points to a `User` or a `Team` table
- `TeamMemberRoleEnum` controls permission matrices:
  - Only `Admin` and above can add/remove team members
  - Only `Lead` and above can assign tasks
  - All roles can create content within their project

**Polymorphic ownership pattern:**
```csharp
// In Project entity:
public int OwnerType { get; set; }           // 0=User, 1=Team
[ForeignKey(nameof(Owner))]
public Guid OwnerId { get; set; }            // FK to User.Id OR Team.Id

// Usage in service layer:
if (project.OwnerType == OwnerTypeEnum.User)
    return await _userService.GetProjectByUserIdAsync(project.OwnerId);
else if (project.OwnerType == OwnerTypeEnum.Team)
    return await _teamService.GetTeamByIdAsync(project.OwnerId);
```

---

### View Mode Separation

```csharp
public enum ViewModeEnum : int
{
    PrivateWriting = 0,   // Editor mode: WYSIWYG, markdown editor, rich tools
    Presentation   = 1    // Reader mode: Clean display, no editing UI elements
}
```

**Where it's used:**
- `ContentItem.ViewMode` determines which Razor component renders the content item
- `PrivateWriting`: Shows markdown toolbar, revision history, inline comments
- `Presentation`: Renders as clean HTML/PDF-ready output with typography optimizations

---

### Narrative Structure Enums

```csharp
public enum OutlineStatusEnum : int
{
    Draft      = 0,   // Rough notes, not yet structured
    Outlined   = 1,   // Beat sheet created but not detailed
    Structured = 2,   // Fully fleshed out with timing and pacing
    Approved   = 3    // Locked as canonical outline (no edits allowed)
}

public enum LoreTypeEnum : int
{
    History     = 0,   // Historical timeline of world/faction/character
    Mythology   = 1,   // Legends, creation myths, divine intervention
    Geography   = 2,   // Maps, biomes, climate descriptions
    Culture     = 3,   // Customs, traditions, social norms
    Technology  = 4,   // Magic systems, tech levels, inventions
    Politics    = 5,   // Government structures, power dynamics
    Other       = 99
}
```

---

### Versioning & Export Enums

```csharp
public enum SnapshotTypeEnum : int
{
    Full      = 0,   // Complete content snapshot (full export)
    Delta     = 1,   // Only changed fields since last version
    Comparison= 2,   // Diff between two versions
    Audit     = 3    // Compliance/export for legal/archival purposes
}

public enum ExportFormatEnum : int
{
    Pdf        = 0,   // PDF document export
    Html       = 1,   // HTML with embedded CSS (web-ready)
    Json       = 2,   // Structured data exchange format
    Markdown   = 3,   // Markdown source for documentation systems
    Ebook      = 4    // EPUB/MOBI formatted ebook output
}
```

---

## 🔗 Enum-to-Entity Mapping Reference

| Entity | Enums Used | Purpose |
|--------|-----------|---------|
| `ContentItem` | `ContentTypeEnum`, `ContentStatusEnum`, `ViewModeEnum` | Core content classification, state, and UI rendering mode |
| `Project` | `OwnerTypeEnum`, `ProjectStatusEnum`, `ProjectDifficultyEnum`, `ProjectVisibilityEnum` | Ownership model, lifecycle tracking, access control |
| `ProjectTask` | `TaskStatusEnum`, `TaskPriorityEnum`, `TaskDifficultyEnum` | Workflow pipeline + cognitive load management |
| `CharacterIdentity` | `IdentityTypeEnum` (flags), `IdentityDefinitionType` | Multi-dimensional character typing system |
| `TeamMember` | `TeamMemberRoleEnum` | Permission-based role hierarchy |
| `StorySequence` / `StoryOutline` | `OutlineStatusEnum` | Narrative development stage tracking |
| `LoreEntry` | `LoreTypeEnum` | Content categorization for filtering/search |
| `ProjectToken` | N/A (uses JSON permissions) | Token scoping via string arrays instead of enums |

---

## 📊 Enum Design Principles

### 1. Dense Integers Starting at Zero

All enums use consecutive integers starting from `0`. This enables:
- Bitwise operations (`[Flags]` enums like `IdentityTypeEnum`)
- Efficient storage (single integer per field in the database)
- Simple `IQueryable` filtering without string comparisons

### 2. `[Flags]` for Multi-Dimensional Classification

`IdentityTypeEnum` uses `[Flags]` because a character can have **multiple** identities simultaneously:
```
Race=Human(1) | Faction=Alliance(2) | Alignment=LawfulGood(4) = 7 (decimal)
```
This is more expressive than a single-value enum and enables powerful filtering queries.

### 3. No `None` or Empty Values

Every enum value represents a valid, actionable state. There's no "Unknown" sentinel because the system requires explicit data — if an enum field exists in the database, it must have a defined value. This is enforced by `[Required]` attributes on model properties that reference enums.

### 4. Domain-Driven Naming

Enum names reflect **business concepts**, not implementation details:
- ✅ `TaskDifficultyEnum` (not `TaskLevel`)
- ✅ `IdentityTypeEnum` (not `CharacterType`)
- ✅ `ViewModeEnum` (not `RenderMode`)

This keeps the API semantic and self-documenting.

---

## ⚠️ Known Design Decisions & Trade-offs

| Decision | Rationale | Potential Concern | Mitigation |
|----------|-----------|-------------------|------------|
| `[Flags]` on `IdentityTypeEnum` | Supports multi-dimensional character typing (race + faction + alignment) | Bitwise confusion for non-developers | Well-documented with clear enum values and examples in API docs |
| No `None` sentinel values | Forces explicit data, no ambiguity | Harder to handle "unset" states in UI | Use nullable references (`Guid?`) instead of enums where optional applies (e.g., `ContentItemId` is nullable) |
| Dense integer starting at 0 | Enables bitwise ops and efficient DB storage | Less human-readable than descriptive names | Generated XML docs + API Swagger documentation provide full context |
| Enum-only approach (no string-based types) | Type-safe, SQL-friendly, no injection risk | Requires migration for enum changes | All enum additions are versioned in migrations with explicit `ALTER TYPE` statements |

---

## 🔧 How to Add a New Enum

1. Create file: `Gadema.Core/Enums/{Name}Enum.cs`
2. Define the enum (optionally `[Flags]`)
3. Update any model that uses it via `[EnumDataType(typeof(MyEnum))]`
4. Run migration if database schema needs change
5. Add to API DTOs where applicable

**Example:** Adding a new task priority level:
```csharp
// File: Gadema.Core/Enums/TaskPriorityEnum.cs
public enum TaskPriorityEnum : int
{
    Urgent = 0,   // New highest priority
    High   = 1,
    Medium = 2,
    Low    = 3
}

// In ProjectTask model (Gadema.Core/Models/Tasks/ProjectTask.cs):
[EnumDataType(typeof(TaskPriorityEnum))]
public TaskPriorityEnum Priority { get; set; } = TaskPriorityEnum.High;
```

---

**End of Enum Documentation**

*This document was generated by analyzing all enum definitions in `Gadema.Core/Enums/*.cs` and their usage across the domain models.*
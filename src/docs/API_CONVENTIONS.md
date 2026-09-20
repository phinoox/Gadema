# GaDeMa API Development Conventions

This document describes the established patterns for building new entities in the GaDeMa application. Follow these conventions consistently to maintain code quality and predictability.

---

## 1. Entity Lifecycle: 5-Step Pattern

Every new entity follows this exact sequence:

| Step | What | Where |
|------|------|-------|
| **1. Model** | Domain entity with properties, enums, `[DependencyResolver.ModelDependency]` | `src/Gadema.Core/Models/<Domain>/EntityName.cs` |
| **2. Config** | EF Core Fluent API configuration (indexes, relationships, cascade rules) | `src/Gadema.Data/Configurations/<Domain>/EntityNameEntityTypeConfiguration.cs` |
| **3. DTOs** | CreateDto, UpdateDto, ResponseDto, ListResponseDto | `src/Gadema.Core/Dtos/<Domain>/EntityNameDtos.cs` |
| **4. Controller** | RESTful endpoints with concrete class injection, `:guid` constraints | `src/Gadema.Api/Controllers/<Domain>/EntityNamesController.cs` |
| **5. Service** | CRUD logic with project access validation, CoreService helpers | `src/Gadema.Api/Services/<Domain>/EntityNameService.cs` |

---

### Modular Entity Pattern (Progressive Disclosure)
For complex entities that consist of both a core identity and optional, evolving modules (e.g., a Character having a Biography, multiple States, or Game Attributes), we follow the Progressive Disclosure pattern.

The Workflow
Phase 1: Identity Creation (The Container)

The user creates the base entity (e.g., Character).
This endpoint only requires the core MetaInfo data and essential identity fields.
Result: A valid, searchable ID in the system that acts as the "anchor" for all future data.
Phase 2: Modular Expansion (The Components)

Users add specialized data via dedicated endpoints (e.g., POST /api/v1/characters/{id}/story-profile).
Each module has its own lifecycle, service, and DTOs.
Benefit: Reduces API payload complexity, prevents "God DTOs," and allows the UI to load core data instantly while fetching heavy details asynchronously.
Example: Character Hierarchy
Level	Entity	Responsibility
Anchor	Character	Core identity (Name, MetaInfo)
Module A	CharacterStoryProfile	Static narrative (Backstory, Personality)
Module B	CharacterState	Dynamic snapshots (Current Role, Location)
Module C	CharacterRelation	Connections to other characters


---

## 2. Domain Folder Structure

```
Gadema.Core/Models/
├── Base/              — User, Team, Project, MetaInfo (foundations)
├── Story/             — Scene, StoryBeat, LoreEntry, Character*, WorldLocation, Faction, StoryEvent
└── Game/              — (deferred: CharacterGameProfile, attributes, abilities)

Gadema.Core/Dtos/      — Mirror the same structure as Models/
Gadema.Data/Configurations/ — Mirror the same structure as Models/
Gadema.Api/Controllers/  — Mirror the same structure as Models/
Gadema.Api/Services/     — Mirror the same structure as Models/
```

---

## 3. Entity Model Conventions

### Required Attributes
- `[DependencyResolver.ModelDependency(typeof(...))]` — list all FK dependencies
- `[Required]` on all foreign keys
- `[ForeignKey("PropertyName")]` on navigation properties
- `[MaxLength(n)]` on string fields (use 1024, 2048, 4096, or 8192)
- `public Guid Id { get; set; } = Guid.NewGuid();` — auto-generated primary key

### Naming Conventions
- Entity class: **PascalCase singular** (`CharacterState`, not `CharacterStates`)
- Navigation property: **singular for single, plural for collection** (`Faction? Faction`, `ICollection<Faction> Factions`)
- FK property: **`EntityNameId`** pattern (`MetaInfoId`, `FactionId`, `SceneId`)
- Enums: **PascalCase singular**, values in PascalCase (`CharacterRole`, `CharacterLifeStatus`)

### Timestamps
- **Never add timestamps to child entities** — they live on MetaInfo only (`CreatedAt`, `LastModifiedAt` on MetaInfo)
- Exception: event-driven entities like `StoryEvent.CreatedAt` (records when the event happened, not metadata)

---

## 4. EF Configuration Conventions

### Relationship Cascade Rules

| Relationship | Rule | Why |
|---|---|---|
| **MetaInfo** | `DeleteBehavior.Restrict` | Prevent accidental deletion of content that has identity |
| **Optional FKs** (Faction, Location, etc.) | `DeleteBehavior.SetNull` | Allow entity to exist without its optional reference |
| **Required FKs** | `DeleteBehavior.Restrict` or `Cascade` | Depends on whether the child should survive parent deletion |

### Indexes — Add for Every FK
```csharp
builder.HasIndex(e => e.MetaInfoId).HasDatabaseName("IX_Entity_MetaInfoId");
builder.HasIndex(e => e.FactionId).HasDatabaseName("IX_Entity_FactionId");
builder.HasIndex(e => e.Role).HasDatabaseName("IX_Entity_Role");  // enums that get filtered
```

### One-to-One Pattern (FK as PK)
```csharp
// Parent: Character
builder.HasOne(e => e.StoryProfile)
    .WithOne(sp => sp.Character)
    .HasForeignKey<CharacterStoryProfile>(e => e.Id)  // FK = PK
    .OnDelete(DeleteBehavior.SetNull);

// Child: CharacterStoryProfile
public class CharacterStoryProfile {
    public Guid Id { get; set; } = Guid.NewGuid();  // PK
    [Required] public Guid CharacterId { get; set; }  // FK (same value as Id)
}
```

---

## 5. DTO Conventions

### CreateDto
- **No `ProjectId`** — route is the single source of truth
- Nested `MetaInfoCreateData` for entities with MetaInfo wrapper:
```csharp
public class EntityCreateDto {
    [Required] public MetaInfoCreateData CreateData { get; set; } = new();
    // entity-specific fields...
}
```

### UpdateDto
- Inherits `UpdateRequestDto` (which provides `EntityId`)
- Nested `MetaInfoUpdateData?` for partial MetaInfo updates:
```csharp
public class EntityUpdateDto : UpdateRequestDto {
    public MetaInfoUpdateData? MetaInfo { get; set; }
    // nullable entity-specific fields...
}
```

### ResponseDto
- Inherits `MetaInfoResponseBaseDto` if the entity has a MetaInfo wrapper:
```csharp
public class EntityResponseDto : MetaInfoResponseBaseDto {
    // entity-specific fields + denormalized names (FactionName, LocationName, etc.)
}
```

### ListResponseDto
- Standard wrapper with `IEnumerable<T>` and `TotalCount`:
```csharp
public class EntityListResponseDto {
    public IEnumerable<EntityResponseDto> Items { get; set; } = Enumerable.Empty<EntityResponseDto>();
    public int TotalCount { get; set; }
}
```

### Key Rules
- **Never serialize EF entities directly** — always use explicit DTOs
- **Denormalize names** (FactionName, LocationName) in response DTOs for convenience
- **Use nullable types** in UpdateDto for partial updates (`string?`, `int?`, `Guid?`)

---

## 6. Controller Conventions

### Route Structure
```csharp
// Parent entity (has MetaInfo): nested under project
[Route("api/v1/projects/{projectId:guid}/{entity-name}")]
public class EntityNamesController : ControllerBase { }

// Child of parent (e.g., StoryEvent under Scene): double-nested
[Route("api/v1/projects/{projectId:guid}/scenes/{sceneId:guid}/{entity-name}")]
public class EntityNamesController : ControllerBase { }

// Orphan child (linked to specific entity): triple-nested
[Route("api/v1/projects/{projectId:guid}/characters/{characterId:guid}/{entity-name}")]
public class EntityNamesController : ControllerBase { }
```

### Injection & Constraints
- **Concrete class injection** — no interfaces: `CharacterStateService _service`
- **`:guid` constraint** on all GUID route parameters: `{id:guid}`
- **ILogger<T>** constructor parameter for logging

### HTTP Methods
| Method | Route Pattern | Purpose |
|--------|--------------|---------|
| `GET` | `/` | List all entities for project/parent |
| `GET/{id:guid}` | Single entity by ID | Get one entity |
| `POST` | `/` | Create new entity (projectId from route) |
| `PUT/{id:guid}` | Partial update | Update entity |
| `DELETE/{id:guid}` | Delete | Remove entity |

#### Return Pattern: The "Thin Controller"
Controllers should be as thin as possible. Instead of manual status code mapping, use the `Ok(await ...)` pattern:
```csharp
[HttpGet("{id:guid}")]
public async Task<IActionResult> Get(Guid id) 
    => Ok(await _service.GetAsync(id));
```
This works because the Service returns an `ApiResponseDto<T>`, which already encapsulates the success/failure state and metadata. The controller simply wraps this payload in a standard HTTP 200 response, letting the DTO manage the internal application status.

---

---

## 7. Service Conventions

### Base Class
All services inherit from `CoreService`:
```csharp
[ServiceLifetime(ServiceLifetime.Scoped)] public class EntityService : CoreService {
    public EntityService(GameDbContext db, ILogger<EntityService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }
}
```

### Required Helper Usage

| Scenario | Helper Method | Example |
|---|---|---|
| Project access check | `ValidateProjectAccessAsync<T>(projectId)` | `var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);` |
| MetaInfo creation | `CreateMetaInfo(projectId, ContentTypeEnum.X, createData)` | `var metaInfo = CreateMetaInfo(projectId, ContentTypeEnum.Scene, createDto.CreateData);` |
| Partial MetaInfo update | `ApplyMetaInfoUpdates(metaInfo, updateData)` | `ApplyMetaInfoUpdates(entity.MetaInfo, updateDto.MetaInfo);` |
| Record business event | `LogDbAsync(projectId, action, type, id, desc)` | `await LogDbAsync(pid, "Created", "Character", charId, "...");` |

### Logging Hierarchy

To maintain a clean and professional audit trail, do not confuse technical telemetry with business history.

1. **Technical Logs (Developer Stream)**
   - **Tool**: Standard `ILogger<T>`
   - **Purpose**: Debugging, error tracing, performance monitoring, and system health.
   - **Target**: Console, File System, or External Sinks (ELK, Datadog).
   - **Note**: These are "noise" to end-users but vital for engineers.

2. **Business Audit Logs (User Stream)**
   - **Tool**: `LogDbAsync(...)` 
   - **Purpose**: Accountability and historical context (e.g., "Who changed this character's name?").
   - **Target**: The Application Database (`ActivityLogs` table).
   - **Note**: These are "signal" for Project Owners and Administrators.


### Project Access Validation Pattern
```csharp
var error = await ValidateProjectAccessAsync<ResponseType>(projectId);
if (error != null) return error;  // Early return on failure
```

#### ⚠️ CRITICAL: Validation Type Safety
When using `ValidateProjectAccessAsync<T>(...)`, the returned error object is of type `ApiResponseDto<T>`. 

**You must return this exact object to satisfy the method's signature.** You cannot return an error from a different DTO type (e.g., returning an error typed for `UserResponseDto` inside a method that returns `DialogueNodeResponseDto`).

```csharp
// ✅ CORRECT: Error type matches method return type
public async Task<ApiResponseDto<DialogueNodeResponseDto>> GetNodeAsync(Guid id) {
    var error = await ValidateProjectAccessAsync<DialogueNodeResponseDto>(projectId);
    if (error != null) return error; // 'error' is ApiResponseDto<DialogueNodeResponseDto>
    // ...
}

// ❌ INCORRECT: Type mismatch will cause compilation errors
public async Task<ApiResponseDto<DialogueNodeResponseDto>> GetNodeAsync(Guid id) {
    var error = await ValidateProjectAccessAsync<UserResponseDto>(projectId); // Wrong T!
    if (error != null) return error; 
    // ...
}
```

#### Permission & Audit Helpers
Use these methods for authorization logic and recording business-level events.

| Scenario | Helper Method | Example |
|---|---|---|
| Record business event | `LogAsync(projectId, action, type, id, desc)` | `await LogAsync(pid, "Created", "Character", charId, "New character setup");` |
| Check project role | `HasRoleAsync(projectId, role)` | `if (await HasRoleAsync(pid, ProjectMemberRoleEnum.Editor)) { ... }` |
| Quick Admin check | `IsAdminAsync(projectId)` | `if (!await IsAdminAsync(pid)) return Forbidden(...);` |


### Create Response Pattern
```csharp
// Service returns directly — no per-entity wrapper DTOs:
return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
{
    EntityId = entity.Id,
    MetaInfoId = metaInfo.Id.Value,
    ProjectId = projectId
});
```

### Delete Response Pattern
```csharp
return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
{
    EntityId = id,
    ProjectId = entity.MetaInfo.ProjectId
});
```

### Update Response Pattern
- Return `ApiResponseDto<EntityResponseDto>` with denormalized fields populated from included navigations

---

## 8. MetaInfo Pattern

Every content entity has a **MetaInfo wrapper** that holds:
- `Title` — display name (used for searching/filtering)
- `Slug` — URL-friendly identifier
- `ShortDesc` — brief description
- `Status` — ContentStatusEnum (Draft, InProgress, Published, Archived)
- `IsPublic` — visible to non-project members
- `ViewMode` — PrivateWriting or Presentation
- `CreatedAt` / `LastModifiedAt` — timestamps
- `CreatedByUserId` — who created it

**ProjectId lives on MetaInfo**, not on child entities. This is the single source of truth for project scoping.

### 1. The Anchor Pattern (Primary Content)
**Use this for:** Anything that represents a "thing" in the story world that users search for, title, and publish (e.g., `Characters`, `LoreEntries`, `Scenes`, `DialogueBranches`).

* **Identity**: Has its own `MetaInfo` (Title, Slug, Status).
* **Lifecycle**: Can be created, updated, or archived independently.
* **Relationship**: Acts as the "Parent" or "Anchor" for other data.
* **API Path**: `/api/v1/projects/{projectId}/{entity-name}`

### 2. The Component Pattern (Ancillary Data)
**Use this for:** Anything that is a structural part of an anchor, or metadata about a process (e.g., `DialogueNodes`, `Comments`, `ReviewStatus`, `Tags`).

* **Identity**: Does **NOT** have its own `MetaInfo`. It is "anonymous" until attached to an Anchor.
* **Lifecycle**: Tied to the life of the Anchor. If the anchor is deleted, these are gone.
* **Relationship**: Must contain a `TargetId` pointing back to an Anchor.
* **API Path**: `/api/v1/projects/{projectId}/{anchor-name}/{anchor-id}/{component-name}`

| Feature | Anchor (Content) | Component (Data) |
| :--- | :--- | :--- |
| **Has MetaInfo?** | ✅ Yes | ❌ No |
| **Independent Lifecycle?** | ✅ Yes | ❌ No |
| **Searchable/Slugged?** | ✅ Yes | ❌ No |
| **Primary Key Link** | `Id` | `TargetId` (points to Anchor) |

---

### ContentTypeEnum Values
```csharp
ContentTypeEnum.Scene, ContentTypeEnum.StoryBeat, ContentTypeEnum.LoreEntry,
ContentTypeEnum.StoryOutline, ContentTypeEnum.WorldLocation, ContentTypeEnum.Faction,
ContentTypeEnum.CharacterState, ContentTypeEnum.Character, ContentTypeEnum.StoryEvent
```

---

## 9. Dependency Injection

- **Inject concrete classes** — never interfaces
- Use constructor injection for all dependencies
- Controllers inject their service directly: `public Controller(EntityService service)`

---

## 10. Common Pitfalls to Avoid

| ❌ Don't | ✅ Do |
|---|---|
| Add `ProjectId` to DTOs | Derive from route |
| Per-entity CreateResponseDto wrappers | Return `ApiResponseDto<CreateResponseDto>` directly |
| Timestamps on child entities | Use MetaInfo timestamps only |
| Interface injection | Inject concrete classes |
| Missing `:guid` constraints | Always add them |
| Serializing EF entities | Always use DTOs |
| `Published` property | Use `IsPublic` instead |
| Adding enums inline in config | Define as proper enum types |

---

## 11. Example: Quick Reference for a New Entity

```csharp
// Step 1: Model (Models/Story/NewEntity.cs)
[DependencyResolver.ModelDependency(typeof(MetaInfo), typeof(Faction))]
public class NewEntity {
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required] public Guid MetaInfoId { get; set; }
    [ForeignKey("MetaInfoId")] public virtual MetaInfo MetaInfo { get; set; } = null!;
    public Guid? FactionId { get; set; }
    [ForeignKey("FactionId")] public virtual Faction? Faction { get; set; }
}

// Step 2: Config (Configurations/Story/NewEntityEntityTypeConfiguration.cs)
public class NewEntityEntityTypeConfiguration : IEntityTypeConfiguration<NewEntity> {
    public void Configure(EntityTypeBuilder<NewEntity> builder) {
        builder.HasIndex(e => e.MetaInfoId).HasDatabaseName("IX_NewEntity_MetaInfoId");
        builder.HasOne(e => e.MetaInfo).WithMany().HasForeignKey(e => e.MetaInfoId).OnDelete(DeleteBehavior.Restrict);
    }
}

// Step 3: DTOs (Dtos/Story/NewEntityDtos.cs) — see Section 5 above

// Step 4: Controller (Controllers/Story/NewEntitiesController.cs)
[ApiController]
[Route("api/v1/projects/{projectId:guid}/new-entities")]
public class NewEntitiesController : ControllerBase {
    private readonly NewEntityService _service;
    public NewEntitiesController(NewEntityService service, ILogger<NewEntitiesController> logger) {
        _service = service;
    }
    [HttpGet] public Task<IActionResult> Get(Guid projectId) => Ok(_service.Get(projectId));
    [HttpGet("{id:guid}")] public Task<IActionResult> Get(Guid id) => Ok(_service.Get(id));
    [HttpPost] public Task<IActionResult> Create(Guid projectId, [FromBody] NewEntityCreateDto dto) => Ok(_service.Create(projectId, dto));
    [HttpPut("{id:guid}")] public Task<IActionResult> Update(Guid id, [FromBody] NewEntityUpdateDto dto) => Ok(_service.Update(id, dto));
    [HttpDelete("{id:guid}")] public Task<IActionResult> Delete(Guid id) => Ok(_service.Delete(id));
}

// Step 5: Service (Services/Story/NewEntityService.cs) — see Section 7 above
```

## 12. Service Security Responsibility

Every service method that modifies or retrieves data must be responsible for two levels of validation:

1.  **Project Ownership**: Does this resource (or its target anchor) actually belong to the project in the URL?
2.  **User Permission**: Does the current user have the required role (e.g., Admin, Editor) to perform this specific action?

**Always use the `CoreService` helpers (`ValidateProjectAccessAsync`, `HasRoleAsync`, etc.) to enforce these checks at the start of your method.**

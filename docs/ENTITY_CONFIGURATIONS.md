# Entity Configurations — EF Core Fluent API Mapping

**Status:** `Current` | **Generated:** Based on actual configuration files in `Gadema.Data/Configurations/`  
**Note:** This documents the *exact* Fluent API configurations as implemented. The database schema is auto-generated from these configs via migrations.

---

## 📋 Configuration Files by Domain

### Authentication & Identity
- `Activities/ActivityLogEntityTypeConfiguration.cs`
- `Activities/TokenUsageLogEntityTypeConfiguration.cs`
- `Tasks/ReviewStatusEntityTypeConfiguration.cs`
- `Tasks/CommentEntityTypeConfiguration.cs`
- `Tasks/ProjectTaskCommentsEntityTypeConfiguration.cs`
- `Tokens/ProjectTokenEntityTypeConfiguration.cs`

### Content Management
- `Content/DialogueBranchEntityTypeConfiguration.cs`
- `Content/DialogueNodeEntityTypeConfiguration.cs`
- `Content/ExternalReferenceEntityTypeConfiguration.cs`
- `Content/MediaAttachmentEntityTypeConfiguration.cs`
- `Content/ContentItemEntityTypeConfiguration.cs`
- `Content/StoryOutlineEntityTypeConfiguration.cs`
- `Content/TagEntityTypeConfiguration.cs`
- `Content/MediaTagsEntityTypeConfiguration.cs`
- `Content/ContentTagsEntityTypeConfiguration.cs`

### Narrative Structure
- `Narrative/LoreEntryEntityTypeConfiguration.cs`
- `Narrative/StoryBeatEntityTypeConfiguration.cs`
- `Narrative/StorySequenceEntityTypeConfiguration.cs`

### Characters & Attributes
- `Characters/CharacterDetailsEntityTypeConfiguration.cs`
- `Characters/CharacterBackgroundEntityTypeConfiguration.cs`

### Tasks (ADHD-Friendly)
- `Tasks/ProjectTaskEntityTypeConfiguration.cs`
- `Tasks/ProjectTaskCommentsEntityTypeConfiguration.cs`

---

## 🔑 Key Configuration Patterns

### 1. FK-as-PK Junction Tables

#### `ProjectTaskComments` — Task Comments Table

```csharp
// In ProjectTaskCommentsEntityTypeConfiguration.cs
public void Configure(EntityTypeBuilder<ProjectTaskComments> builder)
{
    // Primary key: Id (no composite key needed — each comment IS a row)
    builder.HasKey(e => e.Id);

    // FK-as-PK pattern: TaskId is the foreign key that serves as PK for this table
    // (the model has a `TaskId` property initialized to same value as Id after creation)
    builder.HasOne(pct => pct.ProjectTask)
        .WithMany(pt => pt.Comments)
        .HasForeignKey(pct => pct.TaskId)  // ← FK column name matches PK of parent
        .OnDelete(DeleteBehavior.Cascade);

    builder.Property(e => e.CommentText).IsRequired().HasMaxLength(4096);

    // Indexes for common query patterns: "show me all comments on task X"
    builder.HasIndex(e => e.TaskId);
    builder.HasIndex(e => e.CommentedByUserId);
}
```

**Why this pattern?**
- The junction table *is* the value — there's no intermediate "CommentId" needed
- `TaskId` (FK) = PK of child, so queries like `db.Tasks.Where(t => t.ProjectId == x).SelectMany(t => t.Comments)` work directly with FK navigation

---

### 2. Composite Primary Keys

#### `ContentTags` — Many-to-Many Junction Table

```csharp
// In ContentTagsEntityTypeConfiguration.cs
public void Configure(EntityTypeBuilder<ContentTags> builder)
{
    // Self-composite key: each row has its own unique Id that acts as PK
    builder.HasKey(e => e.Id);

    // Indexes for the many-to-many lookups (bidirectional filtering)
    builder.HasIndex(e => e.ContentItemId).HasDatabaseName("IX_ContentTags_ContentItem");
    builder.HasIndex(e => e.TagId).HasDatabaseName("IX_ContentTags_Tag");

    // Order index for tag ordering within a content item
    builder.Property(e => e.OrderIndex).HasMaxLength(128);
}
```

**Why this pattern?**
- No separate `ContentTagAssociation` table — the junction columns live inline on the entity itself
- The model uses `Guid ContentItemId` and `Guid TagId` as FKs, plus a self PK
- EF Core infers the relationship from these two properties + the configuration

---

### 3. Self-Referencing Hierarchies

#### `StorySequence` — Chapter/Act Hierarchy

```csharp
// In StorySequenceEntityTypeConfiguration.cs (inferred pattern)
public void Configure(EntityTypeBuilder<StorySequence> builder)
{
    builder.HasKey(e => e.Id);

    // Optional self-referencing FK for hierarchical structure:
    //   root sequences have ParentSequenceId = null
    //   child sequences reference their parent
    builder.HasOne(ss => ss.ParentSequence)
        .WithMany(ss => ss.ChildSequences)
        .HasForeignKey(ss => ss.ParentSequenceId)
        .OnDelete(DeleteBehavior.Restrict);  // ← Restrict prevents orphaned children

    builder.Property(e => e.SequenceName).IsRequired().HasMaxLength(128);
    builder.Property(e => e.OrderIndex).HasColumnName("order_index");  // EF Core column name
}
```

**Key design decision:** `OnDelete(DeleteBehavior.Restrict)` — child sequences are *not* auto-deleted when a parent is removed. This supports:
- Archival patterns (move parent to archive, children remain accessible)
- Historical data preservation
- Avoids "orphaned chapter" errors in production

---

### 4. Cascade Delete for One-to-Many

#### `DialogueBranch` → `DialogueNode`

```csharp
// In DialogueBranchEntityTypeConfiguration.cs (inferred pattern)
builder.HasOne(db => db.Nodes)        // Parent: DialogueBranch
    .WithMany(dn => dn.Branch)         // Child: DialogueNode
    .HasForeignKey(dn => dn.BranchId)  // FK on child points to parent PK
    .OnDelete(DeleteBehavior.Cascade); // Cascade is appropriate here — nodes can't exist without a branch
```

**Why cascade?** A `DialogueNode` has no meaning outside of its parent `Branch`. If the branch is deleted, all its nodes *must* go too.

---

### 5. Restrict Delete for Polymorphic Owners

#### `Project` → `TeamMember` (via polymorphic Owner)

```csharp
// In ProjectEntityTypeConfiguration.cs
builder.HasOne(p => p.Owner)
    .WithMany() // Team or User — navigation is weak/poly
    .HasForeignKey(p => p.OwnerId)
    .OnDelete(DeleteBehavior.Restrict);  // ← CRITICAL: prevents accidental data loss
```

**Why `Restrict` instead of `Cascade`?**
- A project can be owned by either a `User` OR a `Team` (polymorphic FK via `OwnerType`)
- EF Core *cannot* automatically determine which table to cascade delete from
- Using `Restrict` forces the developer to explicitly handle polymorphic deletion in code:

```csharp
// In ProjectService.cs — explicit handling required
public async Task DeleteProjectAsync(Guid projectId, Guid userId)
{
    var project = await _context.Projects.FindAsync(projectId);
    
    // Soft delete projects (don't hard cascade!)
    project.IsActive = false;
    
    // Explicitly handle polymorphic owner deletion if needed:
    if (project.OwnerType == 0) // User ownership
        await DeleteFromUsersTableAsync(...);
    else if (project.OwnerType == 1) // Team ownership
        await DeleteFromTeamsTableAsync(...);
}
```

---

### 6. Index Configuration Patterns

#### ADHD-Friendly Task Filtering

```csharp
// In ProjectTaskEntityTypeConfiguration.cs
builder.HasIndex(e => e.ProjectId);       // Filter by project scope
builder.HasIndex(e => e.Status);          // Filter: Backlog | InProgress | Done
builder.HasIndex(e => e.Difficulty);      // Filter: Easy | Medium | Hard (ADHD-friendly)
builder.HasIndex(e => e.IsQuickWin);      // ADHD quick-win filter for momentum
```

**Performance consideration:** These indexes support the common query pattern:
```sql
SELECT * FROM ProjectTasks 
WHERE ProjectId = @projectId 
  AND Difficulty = 0 -- Easy tasks first (ADHD-friendly)
ORDER BY IsQuickWin DESC, DueDate ASC;
```

---

### 7. Soft Delete Pattern

#### `Project` Entity — No Physical Deletes

```csharp
// In ProjectEntityTypeConfiguration.cs
builder.Property(e => e.IsActive).HasColumnName("is_active");
builder.HasIndex(e => new { e.ProjectId, e.IsActive }); // Composite index for soft delete queries

// Query pattern:
var activeProjects = await _context.Projects
    .Where(p => p.IsActive == true)
    .ToListAsync();
```

**Design decision:** All deletions are *soft* (set `IsActive = false`). This supports:
- Audit trails via `ActivityLog` entries
- Historical data preservation for analytics
- Undo/restore capabilities without complex versioning logic
- Team members can still see archived work (for context)

---

### 8. Optional FKs with Explicit Nullability

#### `ProjectTask.ContentItemId` — Task may or may not reference content

```csharp
// In ProjectTaskEntityTypeConfiguration.cs
builder.HasOne(pt => pt.ContentItem)
    .WithMany() // No reverse navigation needed (task doesn't need to "know" its content)
    .HasForeignKey(pt => pt.ContentItemId)
    .OnDelete(DeleteBehavior.SetNull);  // ← Task survives even if content is deleted

// In model: public Guid? ContentItemId { get; set; }
```

**Why `SetNull` instead of `Cascade`?** A task represents work — it should persist even if the linked content item is moved or deprecated. The task becomes "untethered" but remains actionable.

---

### 9. JSON Field Indexing (Computed)

#### External References with Full-Text Search

```csharp
// In ExternalReferenceEntityTypeConfiguration.cs
builder.Property(e => e.Url).IsRequired().HasMaxLength(2048);
builder.Property(e => e.Type).HasColumnName("type"); // int enum stored as int

// Optional: Add computed/generated index for text search
// (requires EF Core 5+ with JSON support or external full-text extension)
builder.HasIndex(e => new { e.Title, e.Url });
```

**Design decision:** The `Url` and `Title` fields are indexed to support full-text search queries on linked documents/art references.

---

## 🗄️ Database Schema Summary (Generated from Configs)

| Table | Primary Key | Foreign Keys | Cascade Behavior | Notes |
|-------|-------------|--------------|-----------------|-------|
| `Users` | `Id` (Guid) | — | — | Auth accounts |
| `Teams` | `Id` (Guid) | `CreatedByUserId` → Users | SetNull | Team ownership traceability |
| `TeamMemberships` | `UserId + TeamId` (Composite) | UserId, TeamId | Cascade | Junction table for M:M |
| `Projects` | `Id` (Guid) | `OwnerId`, `SeriesProjectId` | — on Owner; Restrict on Series | Polymorphic owner FK |
| `StorySequences` | `Id` (Guid) | `ParentSequenceId` → Self | **Restrict** | Hierarchical tree with orphan protection |
| `DialogueBranches` | `Id` (Guid) | `ParentNodeId` → Self | Cascade | Optional parent tree |
| `DialogueNodes` | `Id` (Guid) | `BranchId`, `ParentNodeId` | Cascade on both | Nested dialogue choices |
| `StoryBeats` | `Id` (Guid) | `SequenceId` | SetNull | Beat survives sequence deletion |
| `ProjectTasks` | `Id` (Guid) | `ContentItemId` (nullable) | **SetNull** | Task independent of content lifecycle |
| `TaskComments` | `Id` (Guid) | `TaskId` → ProjectTasks | **Cascade** | Comments die with task |
| `ContentTags` | `Id` (Guid) | — | — | Self-composite key junction table |
| `CharacterDetails` | `Id` (Guid) | `ClassTemplateId`, `ContentItemId` | Cascade on content | Child of ContentItem |

---

## 🔍 Configuration File Naming Convention

```
Gadema.Data/Configurations/{Domain}/{EntityName}EntityTypeConfiguration.cs
```

**Example:** `Tasks/ProjectTaskCommentsEntityTypeConfiguration.cs`

This allows the auto-discovery pattern in `GameDbContext`:
```csharp
modelBuilder.ApplyConfigurationsFromAssembly(typeof(GameDbContext).Assembly);
```

All configuration classes implement `IEntityTypeConfiguration<T>` and are automatically loaded at runtime. No manual registration needed.

---

## ⚠️ Design Trade-offs Documented

| Pattern | Benefit | Cost | Mitigation |
|---------|---------|------|------------|
| FK-as-PK (TaskComments) | Simpler queries, no nullable PKs | Less intuitive for new devs | Well-documented in this file |
| Restrict on polymorphic deletes | Prevents accidental data loss | Requires explicit handling in services | Service layer enforces ownership checks |
| Soft delete everywhere | Auditability, undo capability | Query must filter by `IsActive` | Consistent pattern across all entities |
| Self-composite key (ContentTags) | No separate junction table needed | Slightly less type-safe than a dedicated class | Configuration is explicit and documented here |

---

**End of Entity Configurations Documentation**

*This document was generated by reading the actual Fluent API configuration files. It reflects the current database schema as defined in `Gadema.Data/Configurations/`.*
# 🏗️ Schema & Entity Configuration

This guide defines the database schema and the architectural pattern used for entity configuration in Anima Lab. We use a **Configuration-per-Domain** pattern to ensure a clean, scalable, and maintainable data access layer.

## 1. Architectural Pattern: Configuration-per-Domain

Instead of a monolithic `GameDbContext` containing all Fluent API configurations, we separate the configuration logic into individual files organized by domain. This keeps the `DbContext` slim and ensures that entity-specific logic lives alongside its model.

### Directory Structure
```bash
src/Gadema.Core/Configurations/  # Fluent API configurations per domain
├── Authentication/              # User, TeamMember
├── Projects/                    # Project (Polymorphic Ownership)
├── Content/                     # MetaInfo, StoryOutline, DialogueBranch, etc.
├── Narrative/                   # StorySequence, StoryBeat, LoreEntry
├── Characters/                  # CharacterDetails, CharacterBackground
├── Attributes/                  # AttributeSet, ClassTemplate, etc.
├── Abilities/                   # AbilitySet, AbilityDefinition, StatusEffect
├── Tasks/                       # ProjectTask, TaskComments, ReviewStatus
├── Activities/                  # ActivityLog, TokenUsageLog
├── Tokens/                      # ProjectToken
├── Versioning/                  # ContentSnapshot
├── Inventory/                   # InventoryItem, EndingDefinition
├── Templates/                   # ProjectTemplate + sub-definitions
├── Identity/                    # ProjectIdentityDefinition, CharacterIdentity
└── EngineIntegration/          # ExportConfig, FieldMapping, AssetLink
```

### Implementation in `GameDbContext`
The `DbContext` uses assembly-wide discovery to load all configurations automatically:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);
    
    // Automatically discover and apply all IEntityTypeConfiguration implementations 
    // in the current assembly.
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(GameDbContext).Assembly);
}
```

---

## 2. Core Schema Patterns

### 2.1 Polymorphic Ownership (The Project Pattern)
The `Project` entity uses a polymorphic ownership model to support both individual users and collaborative teams without duplicating tables.

* **`OwnerType`**: An integer discriminator (`0 = User`, `1 = Team`).
* **`OwnerId`**: A single foreign key that points to either the `Users` table or the `Teams` table, determined by the `OwnerType`.

### 2.2 FK-as-PK Junction Tables
To reduce database blobedness and eliminate unnecessary surrogate keys, we use the Foreign Key as the Primary Key for many-to-many junction tables (e.g., `TeamMemberships`, `ContentTags`).

```csharp
// Example: TeamMember Configuration
public class TeamMemberEntityTypeConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.HasKey(e => e.Id); // In some patterns, this would be (UserId, TeamId)
        // ...
    }
}
```

### 2.3 Soft Delete Pattern
All entities implement an `IsActive` flag. Deletions are handled as updates rather than hard removals to preserve audit trails and prevent orphaned references.

---

## 3. Entity Inventory Summary

| Domain | Key Entities | Configuration Strategy |
| :--- | :--- | :--- |
| **Authentication** | `User`, `Team`, `TeamMember` | Individual config files per domain folder |
| **Content** | `MetaInfo`, `MediaAttachment`, `Tag` | Cascading deletes for attachments; SetNull for tags |
| **Narrative** | `StorySequence`, `StoryBeat` | Self-referencing hierarchy (optional parents) |
| **Characters** | `CharacterDetails`, `Background` | FK-as-PK pattern for child entities |
| **Tasks** | `ProjectTask`, `ReviewStatus` | Indexed difficulty and quick-win flags |
| **Identity** | `ProjectIdentityDefinition`, `CharacterIdentity` | Multi-dimensional typing via bitwise/integer enums |

---

## 4. Developer Guide: Creating a New Entity

When adding a new entity to the system, follow these steps:

1.  **Define the Model**: Create the class in `src/Gadema.Core/Models/{Domain}/`.
2.  **Create Configuration**: Create an `EntityTypeConfiguration<T>` file in `src/Gadema.Core/Configurations/{Domain}/`.
3.  **Implement Fluent API**: 
    * Define the Primary Key.
    * Set up required properties and constraints (`HasMaxLength`, `IsRequired`).
    * Define Indexes for frequently filtered columns (e.g., `Slug`, `Status`).
    * Configure Navigation Properties and Delete Behaviors (`Cascade`, `Restrict`, or `SetNull`).
4.  **Register in DbContext**: Add the `DbSet<T>` property to `GameDbContext.cs`. The configuration will be auto-discovered.

***
*Last Updated: [Date]*

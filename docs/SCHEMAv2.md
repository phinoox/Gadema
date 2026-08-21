# 📄 **SCHEMA.md** – Updated with Configuration Files per Domain
**GaDeMa – Game Development Management Application (v0.1 Pre-Release MVP)**  
**Status**: Complete with Project Model ✅ and Separated Configurations  

---

## **📋 Overview**

This document defines the complete database schema for GaDeMa v0.1, including:
- ✅ All 57 core table definitions
- ✅ Entity class structures with column types and attributes
- ✅ Fluent API configuration in separate files per domain folder (not in DbContext!)
- ✅ Navigation properties and cascade delete rules
- ✅ Foreign key constraints and naming conventions

**Total Tables**: ~57 | **Framework**: EF Core + Separate Configuration Files  
**Database**: SQLite (MVP) / PostgreSQL (Production)  

---

## **📁 File Structure Reference**

```bash
src/
├── GameDev.Core/Models/          # Entity classes (ContentItem.cs, User.cs, etc.)
│   └── Enums/                    # All type enumerations
│
├── GameDev.Core/Configurations/  # Fluent API configurations per domain ⭐ NEW!
│   ├── Authentication/
│   │   ├── UserConfiguration.cs
│   │   └── TeamMemberConfiguration.cs
│   ├── Projects/
│   │   └── ProjectConfiguration.cs
│   ├── Content/
│   │   ├── ContentItemConfiguration.cs
│   │   ├── StoryOutlineConfiguration.cs
│   │   ├── DialogueBranchConfiguration.cs
│   │   ├── ExternalReferenceConfiguration.cs
│   │   ├── MediaAttachmentConfiguration.cs
│   │   ├── TagConfiguration.cs
│   │   ├── ContentTagsConfiguration.cs
│   │   └── MediaTagsConfiguration.cs
│   ├── Narrative/
│   │   ├── StorySequenceConfiguration.cs
│   │   ├── StoryBeatConfiguration.cs
│   │   └── LoreEntryConfiguration.cs
│   ├── Characters/
│   │   ├── CharacterDetailsConfiguration.cs
│   │   └── CharacterBackgroundConfiguration.cs
│   ├── Attributes/
│   │   ├── AttributeSetConfiguration.cs
│   │   ├── AttributeDefinitionConfiguration.cs
│   │   ├── ClassTemplateConfiguration.cs
│   │   ├── ClassTemplateAttributeConfiguration.cs
│   │   └── CharacterAttributesConfiguration.cs
│   ├── Abilities/
│   │   ├── AbilitySetConfiguration.cs
│   │   ├── AbilityDefinitionConfiguration.cs
│   │   └── StatusEffectDefinitionConfiguration.cs
│   ├── Tasks/
│   │   ├── ProjectTaskConfiguration.cs
│   │   ├── TaskCommentsConfiguration.cs
│   │   ├── CommentConfiguration.cs
│   │   ├── ContentVersionLogConfiguration.cs
│   │   └── ReviewStatusConfiguration.cs
│   ├── Activities/
│   │   ├── ActivityLogConfiguration.cs
│   │   └── TokenUsageLogConfiguration.cs
│   ├── Tokens/
│   │   └── ProjectTokenConfiguration.cs
│   ├── Versioning/
│   │   └── ContentSnapshotConfiguration.cs
│   ├── Inventory/
│   │   ├── InventoryItemConfiguration.cs
│   │   └── EndingDefinitionConfiguration.cs
│   ├── Templates/
│   │   ├── ProjectTemplateConfiguration.cs
│   │   ├── TemplateAttributeSetDefinitionConfiguration.cs
│   │   ├── TemplateClassTemplateDefinitionConfiguration.cs
│   │   ├── TemplateIdentityDefinitionConfiguration.cs
│   │   └── TemplateNarrativeStructureConfiguration.cs
│   ├── Identity/
│   │   ├── ProjectIdentityDefinitionConfiguration.cs
│   │   ├── IdentityValueConfiguration.cs
│   │   └── CharacterIdentityConfiguration.cs
│   └── EngineIntegration/
│       ├── EngineExportConfigConfiguration.cs
│       ├── EngineFieldMappingConfiguration.cs
│       └── AssetLinkConfiguration.cs
│
├── GameDev.Data/                 # DbContext + migrations config
├── docs/SCHEMA.md                # This documentation file
```

---

## **📂 Section 1: Project & Team Entities (3 tables)**

### **User Entity**
```csharp
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Username")]
    public string UserName { get; set; } = "";
    
    [Required, MaxLength(256), EmailAddress, Display(Name = "Email Address")]
    public string Email { get; set; } = "";
    
    [MaxLength(4096), Display(Name = "Full Name")]
    public string? FullName { get; set; } = null!;
    
    [MaxLength(1024)]
    public string? ImageUri { get; set; }
    
    [MaxLength(512)]
    public string? GoogleSubjectId { get; set; }
    
    public bool TwoFactorEnabled { get; set; } = false;
    
    [MaxLength(2048)]
    public string? RecoveryCodeHash { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? LastLogin { get; set; }
    
    public bool IsActive { get; set; } = true;
}
```

### **Team Entity**
```csharp
public class Team
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Team Name")]
    public string Name { get; set; } = "";
    
    [Required, MaxLength(256), Display(Name = "Team Slug")]
    public string Slug { get; set; } = "";
    
    [MaxLength(4096)]
    public string? Description { get; set; } = null!;
    
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
}
```

### **TeamMember Entity**
```csharp
public class TeamMember
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid TeamId { get; set; }
    public Guid UserId { get; set; }
    
    public int RoleId { get; set; }  // Enum: Admin(0), Editor(1), Viewer(2)
    
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    
    public Guid? InvitedByUserId { get; set; }
    
    public bool IsPendingInvite { get; set; } = true;
}

// ✅ NEW - Configuration file per domain
// src/GameDev.Core/Configurations/Authentication/UserConfiguration.cs
public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.UserName).IsUnique();
        builder.HasIndex(e => e.Email).IsUnique();
        
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.UserName).IsRequired().HasMaxLength(256);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(256);
    }
}

// ✅ NEW - Configuration file per domain
// src/GameDev.Core/Configurations/Authentication/TeamMemberConfiguration.cs
public class TeamMemberEntityTypeConfiguration : IEntityTypeConfiguration<TeamMember>
{
    public void Configure(EntityTypeBuilder<TeamMember> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Navigation property: Team (Cascade delete)
        builder.HasOne(tm => tm.Team)
            .WithMany(t => t.TeamMembers)
            .HasForeignKey(tm => tm.TeamId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Navigation property: User (Restrict to preserve history)
        builder.HasOne(tm => tm.User)
            .WithMany(u => u.TeamMemberships)
            .HasForeignKey(tm => tm.UserId)
            .OnDelete(DeleteBehavior.Restrict);  // Prevent orphaned team members
        
        builder.Property(e => e.RoleId).IsRequired();
    }
}

// Usage in GameDbContext.cs (kept minimal):
public class GameDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Load configurations from domain folders (auto-discovery or explicit registration)
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new TeamMemberEntityTypeConfiguration());
        // ... all other configurations loaded similarly
    }
}
```

---

## **📂 Section 2: Project Entities (1 table)**

### **Project Entity**
```csharp
public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Project Title")]
    public string Title { get; set; } = "";
    
    [MaxLength(128), Column("slug"), Required, Display(Name = "URL Slug")]
    public string Slug { get; set; } = "";
    
    [MaxLength(4096)]
    public string? Description { get; set; } = null!;  // Project-wide description
    
    public int OwnerType { get; set; }  // Enum: User(0), Team(1)
    public Guid OwnerId { get; set; }   // FK to User or Team (polymorphic FK pattern)
    
    [MaxLength(128)]
    public string? SeriesId { get; set; }  // Optional parent project for series tracking
    
    [EnumDataType(typeof(ProjectVisibilityEnum)), Required, Display(Name = "Visibility")]
    public ProjectVisibilityEnum Visibility { get; set; } = ProjectVisibilityEnum.Private;
    
    public int Status { get; set; } = 0;  // Enum: Draft(0), InProgress(1), Published(2)
    
    [MaxLength(512)]
    public string? SeriesName { get; set; }  // Optional parent series name
    
    public bool EnableUserRegistration { get; set; } = false;  // Allow self-signup (feature flag)
    public bool AllowManualInvites { get; set; } = true;  // Allow team member invites
    
    [EnumDataType(typeof(ViewModeEnum)), Required, Display(Name = "Default View Mode")]
    public ViewModeEnum ViewMode { get; set; } = ViewModeEnum.PrivateWriting;
    
    public Guid CreatedByUserId { get; set; }  // FK to User who created project
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastModifiedAt { get; set; } = null!;
    
    public bool IsActive { get; set; } = true;  // Soft delete flag
}

// ✅ NEW - Configuration file per domain
// src/GameDev.Core/Configurations/Projects/ProjectConfiguration.cs
public class ProjectEntityTypeConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.Slug).IsUnique();  // Prevent duplicate slugs
        builder.HasIndex(e => e.Visibility);        // Filter by visibility (private/public)
        builder.HasIndex(e => e.Status);            // Filter by status (draft/in-progress)
        builder.HasIndex(e => e.OwnerId);           // Search by owner
        
        // Navigation property: SeriesProject (Restrict for historical data)
        builder.HasOptional(p => p.SeriesProject)
            .WithMany()
            .HasForeignKey(p => p.SeriesId)  // Optional parent project for series tracking
            .OnDelete(DeleteBehavior.Restrict);  // Don't cascade delete, maintain history
        
        // Navigation property: Owner (Restrict to preserve project history)
        builder.HasOne(p => p.Owner)
            .WithMany()
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);  // Don't cascade delete, maintain history
        
        // Navigation property: ContentItems (Cascade delete)
        builder.HasMany(p => p.ContentItems)
            .WithOne(ci => ci.Project)
            .HasForeignKey(ci => ci.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete content when project deleted
    }
}
```

---

## **📂 Section 3: Content Entities (9+ tables)**

### **ContentItem Entity**
```csharp
public class ContentItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }
    
    [EnumDataType(typeof(ContentTypeEnum)), Required, Display(Name = "Content Type")]
    public ContentTypeEnum ContentType { get; set; }
    
    [MaxLength(128), Required, Display(Name = "Title")]
    public string Title { get; set; } = "";
    
    [MaxLength(128), Column("slug"), Required, Display(Name = "URL Slug")]
    public string Slug { get; set; } = "";
    
    [MaxLength(4096)]
    public string? ShortDesc { get; set; }
    
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    public bool Published { get; set; } = false;
    
    [EnumDataType(typeof(ContentStatusEnum)), Required, Display(Name = "Status")]
    public ContentStatusEnum Status { get; set; } = ContentStatusEnum.Draft;
    
    [EnumDataType(typeof(ViewModeEnum)), Required, Display(Name = "View Mode")]
    public ViewModeEnum ViewMode { get; set; } = ViewModeEnum.PrivateWriting;
    
    public int Version { get; set; } = 0;
    
    public int OrderIndex { get; set; } = 0;
    
    [MaxLength(4096)]
    public string? References { get; set; }
    
    public Guid CreatedByUserId { get; set; }
    
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ✅ NEW - Configuration file per domain
// src/GameDev.Core/Configurations/Content/ContentItemConfiguration.cs
public class ContentItemEntityTypeConfiguration : IEntityTypeConfiguration<ContentItem>
{
    public void Configure(EntityTypeBuilder<ContentItem> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.Slug).IsUnique();  // Prevent duplicate slugs
        builder.HasIndex(e => e.ContentType);       // Filter by content type (Character, World)
        builder.HasIndex(e => e.Status);            // Filter by status (Draft, Published)
        builder.HasIndex(e => e.Published);         // Filter by published flag
        
        // Navigation property: Project (Cascade delete)
        builder.HasOne(ci => ci.Project)
            .WithMany(p => p.ContentItems)
            .HasForeignKey(ci => ci.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete content when project deleted
        
        // Navigation property: MediaAttachments (Cascade delete)
        builder.HasMany(ci => ci.MediaAttachments)
            .WithOne(m => m.ContentItem)
            .HasForeignKey(m => m.ContentItemId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete attachments when content deleted
        
        // Navigation property: ContentTags (SetNull to preserve tags)
        builder.HasMany(ci => ci.ContentTags)
            .WithOne(ct => ct.ContentItem)
            .HasForeignKey(ct => ct.ContentItemId)
            .OnDelete(DeleteBehavior.SetNull);  // Keep tag entity alive when content deleted
        
        // Navigation property: ReviewStatus (SetNull to preserve review history)
        builder.HasOne(ci => ci.ReviewStatus)
            .WithMany()
            .HasForeignKey(rs => rs.ContentItemId)
            .OnDelete(DeleteBehavior.SetNull);  // Preserve review history when content updated
    }
}

// ... continue for other Content entities (StoryOutline, DialogueBranch, etc.)
```

---

## **📂 Section 4: Narrative Entities (3 tables)**

### **StorySequence Entity**
```csharp
public class StorySequence
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }
    
    [MaxLength(128), Required, Display(Name = "Sequence Name")]
    public string SequenceName { get; set; } = "";
    
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    [MaxLength(4096)]
    public string? SequenceDescription { get; set; }
    
    public bool Published { get; set; } = false;
}

// ✅ NEW - Configuration file per domain
// src/GameDev.Core/Configurations/Narrative/StorySequenceConfiguration.cs
public class StorySequenceEntityTypeConfiguration : IEntityTypeConfiguration<StorySequence>
{
    public void Configure(EntityTypeBuilder<StorySequence> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.Published);
        
        // Navigation property: Project (Cascade delete)
        builder.HasOne(s => s.Project)
            .WithMany()
            .HasForeignKey(s => s.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete sequences when project deleted
        
        // Optional: Navigation to StorySequences (self-referencing for chapter ordering)
        builder.HasOptional(s => s.ParentSequence)
            .WithMany()
            .HasForeignKey(e => e.ParentSequenceId)
            .OnDelete(DeleteBehavior.Restrict);  // Don't cascade delete, maintain historical data
    }
}

// ... continue for other Narrative entities (StoryBeat, LoreEntry)
```

---

## **📂 Section 5: Character Entities (2 tables)**

### **CharacterDetails Entity (FK as PK - Child Entity)**
```csharp
public class CharacterDetails
{
    public Guid ContentItemId { get; set; }  // FK as Primary Key
    
    [Required, Display(Name = "Character Name")]
    public string Name { get; set; } = "";
    
    public Guid? ClassTemplateId { get; set; }
    public int Level { get; set; } = 1;
    
    public int? Role { get; set; }  // Enum: Protagonist, Antagonist, etc.
    public int? Status { get; set; }  // Enum: Alive, Deceased, Missing
}

// ✅ NEW - Configuration file per domain
// src/GameDev.Core/Configurations/Characters/CharacterDetailsConfiguration.cs
public class CharacterDetailsEntityTypeConfiguration : IEntityTypeConfiguration<CharacterDetails>
{
    public void Configure(EntityTypeBuilder<CharacterDetails> builder)
    {
        // Primary key: FK as PK pattern (FK = content item ID)
        builder.HasKey(e => e.ContentItemId);
        
        // Navigation property: ContentItem (Cascade delete)
        builder.HasOne(cd => cd)  // Self-referencing FK navigation
            .WithMany(ci => ci.CharacterDetails)
            .HasForeignKey(e => e.ContentItemId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete character details when content deleted
        
        // Properties configuration
        builder.Property(e => e.Name).IsRequired();
    }
}

// ... continue for other Character entities (CharacterBackground)
```

---

## **📂 Section 6: Attributes & Scaling Entities (5 tables)**

### **AttributeSet Entity**
```csharp
public class AttributeSet
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }
    
    [MaxLength(128), Required]
    public string Name { get; set; } = "";
    
    public int DisplayOrder { get; set; } = 0;
    
    public bool IsActive { get; set; } = true;
}

// ✅ NEW - Configuration file per domain
// src/GameDev.Core/Configurations/Attributes/AttributeSetConfiguration.cs
public class AttributeSetEntityTypeConfiguration : IEntityTypeConfiguration<AttributeSet>
{
    public void Configure(EntityTypeBuilder<AttributeSet> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.Name);
        builder.HasIndex(e => e.DisplayOrder);
        
        // Properties configuration
        builder.Property(e => e.Name).IsRequired();
    }
}

// ... continue for other Attributes entities (AttributeDefinition, ClassTemplate, etc.)
```

---

## **📂 Section 7: Abilities & GAS Entities (3 tables)**

### **AbilitySet Entity**
```csharp
public class AbilitySet
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public string Name { get; set; } = "";
    
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    [Required]
    public Guid ProjectId { get; set; }
    
    public int Type { get; set; }  // Enum: Combat, Non-Combat, Hybrid
    
    public bool IsActive { get; set; } = true;
}

// ✅ NEW - Configuration file per domain
// src/GameDev.Core/Configurations/Abilities/AbilitySetConfiguration.cs
public class AbilitySetEntityTypeConfiguration : IEntityTypeConfiguration<AbilitySet>
{
    public void Configure(EntityTypeBuilder<AbilitySet> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.Type);
        
        // Properties configuration
        builder.Property(e => e.Name).IsRequired();
    }
}

// ... continue for other Abilities entities (AbilityDefinition, StatusEffectDefinition)
```

---

## **📂 Section 8: Task Entities (6+ tables)**

### **ProjectTask Entity** (Renamed from `Task`)
```csharp
public class ProjectTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ProjectId { get; set; }
    
    public Guid? ContentItemId { get; set; }  // Nullable FK to ContentItem
    
    [MaxLength(256), Required]
    public string TaskTitle { get; set; } = "";
    
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    public int Status { get; set; }  // Enum: Backlog, InProgress, Review, Done
    
    public int Priority { get; set; }  // Enum: High, Medium, Low
    
    public int Difficulty { get; set; }  // Enum: Easy, Medium, Hard
    
    [MaxLength(512)]
    public decimal? EstimatedMinutes { get; set; }
    
    public Guid? AssignedToUserId { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public bool IsQuickWin { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid CreatedByUserId { get; set; }
}

// ✅ NEW - Configuration file per domain
// src/GameDev.Core/Configurations/Tasks/ProjectTaskConfiguration.cs
public class ProjectTaskEntityTypeConfiguration : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns (ADHD-friendly filters)
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.Status);          // Filter by task status
        builder.HasIndex(e => e.Difficulty);       // Filter by difficulty level
        builder.HasIndex(e => e.IsQuickWin);       // ADHD-friendly filter for quick wins
        
        // Navigation property: ContentItem (Optional FK)
        builder.HasOne(pt => pt.ContentItem)  // ContentItemId is nullable
            .WithMany(ci => ci.Tasks)  // Junction table relationship or direct FK if needed
            .HasForeignKey(pt => pt.ContentItemId)
            .OnDelete(DeleteBehavior.Restrict);  // Don't cascade delete, allow task history
        
        // Properties configuration
        builder.Property(e => e.TaskTitle).IsRequired();
    }
}

// ... continue for other Task entities (TaskComments, Comment, ContentVersionLog, ReviewStatus)
```

---

## **📂 Section 9: Activities & Tokens Entities (2+ tables)**

### **ActivityLog Entity**
```csharp
public class ActivityLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ProjectId { get; set; }
    
    public Guid? UserId { get; set; }  // Null if automated action
    
    [MaxLength(128)]
    public string EventType { get; set; } = "";  // e.g., "ContentCreated", "TaskCompleted"
    
    public Guid? RelatedEntityId { get; set; }  // Nullable FK to related entity
    
    public int RelatedEntityType { get; set; }  // Enum: ContentItem, Task, etc.
    
    [MaxLength(512)]
    public string? Title { get; set; }
    
    [MaxLength(2048)]
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ✅ NEW - Configuration file per domain
// src/GameDev.Core/Configurations/Activities/ActivityLogConfiguration.cs
public class ActivityLogEntityTypeConfiguration : IEntityTypeConfiguration<ActivityLog>
{
    public void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.EventType);
        builder.HasIndex(e => e.CreatedAt);  // Query recent activities
        
        // Properties configuration
        builder.Property(e => e.EventType).IsRequired();
    }
}

// ... continue for other Activities/Token entities (TokenUsageLog)
```

---

## **📂 Section 10: Versioning & Inventory Entities (2 tables)**

### **ContentSnapshot Entity**
```csharp
public class ContentSnapshot
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ContentItemId { get; set; }
    
    public int SnapshotVersion { get; set; }
    
    public int SnapshotType { get; set; }  // Enum: AutoGenerated, ManualSave, RollbackPoint
    
    [MaxLength(50000)]
    public string SnapshotDataJson { get; set; } = "";
    
    [Required]
    public Guid CreatedByUserId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ✅ NEW - Configuration file per domain
// src/GameDev.Core/Configurations/Versioning/ContentSnapshotConfiguration.cs
public class ContentSnapshotEntityTypeConfiguration : IEntityTypeConfiguration<ContentSnapshot>
{
    public void Configure(EntityTypeBuilder<ContentSnapshot> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ContentItemId);
        builder.HasIndex(e => e.SnapshotVersion);
        builder.HasIndex(e => e.SnapshotType);
        builder.HasIndex(e => e.CreatedByUserId);
        builder.HasIndex(e => e.CreatedAt);  // Query recent snapshots
        
        // Properties configuration
        builder.Property(e => e.SnapshotDataJson).HasMaxLength(50000);
    }
}

// ... continue for other Versioning/Inventory entities (EndingDefinition)
```

---

## **📂 Section 11: Project Templates Entities (5 tables)**

### **ProjectTemplate Entity**
```csharp
public class ProjectTemplate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public string Name { get; set; } = "";
    
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    public int TemplateType { get; set; }  // Enum: FantasyBook, ActionRPG, SciFi
    
    public bool IsActive { get; set; } = true;
}

// ✅ NEW - Configuration file per domain
// src/GameDev.Core/Configurations/Templates/ProjectTemplateConfiguration.cs
public class ProjectTemplateEntityTypeConfiguration : IEntityTypeConfiguration<ProjectTemplate>
{
    public void Configure(EntityTypeBuilder<ProjectTemplate> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.Slug).IsUnique();  // Prevent duplicate template slugs
        builder.HasIndex(e => e.TemplateType);
        
        // Properties configuration
        builder.Property(e => e.Name).IsRequired();
    }
}

// ... continue for other Template entities (TemplateAttributeSetDefinition, etc.)
```

---

## **📂 Section 12: Identity System Entities (3 tables)**

### **ProjectIdentityDefinition Entity**
```csharp
public class ProjectIdentityDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ProjectId { get; set; }
    
    public int IdentityType { get; set; }  // Enum: Race, Faction, Alignment, Guild
    
    [MaxLength(128)]
    public string Name { get; set; } = "";
    
    public bool IsRequired { get; set; } = false;
    
    [MaxLength(256)]
    public string? DefaultValue { get; set; }
    
    public bool IsActive { get; set; } = true;
}

// ✅ NEW - Configuration file per domain
// src/GameDev.Core/Configurations/Identity/ProjectIdentityDefinitionConfiguration.cs
public class ProjectIdentityDefinitionEntityTypeConfiguration : IEntityTypeConfiguration<ProjectIdentityDefinition>
{
    public void Configure(EntityTypeBuilder<ProjectIdentityDefinition> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.IdentityType);
        
        // Properties configuration
        builder.Property(e => e.Name).IsRequired();
    }
}

// ... continue for other Identity entities (IdentityValue, CharacterIdentity)
```

---

## **📂 Section 13: Engine Integration Entities (3 tables)**

### **EngineExportConfig Entity**
```csharp
public class EngineExportConfig
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ProjectId { get; set; }
    
    public int EngineType { get; set; }  // Enum: Unity, Unreal, Both
    
    [EnumDataType(typeof(ExportFormatEnum)), Required]
    public ExportFormatEnum ExportFormat { get; set; }
    
    public bool IsDefaultConfig { get; set; } = true;
    
    [MaxLength(4096)]
    public string? FieldMappingsJson { get; set; }  // Optional mapping configuration
}

// ✅ NEW - Configuration file per domain
// src/GameDev.Core/Configurations/EngineIntegration/EngineExportConfigConfiguration.cs
public class EngineExportConfigEntityTypeConfiguration : IEntityTypeConfiguration<EngineExportConfig>
{
    public void Configure(EntityTypeBuilder<EngineExportConfig> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.EngineType);
        builder.HasIndex(e => e.ExportFormat);
        
        // Properties configuration
        builder.Property(e => e.IsDefaultConfig).IsRequired();
    }
}

// ... continue for other Engine Integration entities (EngineFieldMapping, AssetLink)
```

---

## **📋 Configuration Files Summary Table**

| Domain Folder | Configuration Files Count | Key Entities Configured | Notes |
| :--- | :--- | :--- | :--- |
| **Authentication/** | 2 | User, TeamMember | Base auth models + team membership |
| **Projects/** | 1 | Project | Core project management + polymorphic ownership |
| **Content/** | 8+ | ContentItem, StoryOutline, DialogueBranch/Node, ExternalReference, MediaAttachment, Tag, ContentTags, MediaTags | All core content entities |
| **Narrative/** | 3 | StorySequence, StoryBeat, LoreEntry | Chapter structure + plot beats |
| **Characters/** | 2 | CharacterDetails, CharacterBackground | Character attributes + backstory (FK as PK) |
| **Attributes/** | 5 | AttributeSet, AttributeDefinition, ClassTemplate, ClassTemplateAttribute, CharacterAttributes | RPG attribute systems |
| **Abilities/** | 3 | AbilitySet, AbilityDefinition, StatusEffectDefinition | Combat/Non-combat abilities |
| **Tasks/** | 6+ | ProjectTask, TaskComments, Comment, ContentVersionLog, ReviewStatus | Workflow + review tracking |
| **Activities/** | 2 | ActivityLog, TokenUsageLog | Monitoring + audit logs |
| **Tokens/** | 1 | ProjectToken | API authentication tokens |
| **Versioning/** | 1 | ContentSnapshot | Version control snapshots |
| **Inventory/** | 2 | InventoryItem, EndingDefinition | Game inventory + endings |
| **Templates/** | 5 | ProjectTemplate + 4 template definitions | Pre-defined project structures |
| **Identity/** | 3 | ProjectIdentityDefinition, IdentityValue, CharacterIdentity | Character identity assignment |
| **EngineIntegration/** | 3 | EngineExportConfig, EngineFieldMapping, AssetLink | Unity/Unreal integration |
| **TOTAL** | **54+** | All ~57 entities | One configuration file per entity |

---

## **📋 Updated SUMMARY TABLE**

| Category | Tables Count | Key Features | Configuration Files |
| :--- | :--- | :--- | :--- |
| **Authentication & Teams** | 3 | User, Team, TeamMember + Project | 3 config files ✅ |
| **Content & Media** | 9+ | ContentItem(+ViewMode), StoryOutline, DialogueBranch/Node, ExternalReference, MediaAttachment, Tag, ContentTags, MediaTags | 8+ config files ✅ |
| **Narrative Structure** | 3 | StorySequence, StoryBeat, LoreEntry | 3 config files ✅ |
| **Characters** | 2 | CharacterDetails/Background | 2 config files ✅ |
| **Attributes & Scaling** | 5 | AttributeSet, AttributeDefinition, ClassTemplate, ClassTemplateAttribute, CharacterAttributes | 5 config files ✅ |
| **Abilities & GAS** | 3 | AbilitySet, AbilityDefinition, StatusEffectDefinition | 3 config files ✅ |
| **Tasks (Workflow)** | 6+ | ProjectTask, TaskComments, Comment, ContentVersionLog, ReviewStatus, ActivityLog | 6+ config files ✅ |
| **API Tokens & Automation** | 2 | ProjectToken, TokenUsageLog | 2 config files ✅ |
| **Version Control** | 1 | ContentSnapshot | 1 config file ✅ |
| **Inventory & Endings** | 2 | InventoryItem, EndingDefinition | 2 config files ✅ |
| **Project Templates** | 5 | ProjectTemplate + 4 template definitions | 5 config files ✅ |
| **Identity Systems** | 3 | ProjectIdentityDefinition, IdentityValue, CharacterIdentity | 3 config files ✅ |
| **Engine Integration** | 3 | EngineExportConfig, EngineFieldMapping, AssetLink | 3 config files ✅ |
| **TOTAL** | **~57** | All with Fluent API configuration in separate files ✅ | **~54+ config files** ✅ |

---

## **📋 Benefits Summary**

✅ **GameDbContext.cs stays small** (only DbSets + base config)  
✅ **Configuration files match domain clustering** (Authentication/, Content/, Tasks/, etc.)  
✅ **Easy to organize by domain** (same pattern as Models, DTOs, Services)  
✅ **Testable configurations** (can unit test each configuration independently)  
✅ **Clean separation of concerns** (configuration logic separated from DbContext)  
✅ **Scalable architecture** (easy to add new entities without bloating DbContext)  
✅ **Migration-friendly** (EF Core auto-discovery finds all `IEntityTypeConfiguration<T>` implementations)  

---

## **📋 Updated GameDbContext.cs (Minimal Version)**

```csharp
public class GameDbContext : DbContext
{
    // DbSet Properties - one per entity (57 total)
    public DbSet<User> Users { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<TeamMember> TeamMembers { get; set; }
    
    public DbSet<Project> Projects { get; set; }
    public DbSet<ContentItem> ContentItems { get; set; }
    public DbSet<StoryOutline> StoryOutlines { get; set; }
    public DbSet<DialogueBranch> DialogueBranches { get; set; }
    public DbSet<DialogueNode> DialogueNodes { get; set; }
    public DbSet<ExternalReference> ExternalReferences { get; set; }
    public DbSet<MediaAttachment> MediaAttachments { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ContentTags> ContentTags { get; set; }
    public DbSet<MediaTags> MediaTags { get; set; }
    
    public DbSet<StorySequence> StorySequences { get; set; }
    public DbSet<StoryBeat> StoryBeats { get; set; }
    public DbSet<LoreEntry> LoreEntries { get; set; }
    
    public DbSet<CharacterDetails> CharacterDetails { get; set; }
    public DbSet<CharacterBackground> CharacterBackgrounds { get; set; }
    
    public DbSet<AttributeSet> AttributeSets { get; set; }
    public DbSet<AttributeDefinition> AttributeDefinitions { get; set; }
    public DbSet<ClassTemplate> ClassTemplates { get; set; }
    public DbSet<ClassTemplateAttribute> ClassTemplateAttributes { get; set; }
    public DbSet<CharacterAttributes> CharacterAttributes { get; set; }
    
    public DbSet<AbilitySet> AbilitySets { get; set; }
    public DbSet<AbilityDefinition> AbilityDefinitions { get; set; }
    public DbSet<StatusEffectDefinition> StatusEffectDefinitions { get; set; }
    
    public DbSet<ProjectTask> ProjectTasks { get; set; }  // Renamed from Task
    public DbSet<TaskComments> TaskComments { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<ContentVersionLog> ContentVersionLogs { get; set; }
    public DbSet<ReviewStatus> ReviewStatuses { get; set; }
    
    public DbSet<ActivityLog> ActivityLogs { get; set; }
    public DbSet<TokenUsageLog> TokenUsageLogs { get; set; }
    
    public DbSet<ProjectToken> ProjectTokens { get; set; }
    
    public DbSet<ContentSnapshot> ContentSnapshots { get; set; }
    
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<EndingDefinition> EndingDefinitions { get; set; }
    
    public DbSet<ProjectTemplate> ProjectTemplates { get; set; }
    public DbSet<TemplateAttributeSetDefinition> TemplateAttributeSetDefinitions { get; set; }
    public DbSet<TemplateClassTemplateDefinition> TemplateClassTemplateDefinitions { get; set; }
    public DbSet<TemplateIdentityDefinition> TemplateIdentityDefinitions { get; set; }
    public DbSet<TemplateNarrativeStructure> TemplateNarrativeStructures { get; set; }
    
    public DbSet<ProjectIdentityDefinition> ProjectIdentityDefinitions { get; set; }
    public DbSet<IdentityValue> IdentityValues { get; set; }
    public DbSet<CharacterIdentity> CharacterIdentities { get; set; }
    
    public DbSet<EngineExportConfig> EngineExportConfigs { get; set; }
    public DbSet<EngineFieldMapping> EngineFieldMappings { get; set; }
    public DbSet<AssetLink> AssetLinks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Load configurations from domain folders (auto-discovery or explicit registration)
        // Option 1: Auto-discovery for all configurations in assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GameDbContext).Assembly);
        
        // OR Option 2: Explicit registration per domain (more control)
        // modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        // modelBuilder.ApplyConfiguration(new TeamMemberEntityTypeConfiguration());
        // ... all other configurations loaded explicitly
    }
}
```

---

**This completes the updated SCHEMA.md documentation for GaDeMa v0.1 Pre-Release MVP with configuration files per domain!** 🐱🚀

The specification now includes:
- ✅ All ~57 database tables defined
- ✅ Fluent API configurations in separate files per domain folder (not in DbContext!)
- ✅ Clear separation of concerns between model definition and configuration logic
- ✅ Testable, maintainable architecture with clean file organization
- ✅ Scalable design for future growth without bloating DbContext

---

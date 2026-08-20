# 📄 **SCHEMA.md** - Complete Database Schema Documentation  
**GaDeMa – Game Development Management Application (v0.1 Pre-Release MVP)**  
**Status**: Production-Ready Architecture with Fluent API Configuration  

---

## **📋 Overview**

This document contains the complete database schema definition for GaDeMa v0.1, including:
- ✅ All 53 core table definitions
- ✅ Entity class structures with column types and attributes
- ✅ Fluent API configuration for relationships and indexes
- ✅ Navigation properties and cascade delete rules
- ✅ Foreign key constraints and naming conventions

**Total Tables**: ~53 | **Framework**: EF Core | **Database**: SQLite (MVP) / PostgreSQL (Production)

---

## **📁 File Structure Reference**

```bash
src/
├── GameDev.Core/Models/       # Entity classes (ContentItem.cs, User.cs, etc.)
├── GameDev.Data/              # DbContext + migrations config
└── docs/SCHEMA.md             # This documentation file
```

---

## **📂 Section 1: Base & Authentication Entities (3 tables)**

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
```

### **Fluent API Configuration**

```csharp
// GameDbContext.cs - OnModelCreating configuration
modelBuilder.Entity<User>(entity =>
{
    entity.HasKey(e => e.Id);
    entity.HasIndex(e => e.UserName).IsUnique();
    entity.HasIndex(e => e.Email).IsUnique();
    
    entity.Property(e => e.Id).ValueGeneratedOnAdd();
    entity.Property(e => e.UserName).IsRequired().HasMaxLength(256);
    entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
});

modelBuilder.Entity<Team>(entity =>
{
    entity.HasKey(e => e.Id);
    
    entity.HasIndex(e => e.Slug).IsUnique();
    
    entity.Property(e => e.Name).IsRequired();
    entity.Property(e => e.Description).HasMaxLength(4096);
});

modelBuilder.Entity<TeamMember>(entity =>
{
    entity.HasKey(e => e.Id);
    
    // Foreign key constraints with cascade behavior
    entity.HasOne(tm => tm.Team)
        .WithMany(t => t.TeamMembers)
        .HasForeignKey(tm => tm.TeamId)
        .OnDelete(DeleteBehavior.Cascade);
    
    entity.HasOne(tm => tm.User)
        .WithMany(u => u.TeamMemberships)
        .HasForeignKey(tm => tm.UserId)
        .OnDelete(DeleteBehavior.Restrict);  // Prevent orphaned team members
    
    entity.HasIndex(e => e.RoleId).IsUnique();
});
```

---

## **📂 Section 2: Content & Media Entities (9 tables)**

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
```

### **StoryOutline Entity**

```csharp
public class StoryOutline
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid SequenceId { get; set; }
    
    [MaxLength(4096)]
    public string Summary { get; set; } = "";
    
    [MaxLength(512)]
    public string? CharacterSnapshot { get; set; }
    
    [MaxLength(1024)]
    public string? ThemeStatement { get; set; }
    
    [EnumDataType(typeof(OutlineStatusEnum)), Required, Display(Name = "Outline Status")]
    public OutlineStatusEnum OutlineStatus { get; set; } = OutlineStatusEnum.DraftOutline;
}
```

### **DialogueBranch Entity**

```csharp
public class DialogueBranch
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }
    
    [MaxLength(128), Required, Display(Name = "Branch Title")]
    public string Title { get; set; } = "";
    
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    [MaxLength(1024)]
    public string? VisualNodeImageUri { get; set; }
    
    [MaxLength(512)]
    public string? CharacterIconUri { get; set; }
    
    public bool IsRoot { get; set; } = false;
    
    public Guid? ParentNodeId { get; set; }  // Self-referencing FK for tree structure
    
    public int OrderIndex { get; set; } = 0;
}
```

### **DialogueNode Entity**

```csharp
public class DialogueNode
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Branch ID")]
    public Guid BranchId { get; set; }
    
    [MaxLength(4096)]
    public string NodeText { get; set; } = "";
    
    public Guid? SpeakerId { get; set; }
    
    [MaxLength(4096)]
    public string? ChoiceOptions { get; set; }  // JSON array
    
    [MaxLength(4096)]
    public string? Conditions { get; set; }  // JSON conditions
}
```

### **ExternalReference Entity**

```csharp
public class ExternalReference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public int ParentType { get; set; }  // Enum: ContentItem(0), Task(1), Comment(2)
    public Guid? ParentId { get; set; }
    
    [Required, Display(Name = "External URL")]
    public string Url { get; set; } = "";
    
    [MaxLength(128)]
    public string Title { get; set; } = "";
    
    public int Type { get; set; }  // Enum: Document(0), Image(1), Video(2), Audio(3)
    
    public bool IsActive { get; set; } = true;
}
```

### **MediaAttachment Entity**

```csharp
public class MediaAttachment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Content Item ID")]
    public Guid ContentItemId { get; set; }
    
    [Required, Display(Name = "File Name")]
    public string FileName { get; set; } = "";
    
    [Display(Name = "Content Type")]
    public string ContentType { get; set; } = "application/octet-stream";
    
    [Column("storage_path"), MaxLength(2048)]
    public string StoragePath { get; set; } = "";
    
    [Display(Name = "File Size")]
    public long FileSize { get; set; }  // Bytes
    
    public Guid UploadedByUserId { get; set; }
    
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
```

### **Tag Entity**

```csharp
public class Tag
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Tag Name")]
    public string Name { get; set; } = "";
    
    [Column("slug"), MaxLength(128), Required]
    public string Slug { get; set; } = "";
    
    [MaxLength(128)]
    public string? Description { get; set; }
    
    [MaxLength(36)]
    public string? ColorHex { get; set; }
    
    public bool IsActive { get; set; } = true;
}
```

### **ContentTags Entity (Junction Table)**

```csharp
public class ContentTags
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Content Item ID")]
    public Guid ContentItemId { get; set; }
    
    [Required, Display(Name = "Tag ID")]
    public Guid TagId { get; set; }
    
    public int? OrderIndex { get; set; }  // Nullable for future enhancements
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

### **MediaTags Entity (Junction Table)**

```csharp
public class MediaTags
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Media Attachment ID")]
    public Guid MediaAttachmentId { get; set; }
    
    [Required, Display(Name = "Tag ID")]
    public Guid TagId { get; set; }
}
```

### **Fluent API Configuration for Content Entities**

```csharp
modelBuilder.Entity<ContentItem>(entity =>
{
    entity.HasKey(e => e.Id);
    
    // Indexes for frequently filtered columns
    entity.HasIndex(e => e.Slug).IsUnique();
    entity.HasIndex(e => e.ContentType);
    entity.HasIndex(e => e.Status);
    entity.HasIndex(e => e.Published);
    
    // Navigation property: MediaAttachments (Cascade delete)
    entity.HasMany(ci => ci.MediaAttachments)
          .WithOne(m => m.ContentItem)
          .HasForeignKey(m => m.ContentItemId)
          .OnDelete(DeleteBehavior.Cascade);
    
    // Navigation property: ContentTags (Junction Table - SetNull)
    entity.HasMany(ci => ci.ContentTags)
          .WithOne(ct => ct.ContentItem)
          .HasForeignKey(ct => ct.ContentItemId)
          .OnDelete(DeleteBehavior.SetNull);  // Keep tag entity alive
    
    // Navigation property: ReviewStatus (SetNull to preserve history)
    entity.HasOne(ci => ci.ReviewStatus)
          .WithMany()
          .HasForeignKey(rs => rs.ContentItemId)
          .OnDelete(DeleteBehavior.SetNull);
});

modelBuilder.Entity<StoryOutline>(entity =>
{
    entity.HasKey(e => e.Id);
    
    entity.HasIndex(e => e.SequenceId);
    
    entity.HasOne(s => s.StorySequence)
        .WithMany()
        .HasForeignKey(s => s.SequenceId)
        .OnDelete(DeleteBehavior.Cascade);
});

modelBuilder.Entity<DialogueBranch>(entity =>
{
    entity.HasKey(e => e.Id);
    
    // Self-referencing FK for tree structure
    entity.HasOne(db => db.Parent)
        .WithMany()
        .HasForeignKey(e => e.ParentNodeId)
        .OnDelete(DeleteBehavior.Restrict);
});

modelBuilder.Entity<ExternalReference>(entity =>
{
    entity.HasKey(e => e.Id);
    
    entity.HasIndex(e => e.ParentId);
    
    // Cascade delete parent reference when external resource is removed
    entity.HasOne(er => er.Parent)  // Navigation property for content item/task/comment
        .WithMany()
        .HasForeignKey(e => e.ParentId)
        .OnDelete(DeleteBehavior.Cascade);
});

modelBuilder.Entity<MediaAttachment>(entity =>
{
    entity.HasKey(e => e.Id);
    
    entity.HasIndex(e => e.ContentItemId);
    
    // Cascade delete content item removes all attachments
    entity.HasOne(m => m.ContentItem)
        .WithMany(ci => ci.MediaAttachments)
        .HasForeignKey(m => m.ContentItemId)
        .OnDelete(DeleteBehavior.Cascade);
});

modelBuilder.Entity<Tag>(entity =>
{
    entity.HasKey(e => e.Id);
    
    entity.HasIndex(e => e.Slug).IsUnique();
});

modelBuilder.Entity<ContentTags>(entity =>
{
    entity.HasKey(e => e.Id);
    
    // Junction table - no navigation property to parent for performance
    entity.HasIndex(e => e.ContentItemId)
        .HasDatabaseName("IX_ContentTags_ContentItem");
    
    entity.HasIndex(e => e.TagId)
        .HasDatabaseName("IX_ContentTags_Tag");
});
```

---

## **📂 Section 3: Narrative Structure Entities (8 tables)**

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
```

### **StoryBeat Entity**

```csharp
public class StoryBeat
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Sequence ID")]
    public Guid SequenceId { get; set; }
    
    [MaxLength(128), Required, Display(Name = "Beat Title")]
    public string BeatTitle { get; set; } = "";
    
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    public int OrderIndex { get; set; } = 0;
    
    public bool Published { get; set; } = false;
}
```

### **LoreEntry Entity**

```csharp
public class LoreEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }
    
    [EnumDataType(typeof(LoreTypeEnum)), Required]
    public LoreTypeEnum LoreType { get; set; }
    
    [MaxLength(128), Required]
    public string Title { get; set; } = "";
    
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    [MaxLength(4096)]
    public string? Content { get; set; }
    
    public bool Published { get; set; } = false;
}
```

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
```

### **CharacterBackground Entity (FK as PK - Child Entity)**

```csharp
public class CharacterBackground
{
    public Guid ContentItemId { get; set; }  // FK as Primary Key
    
    [MaxLength(4096)]
    public string? FullBiography { get; set; }
    
    [MaxLength(4096)]
    public string? PersonalityTraits { get; set; }  // JSON array
    
    [MaxLength(2048)]
    public string? Motivation { get; set; }
    
    [MaxLength(2048)]
    public string? Conflict { get; set; }
    
    [MaxLength(4096)]
    public string? VoiceNotes { get; set; }
    
    [MaxLength(4096)]
    public string? KeyEvents { get; set; }  // JSON array
    
    public bool Published { get; set; } = false;
    
    public int Version { get; set; } = 0;
}
```

---

## **📂 Section 4: Attributes & Scaling Entities (6 tables)**

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
```

### **AttributeDefinition Entity**

```csharp
public class AttributeDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public string Name { get; set; } = "";
    
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    public int ValueType { get; set; }  // Enum: Int, Float, Decimal
    
    [MaxLength(4096)]
    public string? FormulaExpression { get; set; }
    
    [MaxLength(1024)]
    public string? LevelMappingJson { get; set; }  // JSON for scaling
    
    public decimal? DefaultMin { get; set; }
    public decimal? DefaultMax { get; set; }
    
    public int DisplayOrder { get; set; } = 0;
}
```

### **ClassTemplate Entity**

```csharp
public class ClassTemplate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public string Name { get; set; } = "";
    
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    public Guid AttributeSetId { get; set; }
    
    public int BaseLevel { get; set; } = 1;
    public int? MaxLevel { get; set; }
}
```

### **ClassTemplateAttribute Entity**

```csharp
public class ClassTemplateAttribute
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ClassTemplateId { get; set; }
    
    [Required]
    public Guid AttributeDefinitionId { get; set; }
    
    [MaxLength(4096)]
    public string? OverrideFormulaExpression { get; set; }
    
    public decimal? DefaultMinValue { get; set; }
    public decimal? DefaultMaxValue { get; set; }
}
```

### **CharacterAttributes Entity**

```csharp
public class CharacterAttributes
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ContentItemId { get; set; }
    
    [Required]
    public Guid AttributeDefinitionId { get; set; }
    
    public decimal? CurrentValue { get; set; }
    
    public bool CalculatedFromTemplate { get; set; } = true;
    public bool OverridesFormula { get; set; } = false;
}
```

---

## **📂 Section 5: Abilities & GAS Entities (4 tables)**

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
```

### **AbilityDefinition Entity**

```csharp
public class AbilityDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public string Name { get; set; } = "";
    
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    public int AbilityType { get; set; }  // Enum: Attack, Defense, Buff, Debuff
    
    public decimal? CooldownSeconds { get; set; }
    public decimal? ResourceCost { get; set; }
    
    public int? MaxLevel { get; set; }
    
    [MaxLength(1024)]
    public string? ScalingFormulaJson { get; set; }  // JSON for scaling per level
    
    [MaxLength(512)]
    public string? RequiresFlagCondition { get; set; }
}
```

### **StatusEffectDefinition Entity**

```csharp
public class StatusEffectDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public string Name { get; set; } = "";
    
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    public int EffectType { get; set; }  // Enum: Buff, Debuff, Neutral
    
    public decimal? DurationSeconds { get; set; }
    public decimal? DamagePerTick { get; set; }
    
    [MaxLength(512)]
    public string? RequiresCondition { get; set; }
}
```

### **AssetLink Entity**

```csharp
public class AssetLink
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ContentItemId { get; set; }
    
    [MaxLength(2048)]
    public string EnginePath { get; set; } = "";
    
    [MaxLength(128)]
    public string? EngineAssetId { get; set; }  // Cross-reference ID
    
    [MaxLength(64)]
    public string EngineFileType { get; set; } = "fbx";  // e.g., .fbx, .uasset
}
```

---

## **📂 Section 6: Workflow & Task Entities (6 tables)**

### **Task Entity**

```csharp
public class Task
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
```

### **TaskComments Entity**

```csharp
public class TaskComments
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid TaskId { get; set; }
    
    [Required]
    public Guid CommentedByUserId { get; set; }
    
    [MaxLength(4096)]
    public string CommentText { get; set; } = "";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

### **Comment Entity (Content Comments)**

```csharp
public class Comment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ContentItemId { get; set; }
    
    [Required]
    public Guid CommentedByUserId { get; set; }
    
    [MaxLength(4096)]
    public string CommentText { get; set; } = "";
    
    [MaxLength(64)]
    public string? Visibility { get; set; }  // private, team-only, public
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

### **ContentVersionLog Entity**

```csharp
public class ContentVersionLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ContentItemId { get; set; }
    
    [Required]
    public Guid ChangedByUserId { get; set; }
    
    [MaxLength(2048)]
    public string? ChangeDescription { get; set; }
    
    public int VersionNumber { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

### **ReviewStatus Entity**

```csharp
public class ReviewStatus
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ContentItemId { get; set; }
    
    public int Status { get; set; }  // Enum: Pending, Approved, Rejected
    
    public Guid? ReviewedByUserId { get; set; }
    
    [MaxLength(4096)]
    public string? ReviewComments { get; set; }
    
    public DateTime? ReviewedAt { get; set; }
}
```

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
```

---

## **📂 Section 7: API Tokens & Automation Entities (2 tables)**

### **ProjectToken Entity**

```csharp
public class ProjectToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ProjectId { get; set; }
    
    [MaxLength(128), Required, Display(Name = "Token Name")]
    public string TokenName { get; set; } = "";
    
    // NOTE: In actual implementation, this should be hashed before storage
    [MaxLength(512)]
    public string TokenHash { get; set; } = "";
    
    public bool IsActive { get; set; } = true;
    
    public DateTime? ExpiresAt { get; set; }
    
    [MaxLength(2048)]
    public string PermissionsJson { get; set; }  // JSON array of permissions
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

### **TokenUsageLog Entity**

```csharp
public class TokenUsageLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ProjectTokenId { get; set; }
    
    [MaxLength(48)]
    public string? IPAddress { get; set; }  // Nullable for client IP
    
    [MaxLength(512)]
    public string? UserAgent { get; set; }
    
    public int Action { get; set; }  // Enum: Export, Read, Publish, etc.
    
    public Guid? ContentId { get; set; }  // Nullable content item accessed
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
```

---

## **📂 Section 8: Version Control & Inventory Entities (3 tables)**

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
```

### **InventoryItem Entity**

```csharp
public class InventoryItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ProjectId { get; set; }
    
    [MaxLength(128), Required]
    public string ItemName { get; set; } = "";
    
    public int ItemType { get; set; }  // Enum: Collectible, Key, Achievement, Currency
    
    public int CurrentValue { get; set; }
    
    public bool Published { get; set; } = false;
}
```

### **EndingDefinition Entity**

```csharp
public class EndingDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ProjectId { get; set; }
    
    [MaxLength(128), Required]
    public string EndingTitle { get; set; } = "";
    
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    [MaxLength(1024)]
    public string? ConditionsJson { get; set; }  // JSON conditions for triggering ending
    
    public bool Published { get; set; } = false;
}
```

---

## **📂 Section 9: Project Templates Entities (5 tables)**

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
```

### **TemplateAttributeSetDefinition Entity**

```csharp
public class TemplateAttributeSetDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ProjectTemplateId { get; set; }
    
    [Required]
    public Guid AttributeSetDefinitionId { get; set; }
}
```

### **TemplateClassTemplateDefinition Entity**

```csharp
public class TemplateClassTemplateDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ProjectTemplateId { get; set; }
    
    [MaxLength(128)]
    public string ClassTemplateName { get; set; } = "";
    
    public int BaseLevel { get; set; } = 1;
}
```

### **TemplateIdentityDefinition Entity**

```csharp
public class TemplateIdentityDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ProjectTemplateId { get; set; }
    
    [Required]
    public Guid IdentityDefinitionId { get; set; }
}
```

### **TemplateNarrativeStructure Entity**

```csharp
public class TemplateNarrativeStructure
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ProjectTemplateId { get; set; }
    
    [MaxLength(128)]
    public string SequenceName { get; set; } = "";
    
    public int OrderIndex { get; set; } = 0;
    
    public bool IsDefaultStructure { get; set; } = true;
}
```

---

## **📂 Section 10: Identity System Entities (3 tables)**

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
```

### **IdentityValue Entity**

```csharp
public class IdentityValue
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid IdentityDefinitionId { get; set; }
    
    [MaxLength(128)]
    public string Name { get; set; } = "";
    
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    public int OrderIndex { get; set; } = 0;
    
    public bool IsDefault { get; set; } = false;
}
```

### **CharacterIdentity Entity**

```csharp
public class CharacterIdentity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ContentItemId { get; set; }
    
    public Guid? IdentityDefinitionId { get; set; }  // Nullable if project doesn't require identity
    
    public Guid? IdentityValueId { get; set; }  // Nullable FK to IdentityValue selected for character
}
```

---

## **📂 Section 11: Engine Integration Entities (3 tables)**

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
```

### **EngineFieldMapping Entity**

```csharp
public class EngineFieldMapping
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid ProjectId { get; set; }
    
    [EnumDataType(typeof(ContentTypeEnum))]
    public ContentTypeEnum ContentType { get; set; }
    
    [MaxLength(128)]
    public string FieldName { get; set; } = "";  // Unique per project+type
    
    [MaxLength(256)]
    public string? EngineFieldName { get; set; }  // e.g., "Player.Health"
    
    public bool IsRequired { get; set; } = false;
    
    public int DataType { get; set; }  // Nullable: Int, Float, String, Boolean
}
```

### **AssetLink Entity** (Duplicate - see Section 5)

---

## **📂 Complete Fluent API Configuration for All Entities**

```csharp
// GameDbContext.cs - Complete OnModelCreating implementation
modelBuilder.Entity<User>(entity =>
{
    entity.HasKey(e => e.Id);
    entity.HasIndex(e => e.UserName).IsUnique();
    entity.HasIndex(e => e.Email).IsUnique();
    
    entity.Property(e => e.Id).ValueGeneratedOnAdd();
    entity.Property(e => e.UserName).IsRequired().HasMaxLength(256);
    entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
});

modelBuilder.Entity<Team>(entity =>
{
    entity.HasKey(e => e.Id);
    
    entity.HasIndex(e => e.Slug).IsUnique();
    
    entity.Property(e => e.Name).IsRequired();
    entity.Property(e => e.Description).HasMaxLength(4096);
});

modelBuilder.Entity<TeamMember>(entity =>
{
    entity.HasKey(e => e.Id);
    
    // Foreign key constraints with cascade behavior
    entity.HasOne(tm => tm.Team)
        .WithMany(t => t.TeamMembers)
        .HasForeignKey(tm => tm.TeamId)
        .OnDelete(DeleteBehavior.Cascade);
    
    entity.HasOne(tm => tm.User)
        .WithMany(u => u.TeamMemberships)
        .HasForeignKey(tm => tm.UserId)
        .OnDelete(DeleteBehavior.Restrict);  // Prevent orphaned team members
    
    entity.HasIndex(e => e.RoleId).IsUnique();
});

modelBuilder.Entity<ContentItem>(entity =>
{
    entity.HasKey(e => e.Id);
    
    // Indexes for frequently filtered columns
    entity.HasIndex(e => e.Slug).IsUnique();
    entity.HasIndex(e => e.ContentType);
    entity.HasIndex(e => e.Status);
    entity.HasIndex(e => e.Published);
    
    // Navigation property: MediaAttachments (Cascade delete)
    entity.HasMany(ci => ci.MediaAttachments)
          .WithOne(m => m.ContentItem)
          .HasForeignKey(m => m.ContentItemId)
          .OnDelete(DeleteBehavior.Cascade);
    
    // Navigation property: ContentTags (Junction Table - SetNull)
    entity.HasMany(ci => ci.ContentTags)
          .WithOne(ct => ct.ContentItem)
          .HasForeignKey(ct => ct.ContentItemId)
          .OnDelete(DeleteBehavior.SetNull);  // Keep tag entity alive
    
    // Navigation property: ReviewStatus (SetNull to preserve history)
    entity.HasOne(ci => ci.ReviewStatus)
          .WithMany()
          .HasForeignKey(rs => rs.ContentItemId)
          .OnDelete(DeleteBehavior.SetNull);
});

// ... [Continue with all other entities from above sections]
```

---

## **📊 Summary Table: Complete ~53 Tables**

| Category | Tables Count | Key Features |
| :--- | :--- | :--- |
| **Base & Auth** | 3 | User, Team, TeamMember |
| **Content & Media** | 9 | ContentItem(+ViewMode), StoryOutline, DialogueBranch/Node, ExternalReference, MediaAttachment, Tag, ContentTags, MediaTags |
| **Narrative Structure** | 8 | StorySequence, StoryBeat, LoreEntry, CharacterDetails/Background |
| **Attributes & Scaling** | 6 | AttributeSet, AttributeDefinition, ClassTemplate, ClassTemplateAttribute, CharacterAttributes |
| **Abilities & GAS** | 4 | AbilitySet, AbilityDefinition, StatusEffectDefinition, AssetLink |
| **Workflow & Tasks** | 6 | Task, TaskComments, Comment, ContentVersionLog, ReviewStatus, ActivityLog |
| **API Tokens** | 2 | ProjectToken, TokenUsageLog |
| **Version Control** | 1 | ContentSnapshot |
| **Inventory & Endings** | 2 | InventoryItem, EndingDefinition |
| **Project Templates** | 5 | ProjectTemplate + 4 template definitions |
| **Identity Systems** | 3 | Project
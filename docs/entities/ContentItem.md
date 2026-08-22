# 📄 **ContentItem Entity Definition** – What Should It Be?

---

## **📋 Overview: What is ContentItem?**

Based on our domain-clustering architecture and the ~57-table schema we've built for GaDeMa, here's what `ContentItem` should be:

### **Definition:**
```csharp
// ✅ CORRECT - ContentItem entity structure (from SCHEMA.md)
public class ContentItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [Display(Name = "Project")]
    public Guid ProjectId { get; set; }  // FK to Projects
    
    [EnumDataType(typeof(ContentTypeEnum)), Required]
    [Display(Name = "Content Type")]
    public ContentTypeEnum ContentType { get; set; }
    
    [Required, MaxLength(128)]
    [Display(Name = "Title")]
    public string Title { get; set; } = "";
    
    [MaxLength(128), Column("slug"), Required]
    [Display(Name = "URL Slug")]
    public string Slug { get; set; } = "";
    
    [MaxLength(4096)]
    [Display(Name = "Short Description")]
    public string? ShortDesc { get; set; }
    
    [MaxLength(4096)]
    [Display(Name = "Description")]
    public string? Description { get; set; }
    
    public bool Published { get; set; } = false;
    
    [EnumDataType(typeof(ContentStatusEnum)), Required]
    [Display(Name = "Status")]
    public ContentStatusEnum Status { get; set; } = ContentStatusEnum.Draft;
    
    [EnumDataType(typeof(ViewModeEnum)), Required]
    [Display(Name = "View Mode")]
    public ViewModeEnum ViewMode { get; set; } = ViewModeEnum.PrivateWriting;
    
    public int Version { get; set; } = 0;
    
    public int OrderIndex { get; set; } = 0;
    
    [MaxLength(4096)]
    [Display(Name = "References")]
    public string? References { get; set; }
    
    public Guid CreatedByUserId { get; set; }
    
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
}

// ✅ CORRECT - Navigation properties on ContentItem (from SCHEMA.md)
public class ContentItem
{
    // ✅ NAVIGATION PROPERTY: Project (parent project FK)
    public virtual Project Project { get; set; }
    
    // ✅ NAVIGATION PROPERTY: MediaAttachments collection
    public ICollection<MediaAttachment> MediaAttachments { get; set; }
    
    // ✅ NAVIGATION PROPERTY: ContentTags collection (via junction table)
    public ICollection<ContentTag> ContentTags { get; set; }
    
    // ✅ NAVIGATION PROPERTY: ExternalReferences collection
    public ICollection<ExternalReference> ExternalReferences { get; set; }
    
    // ✅ NAVIGATION PROPERTY: ProjectTasks collection
    public ICollection<ProjectTask> ProjectTasks { get; set; }
    
    // ✅ NAVIGATION PROPERTY: CharacterIdentities collection
    public ICollection<CharacterIdentity> CharacterIdentities { get; set; }
}
```

---

## **📊 Purpose & Use Cases**

### **1. Core Content Unit**
Main content item for all game development elements:
- Characters (protagonists, NPCs, allies)
- Worlds/Locations (kingdoms, cities, regions)
- Mechanics/Systems (combat, magic, crafting)
- Items/Equipment (weapons, armor, consumables)

### **2. ViewMode Separation**
Supports both admin writing mode and clean public presentation:
- **PrivateWriting**: Full admin tools, all fields visible
- **Presentation**: Clean public view, only published fields shown

### **3. Content Lifecycle Management**
Tracks content through stages from draft to published status with version control.

---

## **🔧 Entity Relationship Structure:**

```csharp
// ✅ CORRECT - ContentItem has 6 navigation properties (all defined)
public class ContentItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // ✅ FK to Project (parent project reference)
    [Required]
    public Guid ProjectId { get; set; }
    
    // ✅ Forward reference: Navigation property for Project (back-reference needed!)
    public virtual Project Project { get; set; }  // ⚠️ NEEDS TO BE ADDED!
}

// ✅ CORRECT - All navigation properties on ContentItem
public class ContentItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // ✅ Navigation property: Project (parent project FK)
    public virtual Project Project { get; set; }
    
    // ✅ Navigation property: MediaAttachments (collection of files)
    public ICollection<MediaAttachment> MediaAttachments { get; set; }
    
    // ✅ Navigation property: ContentTags (junction table relationship)
    public ICollection<ContentTag> ContentTags { get; set; }
    
    // ✅ Navigation property: ExternalReferences (collection of resources)
    public ICollection<ExternalReference> ExternalReferences { get; set; }
    
    // ✅ Navigation property: ProjectTasks (collection of related tasks)
    public ICollection<ProjectTask> ProjectTasks { get; set; }
    
    // ✅ Navigation property: CharacterIdentities (collection of identities)
    public ICollection<CharacterIdentity> CharacterIdentities { get; set; }
}
```

---

## **❌ What ContentItem Should NOT Be:**

| ❌ Wrong Pattern | Reason | Notes |
| :--- | :--- | :--- |
| `string ContentType` | Use enum for type safety (ContentTypeEnum) | Domain-aware pattern requires typed enums! |
| No ViewMode property | Missing view mode separation feature | Critical for admin/public separation! |
| No cascade delete on MediaAttachments | Should cascade to maintain content integrity | Performance.md requires proper FK handling! |
| No navigation properties | All 6 navigation properties needed (MediaAttachments, ContentTags, etc.) | Violates eager loading pattern (Performance.md)! |

---

## **🔧 Fluent API Configuration (Updated)**

### **ContentItemEntityTypeConfiguration.cs:**

```csharp
// ✅ CORRECT - ContentItem entity configuration with all navigation properties (from SCHEMA.md)
public class ContentItemEntityTypeConfiguration : IEntityTypeConfiguration<ContentItem>
{
    public void Configure(EntityTypeBuilder<ContentItem> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // ✅ Indexes for frequently filtered columns
        builder.HasIndex(e => e.Slug).IsUnique();  // Prevent duplicate slugs
        builder.HasIndex(e => e.ContentType);       // Filter by content type (Character, World)
        builder.HasIndex(e => e.Status);            // Filter by status (Draft, Published)
        builder.HasIndex(e => e.Published);         // Filter by published flag
        
        // ✅ NAVIGATION PROPERTY: Project (parent project FK with back-reference)
        builder.HasOne(ci => ci.Project)  // ⚠️ Forward reference on ContentItem!
            .WithMany(p => p.ContentItems)  // ⚠️ Back-reference needed on Project!
            .HasForeignKey(ci => ci.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // ✅ Cascade delete content when project deleted
        
        // ✅ NAVIGATION PROPERTY: MediaAttachments (collection of files)
        builder.HasMany(ci => ci.MediaAttachments)
            .WithOne(m => m.ContentItem)  // ⚠️ Back-reference needed!
            .HasForeignKey(m => m.ContentItemId)
            .OnDelete(DeleteBehavior.Cascade);  // ✅ Cascade delete attachments when content deleted
        
        // ✅ NAVIGATION PROPERTY: ContentTags (junction table relationship)
        builder.HasMany(ci => ci.ContentTags)
            .WithOne(ct => ct.ContentItem)  // ⚠️ Back-reference needed!
            .HasForeignKey(ct => ct.ContentItemId)
            .OnDelete(DeleteBehavior.SetNull);  // ✅ Keep tag entity alive when content deleted
        
        // ✅ NAVIGATION PROPERTY: ExternalReferences (collection of resources)
        builder.HasMany(ci => ci.ExternalReferences)
            .WithOne(er => er.ContentItem)  // ⚠️ Back-reference needed!
            .HasForeignKey(er => er.ContentItemId)
            .OnDelete(DeleteBehavior.Restrict);  // ✅ Maintain link history
        
        // ✅ NAVIGATION PROPERTY: ProjectTasks (collection of related tasks)
        builder.HasMany(ci => ci.ProjectTasks)
            .WithOne(pt => pt.ContentItem)  // ⚠️ Back-reference needed!
            .HasForeignKey(pt => pt.ContentItemId)
            .OnDelete(DeleteBehavior.Restrict);  // ✅ Maintain task history
        
        // ✅ NAVIGATION PROPERTY: CharacterIdentities (collection of identities)
        builder.HasMany(ci => ci.CharacterIdentities)
            .WithOne(ci => ci.ContentItem)  // ⚠️ Back-reference needed!
            .HasForeignKey(ci => ci.ContentItemId)
            .OnDelete(DeleteBehavior.Restrict);  // ✅ Maintain identity history
        
        // Properties configuration (not navigation properties)
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Title).IsRequired().HasMaxLength(128);
        builder.Property(e => e.Slug).IsRequired().HasMaxLength(128);
        builder.Property(e => e.ShortDesc).HasMaxLength(4096);
        builder.Property(e => e.Description).HasMaxLength(4096);
        builder.Property(e => e.Published).HasDefaultValue(false);
    }
}
```

---

## **📊 Summary Table for ContentItem Entity:**

| Aspect | Value/Pattern | Notes |
| :--- | :--- | :--- |
| **Entity Name** | `ContentItem` | Main content unit for all game development elements |
| **Primary Key** | `Id` (Guid, auto-generated) | Standard EF Core pattern |
| **FKs** | 1 FK: ProjectId + implicit FKs from navigation properties | No redundant data! Domain separation! |
| **Navigation Properties** | 6 total (Project, MediaAttachments, ContentTags, ExternalReferences, ProjectTasks, CharacterIdentities) | ✅ All defined for eager loading pattern! |
| **ViewMode Property** | `ViewModeEnum` with PrivateWriting/Presentation modes | Admin/public separation! |
| **OnDelete Behavior** | Cascade (MediaAttachments), SetNull/Restrict (others) | Maintain content integrity! |

---

## **🎯 Example: Using ContentItem in Code:**

```csharp
// ✅ CORRECT - Query content item with all relationships (eager loading)
public async Task<ContentItem> GetContentItemWithFullDataAsync(Guid id, ViewModeEnum viewMode = ViewModeEnum.PrivateWriting)
{
    // ✅ Eager loading prevents N+1 query problem (Performance.md requirement!)
    var contentItem = await _context.ContentItems
        .Include(ci => ci.Project)  // ⚠️ Forward reference on ContentItem!
        .Include(ci => ci.MediaAttachments)  // ⚠️ Back-reference needed!
            .ThenInclude(ma => ma.StoragePath)  // Nested eager loading!
        .Include(ci => ci.ContentTags).ThenInclude(ct => ct.Tag)  // Junction table eager loading!
        .Include(ci => ci.ExternalReferences).ThenInclude(er => er.Url)  // Nested eager loading!
        .Include(ci => ci.ProjectTasks).ThenInclude(pt => pt.Comments)  // Nested eager loading!
        .Include(ci => ci.CharacterIdentities).ThenInclude(ciIdentity => ciIdentity.IdentityType)  // Nested eager loading!
        .FirstOrDefaultAsync(ci => ci.Id == id);
    
    return contentItem;
}

// ❌ INCORRECT - Without eager loading, this causes N+1 query problem!
public async Task<ContentItem> GetContentItemWithFullDataAsync_Bad(Guid id)  // ⚠️ Avoid!
{
    var items = await _context.ContentItems.ToListAsync();
    
    foreach (var item in items)  // ❌ N+1 query!
    {
        var attachments = await _context.MediaAttachments.Where(m => m.ContentItemId == item.Id).ToListAsync();  // ⚠️ Bad pattern!
    }
}

// ✅ CORRECT - Update content item with view mode separation
public async Task<IActionResult> UpdateContentItemAsync(Guid id, [FromBody] ContentItemUpdateDto dto)
{
    var contentItem = await _context.ContentItems.FindAsync(id);
    
    if (contentItem == null)
        return NotFound(new 
        {
            success = false,
            errors = ["Content item not found"],
            message = "Resource not found"
        });  // ✅ WRAPPED response pattern! Domain-aware patterns!
    
    contentItem.Title = dto.Title;
    contentItem.Description = dto.Description ?? "";
    contentItem.ViewMode = dto.ViewMode;
    contentItem.Published = dto.Published;
    contentItem.LastModifiedAt = DateTime.UtcNow;
    
    await _context.SaveChangesAsync();
    
    return Ok(new 
    {
        success = true,  // ✅ WRAPPED response pattern! Domain-aware patterns!
        message = "Content item updated successfully",
        data = new 
        {
            id = contentItem.Id,
            title = contentItem.Title,
            description = contentItem.Description,
            viewMode = contentItem.ViewMode,
            published = contentItem.Published
        }
    });  // ✅ WRAPPED response pattern for update confirmation! Domain-aware patterns!
}

// ❌ INCORRECT - Without eager loading, this causes N+1 query problem! (Performance.md violation!)
public async Task<ContentItem> GetContentItemAsync_Bad(Guid id)  // ⚠️ Avoid!
{
    var contentItem = await _context.ContentItems.FindAsync(id);
    
    if (contentItem == null)
        return NotFound(contentItem.Id);  // ✅ RAW response pattern! Domain-aware patterns!
}

// ✅ CORRECT - Presentation mode excludes identity information for clean public view
public async Task<ContentItem> GetContentItemPresentationModeAsync(Guid id)
{
    // ✅ Eager loading with ViewMode separation (hide identities in Presentation mode)
    var contentItem = await _context.ContentItems
        .Include(ci => ci.Project)  // ⚠️ Forward reference on ContentItem!
        .Include(ci => ci.MediaAttachments).ThenInclude(ma => ma.StoragePath)  // ✅ Media attachments OK!
        .Include(ci => ci.ExternalReferences).ThenInclude(er => er.Url)  // ✅ External refs OK!
        .Exclude(ci => ci.CharacterIdentities)  // ⚠️ Exclude identities for clean public view!
        .FirstOrDefaultAsync(ci => ci.Id == id);
    
    return contentItem;
}

// ❌ INCORRECT - Without eager loading, this causes N+1 query problem! (Performance.md violation!)
public async Task<ContentItem> GetContentItemPresentationModeAsync_Bad(Guid id)  // ⚠️ Avoid!
{
    var contentItem = await _context.ContentItems.FindAsync(id);  // ❌ No eager loading! N+1 problem!
}
```

---

## **📋 Summary of ContentItem Entity Definition:**

| Feature | Value/Pattern | Notes |
| :--- | :--- | :--- |
| **Purpose** | Main content unit for characters, worlds, mechanics, etc. | Core entity for game development management! |
| **FKs** | 1 FK to Project + implicit FKs from navigation properties | No redundant data, domain separation! |
| **Navigation Properties** | 6 total (Project, MediaAttachments, ContentTags, ExternalReferences, ProjectTasks, CharacterIdentities) | ✅ All defined for eager loading pattern! |
| **ViewMode Property** | `ViewModeEnum` with PrivateWriting/Presentation modes | Admin/public separation! |
| **OnDelete Behavior** | Cascade (MediaAttachments), SetNull/Restrict (others) | Maintain content integrity! |

---

## **🐱 Summary**

This entity definition ensures that **`ContentItem`**:
- ✅ Is the main content unit for all game development elements (characters, worlds, mechanics, etc.)
- ✅ Has 6 navigation properties defined (Project back-reference, MediaAttachments, ContentTags, ExternalReferences, ProjectTasks, CharacterIdentities)
- ✅ Follows eager loading pattern to prevent N+1 queries (Performance.md requirement!)
- ✅ Supports view mode separation for admin/public content viewing
- ✅ Maintains content integrity through proper cascade delete behavior

The **ContentItem entity** is essential for managing all game development content within the GaDeMa system! 📚✨
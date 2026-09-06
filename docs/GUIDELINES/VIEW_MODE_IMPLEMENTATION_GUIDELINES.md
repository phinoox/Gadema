# 👁️ View Mode Implementation Guidelines for GaDeMa v0.1

This document provides detailed implementation guidance on separating PrivateWriting vs Presentation view modes across the application.

## Overview

The **ViewMode** query parameter allows switching between:
- **PrivateWriting**: Full admin interface with all fields + version history
- **Presentation**: Clean public view showing only published fields

## Query Parameter Pattern

### Controller Action Signature

```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetMetaInfoAsync(
    Guid id, 
    [FromQuery] ViewModeEnum viewMode = ViewModeEnum.PrivateWriting)
{
    // ... implementation
}
```

**Default**: PrivateWriting (for admin editing mode)  
**Override**: Presentation (for public viewing)

---

## Response Field Differences

### PrivateWriting Mode Response

```json
{
  "id": "char-001",
  "title": "Geralt of Rivia",
  "description": "<p>Full description with all admin details...</p>",
  "shortDesc": "Main protagonist",
  "published": false,
  "viewMode": "PrivateWriting",
  "version": 3,
  "status": "Draft",
  "createdAt": "2024-03-15T10:00:00Z",
  "lastModifiedAt": "2024-03-16T14:30:00Z",
  "orderIndex": 0,
  "references": "Linked to external resources"
}
```

**Includes:**
- All fields (published + unpublished)
- Version history data
- Draft status indicators
- Full metadata

---

### Presentation Mode Response

```json
{
  "id": "char-001",
  "title": "Geralt of Rivia",
  "description": "<p>Clean published description...</p>",
  "shortDesc": "Main protagonist",
  "published": true,
  "viewMode": "Presentation",
  "slug": "geralt-of-rivia",
  "seoTitle": "Geralt of Rivia - Official Character Sheet"
}
```

**Includes:**
- Only published fields
- Clean layout (no admin details)
- SEO-friendly URLs
- Minimal metadata

---

## DTO Projection Patterns per View Mode

### PrivateWriting DTO Projection

```csharp
public class MetaInfoPrivateWritingDto
{
    public Guid Id { get; set; }
    
    [Required]
    public string Title { get; set; } = "";
    
    [MaxLength(4096)]
    public string Description { get; set; } = "";
    
    [MaxLength(128)]
    public string ShortDesc { get; set; } = "";
    
    public bool Published { get; set; }
    
    [EnumDataType(typeof(ViewModeEnum))]
    public ViewModeEnum ViewMode { get; set; }
    
    public int Version { get; set; }
    
    [EnumDataType(typeof(ContentStatusEnum))]
    public ContentStatusEnum Status { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? LastModifiedAt { get; set; }
    
    public int OrderIndex { get; set; }
}
```

---

### Presentation DTO Projection

```csharp
public class MetaInfoPresentationDto
{
    [Display(Name = "Content ID")]
    public Guid Id { get; set; }
    
    [Required]
    [Display(Name = "Title")]
    public string Title { get; set; } = "";
    
    [MaxLength(4096)]
    [Display(Name = "Description")]
    public string Description { get; set; } = "";
    
    [Display(Name = "Slug")]
    public string Slug { get; set; } = "";
}
```

---

## Controller Implementation Examples

### MetaInfoController Example

```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetMetaInfoAsync(
    Guid id, 
    [FromQuery] ViewModeEnum viewMode = ViewModeEnum.PrivateWriting)
{
    try
    {
        var MetaInfo = await _context.MetaInfos.FindAsync(id);
        
        if (MetaInfo == null)
            return NotFound(new 
            {
                success = false,
                errors = ["Content item not found"],
                message = "Resource not found"
            });
        
        // PrivateWriting mode: return full content + admin tools
        if (viewMode == ViewModeEnum.PrivateWriting)
        {
            var projection = new MetaInfoPrivateWritingDto
            {
                Id = MetaInfo.Id,
                Title = MetaInfo.Title,
                Description = MetaInfo.Description,
                ShortDesc = MetaInfo.ShortDesc,
                Published = MetaInfo.Published,
                ViewMode = ViewModeEnum.PrivateWriting,
                Version = MetaInfo.Version,
                Status = MetaInfo.Status,
                CreatedAt = MetaInfo.CreatedAt,
                LastModifiedAt = MetaInfo.LastModifiedAt
            };
            
            return Ok(projection);  // ✅ WRAPPED for admin tools data retrieval
        }
        
        // Presentation mode: return only published fields + clean layout
        var presentationDto = new MetaInfoPresentationDto
        {
            Id = MetaInfo.Id,
            Title = MetaInfo.Title,
            Description = MetaInfo.Description,
            Slug = MetaInfo.Slug
        };
        
        return Ok(presentationDto);  // ✅ RAW for simple data retrieval
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error retrieving content item: {Id}", id);
        return StatusCode(500, new 
        {
            success = false,
            errors = [ex.Message],
            message = "An error occurred while retrieving content item"
        });
    }
}
```

### ProjectsController Example

```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetProjectAsync(
    Guid id, 
    [FromQuery] ViewModeEnum viewMode = ViewModeEnum.PrivateWriting)
{
    var project = await _context.Projects.FindAsync(id);
    
    if (project == null)
        return NotFound();
    
    // PrivateWriting: full admin interface
    if (viewMode == ViewModeEnum.PrivateWriting)
    {
        return Ok(new 
        {
            id = project.Id,
            title = project.Title,
            slug = project.Slug,
            visibility = project.Visibility,
            status = project.Status,
            enableUserRegistration = project.EnableUserRegistration
        });
    }
    
    // Presentation: clean public view only
    return Ok(new 
    {
        id = project.Id,
        title = project.Title,
        slug = project.Slug,
        visibility = project.Visibility
    });
}
```

---

## Caching Strategy Difference

### Draft Content (PrivateWriting Mode)

```csharp
public async Task<List<StorySequence>> GetSequencesWithCacheAsync(Guid projectId, ViewModeEnum viewMode)
{
    // Only cache published content in Presentation mode
    if (viewMode == ViewModeEnum.Presentation && contentIsPublished)
    {
        return await _redisCache.GetOrCreateAsync($"sequences:{projectId}", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);  // Published: longer TTL
            
            var sequences = await _context.StorySequences
                .Where(s => s.ProjectId == projectId)
                .Include(s => s.OutlineSummary)
                .ToListAsync();
                
            return sequences;
        });
    }
    
    // Draft content or PrivateWriting mode: short TTL or no cache
    var sequences = await _context.StorySequences
        .Where(s => s.ProjectId == projectId)
        .Include(s => s.OutlineSummary)
        .ToListAsync();  // Always query DB for drafts
        
    return sequences;
}
```

### Published Content (Presentation Mode)

```csharp
public async Task<MetaInfo> GetPublishedContentAsync(Guid id, ViewModeEnum viewMode = ViewModeEnum.Presentation)
{
    var cacheKey = $"published-content:{id}";
    
    if (viewMode == ViewModeEnum.Presentation)
    {
        return await _redisCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);  // Published content: 1 hour TTL
            
            var MetaInfo = await _context.MetaInfos.FindAsync(id);
            
            if (MetaInfo != null && MetaInfo.Published)
            {
                return new MetaInfoPresentationDto
                {
                    Id = MetaInfo.Id,
                    Title = MetaInfo.Title,
                    Description = MetaInfo.Description,
                    Slug = MetaInfo.Slug
                };
            }
            
            return null;  // Not published or not found
        });
    }
    
    // PrivateWriting mode: no caching, query DB directly
    var MetaInfo = await _context.MetaInfos.FindAsync(id);
    return MetaInfo;  // Return full entity with all admin fields
}
```

---

## Blazor Page View Mode Separation

### Razor Component Example

```razor
@page "/projects/{projectId}/content/{contentId}"
@model MetaInfoPageModel

@{
    ViewData["Title"] = Model.MetaInfo.Title;
    var viewMode = Context.Request.Query[ViewModeEnum.ToString()] ?? ViewModeEnum.PrivateWriting;
}

@if (viewMode == ViewModeEnum.PrivateWriting)
{
    <div class="admin-panel">
        <button onclick="@() => viewModeService.AddEditButton()">✏️ Edit Content</button>
        <button onclick="@() => viewModeService.AddVersionControl()">📋 Version History</button>
        <button onclick="@() => viewModeService.AddProjectTaskManagement()">✅ Manage ProjectTasks</button>
    </div>
}

@if (viewMode == ViewModeEnum.Presentation)
{
    <div class="public-view">
        <h1>@Model.MetaInfo.Title</h1>
        <p class="published-description">@Model.MetaInfo.Description</p>
        <nav class="navigation-links">
            <a href="@Model.Slug">@Model.Slug</a>
        </nav>
    </div>
}

@if (viewMode == ViewModeEnum.Presentation && Model.MetaInfo.Published)
{
    <script>
        // SEO-friendly meta tags for public view mode
        document.title = `@Model.MetaInfo.Title - GaDeMa`;
        var metaDescription = document.querySelector('meta[name="description"]');
        if (metaDescription) {
            metaDescription.content = @Model.MetaInfo.Description;
        }
    </script>
}
```

---

## Summary: View Mode Implementation Checklist

| Feature | PrivateWriting | Presentation | Notes |
| :--- | :--- | :--- | :--- |
| **API Response** | Full fields + admin tools | Clean public view only | Use separate DTO projections |
| **Caching** | Short TTL or no cache | Long TTL (1 hour) | Only for published content |
| **Blazor UI** | Admin panel with all tools | Clean layout, SEO-optimized | Conditional rendering based on mode |
| **URL Structure** | `/content/{id}` | `/content/{slug}` | Presentation mode uses slug-based URLs |
| **Meta Tags** | Complete page metadata | Minimal, SEO-focused | Presentation mode uses clean titles/descriptions |

---

## Anti-Patterns to Avoid

| Anti-Pattern | Example | ✅ Correct Approach |
| :--- | :--- | :--- |
| **Missing View Mode Check** | Always returning full content regardless of viewMode | ❌ Don't do this! Check viewMode before response |
| **Using Same DTO for Both Modes** | One `MetaInfoResponseDto` for all views | ✅ Use separate DTOs: `PrivateWritingDto`, `PresentationDto` |
| **Caching Draft Content** | Caching unpublished content in Presentation mode | ❌ Never cache draft content! |
| **Admin Tools in Public View** | Showing version control buttons in Presentation mode | ✅ Admin tools only in PrivateWriting mode |

---

## Final Reminder

**Proper view mode separation ensures a clean user experience and optimal caching!**

- ✅ Always check the `viewMode` query parameter before building responses
- ✅ Use separate DTO projections for each view mode
- ✅ Cache only published content in Presentation mode (1 hour TTL)
- ✅ Keep admin tools visible only in PrivateWriting mode
```

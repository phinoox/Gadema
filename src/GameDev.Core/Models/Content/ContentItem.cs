// =============================================================================
using GameDev.Core.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Represents a content item (character, world, mechanic, etc.) in the game development system.
/// Supports polymorphic design with type-specific detail tables and view mode separation.
/// </summary>
public class ContentItem
{
    /// <summary>
    /// Unique identifier for the content item.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the project this content item belongs to.
    /// </summary>
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// Content type (Character, World, Mechanic, etc.).
    /// </summary>
    [EnumDataType(typeof(ContentTypeEnum)), Required, Display(Name = "Content Type")]
    public ContentTypeEnum ContentType { get; set; }
    
    /// <summary>
    /// Title of the content item.
    /// </summary>
    [MaxLength(128), Required, Display(Name = "Title")]
    public string Title { get; set; } = "";
    
    /// <summary>
    /// URL-friendly slug for the content item (unique).
    /// </summary>
    [MaxLength(128), Column("slug"), Required, Display(Name = "URL Slug")]
    public string Slug { get; set; } = "";
    
    /// <summary>
    /// Short description for search/filtering.
    /// </summary>
    [MaxLength(4096)]
    public string? ShortDesc { get; set; }
    
    /// <summary>
    /// Full description (Markdown/HTML).
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Indicates if the content item is published.
    /// </summary>
    public bool Published { get; set; } = false;
    
    /// <summary>
    /// Current status (Draft, InProgress, Published, Archived).
    /// </summary>
    [EnumDataType(typeof(ContentStatusEnum)), Required, Display(Name = "Status")]
    public ContentStatusEnum Status { get; set; } = ContentStatusEnum.Draft;
    
    /// <summary>
    /// View mode: PrivateWriting (admin) or Presentation (public).
    /// </summary>
    [EnumDataType(typeof(ViewModeEnum)), Required, Display(Name = "View Mode")]
    public ViewModeEnum ViewMode { get; set; } = ViewModeEnum.PrivateWriting;
    
    /// <summary>
    /// Current version number.
    /// </summary>
    public int Version { get; set; } = 0;
    
    /// <summary>
    /// Order index for sorting.
    /// </summary>
    public int OrderIndex { get; set; } = 0;
    
    /// <summary>
    /// External references (JSON string).
    /// </summary>
    [MaxLength(4096)]
    public string? References { get; set; }
    
    /// <summary>
    /// ID of the user who created this content item.
    /// </summary>
    public Guid CreatedByUserId { get; set; }
    
    /// <summary>
    /// Last modification timestamp.
    /// </summary>
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
// =============================================================================
using Gadema.Core.Enums;
using Gadema.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models;

/// <summary>
/// Represents a content item (character, world, mechanic, etc.) in the game development system.
/// Supports polymorphic design with type-specific detail tables and view mode separation.
/// </summary>
public class ContentItem
{
    /// <summary>
    /// Unique identifier for the content item.
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// ID of the project this content item belongs to.
    /// </summary>
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }

    // Navigation property: Project (Many-to-One)
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }

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

    // Collection navigation properties
    /// <summary>
    /// Navigation property: Collection of version logs for this content item.
    /// Enables lazy loading to track all historical changes.
    /// Foreign key: ContentItemId (matches FK in ContentVersionLog)
    /// </summary>
    public virtual ICollection<ContentVersionLog> VersionLogs { get; set; } = new List<ContentVersionLog>();

    /// <summary>
    /// Navigation property: Collection of asset links for this content item.
    /// Enables lazy loading to track all engine asset connections.
    /// Foreign key: ContentItemId (matches FK in AssetLink)
    /// </summary>
    public virtual ICollection<AssetLink> AssetLinks { get; set; } = new List<AssetLink>();

    /// <summary>
    /// Navigation property: Collection of media attachments for this content item.
    /// Enables lazy loading to access all attached files/images.
    /// Foreign key: ContentItemId (matches FK in MediaAttachment)
    /// </summary>
    public virtual ICollection<MediaAttachment> MediaAttachments { get; set; } = new List<MediaAttachment>();

    /// <summary>
    /// Navigation property: Collection of comments for this content item.
    /// Enables lazy loading to access all comments.
    /// Foreign key: ContentItemId (matches FK in Comment)
    /// </summary>
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    // Reference navigation properties (Many-to-One relationships)
    /// <summary>
    /// Navigation property: Review status for this content item (Many-to-One).
    /// Foreign key: ContentItemId (matches FK in ReviewStatus)
    /// </summary>
    [Fixture(FixtureHintEnum.Omit)]
    public virtual ReviewStatus? ReviewStatus { get; set; }

    /// <summary>
    /// Navigation property: Collection of tag associations for this content item.
    /// Enables lazy loading to access all tags associated with this content.
    /// Foreign key: ContentItemId (matches FK in ContentTags junction table)
    /// </summary>
    public virtual ICollection<ContentTags> ContentTagAssociations { get; set; } = new List<ContentTags>();
    
    /// <summary>
    /// FK to ContentItem.Id (for FK-as-PK pattern in junction tables).
    /// This is used by configuration files expecting FK-as-PK pattern.
    /// </summary>
    [Required, Display(Name = "Content Item ID")]
    public Guid ContentItemId { get; set; } = Guid.NewGuid(); // Initialize with Id after object creation

    /// <summary>
    /// Navigation property: Collection of usage logs for this project token.
    /// Enables lazy loading to track all API usage history.
    /// Foreign key: ProjectTokenId (matches FK in TokenUsageLog)
    /// </summary>
    public virtual ICollection<TokenUsageLog> UsageLogs { get; set; } = new List<TokenUsageLog>();


}




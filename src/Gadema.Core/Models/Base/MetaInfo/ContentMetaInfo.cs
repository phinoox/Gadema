using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gadema.Core.Models.Base.Enums;
using Gadema.Core.Models.Base.Projects;
using Gadema.Core.Models.Base.Infrastructure;
using Gadema.Core.Models.Base.MetaInfo;

namespace Gadema.Core.Models.Base.MetaInfo;

/// <summary>
/// Represents a content item (character, world, mechanic, etc.) in the game development system.
/// </summary>
[ModelDependency(typeof(Project))]
public class ContentMetaInfo : BaseMetaInfo
{
    // --- Identity (Moved to BaseMetaInfo) ---
    // Id, Title, Slug, CreatedAt, LastModifiedAt removed

    // --- Relationships & Domain Data ---
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }

    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }

    [EnumDataType(typeof(ContentTypeEnum)), Required, Display(Name = "Content Type")]
    public ContentTypeEnum ContentType { get; set; }
    
    [Required]
    public ContentStatusEnum Status { get; set; } = ContentStatusEnum.Draft;

    [Required]
    public ViewModeEnum ViewMode { get; set; } = ViewModeEnum.PrivateWriting;

    public int Version { get; set; } = 0;
    public int OrderIndex { get; set; } = 0;

    [MaxLength(4096)]
    public string? References { get; set; }

    public Guid CreatedByUserId { get; set; }

    // Collection navigation properties
    public virtual ICollection<ContentVersionLog> VersionLogs { get; set; } = new List<ContentVersionLog>();
    public virtual ICollection<AssetLink> AssetLinks { get; set; } = new List<AssetLink>();
    public virtual ICollection<MediaAttachment> MediaAttachments { get; set; } = new List<MediaAttachment>();
    public virtual ReviewStatus ReviewStatus { get; set; }
    public virtual ICollection<ContentMetaInfoTagRelation> MetaInfoTagRelations { get; set; } = new List<ContentMetaInfoTagRelation>();
}
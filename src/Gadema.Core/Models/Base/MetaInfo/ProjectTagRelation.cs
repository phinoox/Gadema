namespace Gadema.Core.Models.Base.MetaInfo;

/// <summary>
/// Junction table linking a Project (Root Anchor) to a MetaTag.
/// </summary>
public class ProjectTagRelation
{
    [Required]
    public Guid ProjectMetaInfoId { get; set; }

    [Required]
    public Guid TagId { get; set; }

    // Navigation properties
    [ForeignKey("ProjectMetaInfoId")]
    public virtual ProjectMetaInfo ProjectMetaInfo { get; set; } = null!;

    [ForeignKey("TagId")]
    public virtual MetaTag Tag { get; set; } = null!;
}
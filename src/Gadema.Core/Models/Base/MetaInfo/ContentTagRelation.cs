namespace Gadema.Core.Models.Base.MetaInfo;

/// <summary>
/// Junction table linking a ContentMetaInfo (Content Anchor) to a MetaTag.
/// </summary>
public class ContentTagRelation
{
    [Required]
    public Guid MetaInfoId { get; set; }

    [Required]
    public Guid TagId { get; set; }

    // Navigation properties
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;

    [ForeignKey("TagId")]
    public virtual MetaTag Tag { get; set; } = null!;
}
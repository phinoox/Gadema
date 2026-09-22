using Gadema.Core.Models.Base.Projects;

namespace Gadema.Core.Models.Base.MetaInfo;

/// <summary>
/// Junction table linking a Project (Root Anchor) to a MetaTag.//ToDo: add dbsets for the different metainfo class
/// </summary>
[ModelDependency(typeof(MetaTag))]
public class TagRelation<T> where T: BaseMetaInfo
{
    [Required]
    public Guid MetaInfoId { get; set; }

    [Required]
    public Guid TagId { get; set; }

    // Navigation properties
    [ForeignKey("ProjectMetaInfoId")]
    public virtual T MetaInfo { get; set; } = null!;

    [ForeignKey("TagId")]
    public virtual MetaTag Tag { get; set; } = null!;
}
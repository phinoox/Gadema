namespace Gadema.Core.Models.Identity;

using System.ComponentModel.DataAnnotations;
using Gadema.Core.Models.Base.MetaInfo;
using Gadema.Core.Models.Base.Projects;

/// <summary>
/// Defines the identity of an ability or property.
/// Following Law I: This is a 1:1 extension of ContentMetaInfo, so Id == MetaInfoId.
/// </summary>
public class IdentityDefinition
{
    [Key]
    public Guid Id { get; set; } // Same as MetaInfoId

    [Required]
    public Guid MetaInfoId { get; set; }

    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;

    public Guid ProjectId { get; set; }

    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; } = null!;
    public IdentityDataTypeEnum DataType { get; set; }
    public bool IsRequired { get; set; }
    public bool IsActive { get; set; }
    
}

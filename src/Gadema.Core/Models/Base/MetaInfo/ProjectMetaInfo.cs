using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gadema.Core.Models.Base.Enums;
using Gadema.Core.Models.Base.Projects;

namespace Gadema.Core.Models.Base.MetaInfo;

/// <summary>
/// Holds the identity and discovery metadata for a Project.
/// Acts as the "Identity Card" for the root anchor.
/// </summary>
public class ProjectMetaInfo : BaseMetaInfo
{
    [Required]
    public ProjectStatusEnum Status { get; set; } = ProjectStatusEnum.Draft;
    
    [Required]
    public ViewModeEnum ViewMode { get; set; } = ViewModeEnum.PrivateWriting;

    // Link back to the Project anchor
    [Required]
    public Guid ProjectId { get; set; }

    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; } = null!;
}
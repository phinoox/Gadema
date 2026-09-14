using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gadema.Core.Enums;

namespace Gadema.Core.Models.Projects;

/// <summary>
/// Holds the identity and discovery metadata for a Project.
/// Acts as the "Identity Card" for the root anchor.
/// </summary>
public class ProjectMetaInfo
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(128)]
    public string Title { get; set; } = "";

    [Required, MaxLength(128), Column("slug")]
    public string Slug { get; set; } = "";

    [Required]
    public ProjectStatusEnum Status { get; set; } = ProjectStatusEnum.Draft;

    [Required]
    public ProjectVisibilityEnum Visibility { get; set; } = ProjectVisibilityEnum.Private;

    [Required]
    public ViewModeEnum ViewMode { get; set; } = ViewModeEnum.PrivateWriting;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Link back to the Project anchor
    [Required]
    public Guid ProjectId { get; set; }

    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; } = null!;
}
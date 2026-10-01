namespace Gadema.Core.Models.Access;

using System.ComponentModel.DataAnnotations;
using Gadema.Core.Models.Base.Projects;

/// <summary>
/// Provides access credentials for a specific project context.
/// Following Law I: This is a 1:1 extension, so Id == ProjectId.
/// </summary>
public class ProjectToken
{
    [Key]
    public Guid Id { get; set; } // Same as ProjectId

    [Required]
    public Guid ProjectId { get; set; }

    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; } = null!;
}

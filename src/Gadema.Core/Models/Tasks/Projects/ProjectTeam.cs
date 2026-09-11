// =============================================================================
using Gadema.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Projects;

/// <summary>
/// Represents a Team's access to a specific Project.
/// Stores the role the team has within this project.
/// </summary>
[ModelDependency(typeof(Project), typeof(Team))]
public class ProjectTeam
{
    /// <summary>
    /// Unique identifier for this membership.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// FK to the Project.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// FK to the Team.
    /// </summary>
    [Required]
    public Guid TeamId { get; set; }

    /// <summary>
    /// Role ID for this team within this project (e.g., Admin, Editor, Viewer).
    /// </summary>
    [Required]
    public int RoleId { get; set; }

    /// <summary>
    /// Navigation property: The Project.
    /// </summary>
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; } = null!;

    /// <summary>
    /// Navigation property: The Team.
    /// </summary>
    [ForeignKey("TeamId")]
    public virtual Team Team { get; set; } = null!;
}
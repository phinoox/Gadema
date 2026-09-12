// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Projects;

/// <summary>
/// DTO for assigning a team to a project.
/// </summary>
public class ProjectTeamUpdateDto
{
    /// <summary>
    /// ID of the project.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// ID of the team.
    /// </summary>
    [Required]
    public Guid TeamId { get; set; }

    /// <summary>
    /// Role ID for this team within this project (e.g., Admin, Editor, Viewer).
    /// </summary>
    [Required]
    public int RoleId { get; set; }
}

/// <summary>
/// Response DTO for a project-team relationship.
/// </summary>
public class ProjectTeamResponseDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectTitle { get; set; } = "";
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = "";
    public int RoleId { get; set; }
}

/// <summary>
/// List response for project teams.
/// </summary>
public class ProjectTeamListResponseDto
{
    public IEnumerable<ProjectTeamResponseDto> Items { get; set; } = Enumerable.Empty<ProjectTeamResponseDto>();
    public int TotalCount { get; set; }
}

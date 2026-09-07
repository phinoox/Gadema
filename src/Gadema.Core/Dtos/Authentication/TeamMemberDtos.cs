// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Authentication;

/// <summary>
/// DTO for adding a member to a team.
/// </summary>
public class TeamMemberCreateDto
{
    /// <summary>
    /// ID of the team this member belongs to.
    /// </summary>
    [Required]
    public Guid TeamId { get; set; }

    /// <summary>
    /// ID of the user who is a member of this team.
    /// </summary>
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Role ID: 0=Admin, 1=Editor, 2=Viewer.
    /// </summary>
    [Required]
    public int RoleId { get; set; }
}

/// <summary>
/// DTO for updating a team member's role.
/// </summary>
public class TeamMemberUpdateDto
{
    /// <summary>
    /// Role ID: 0=Admin, 1=Editor, 2=Viewer.
    /// </summary>
    [Required]
    public int RoleId { get; set; }
}

/// <summary>
/// Response DTO for a team member.
/// </summary>
public class TeamMemberResponseDto
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = "";
    public Guid UserId { get; set; }
    public string UserName { get; set; } = "";
    public string UserEmail { get; set; } = "";
    public int RoleId { get; set; }
    public DateTime JoinedAt { get; set; }
    public Guid? InvitedByUserId { get; set; }
    public bool IsPendingInvite { get; set; }
}

/// <summary>
/// List response for team members.
/// </summary>
public class TeamMemberListResponseDto
{
    public IEnumerable<TeamMemberResponseDto> Items { get; set; } = Enumerable.Empty<TeamMemberResponseDto>();
    public int TotalCount { get; set; }
}

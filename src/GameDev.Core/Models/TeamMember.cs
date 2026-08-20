// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Represents a team member with role-based access control.
/// Supports Admin, Editor, and Viewer roles.
/// </summary>
public class TeamMember
{
    /// <summary>
    /// Unique identifier for the team membership.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the team this member belongs to.
    /// </summary>
    public Guid TeamId { get; set; }
    
    /// <summary>
    /// ID of the user who is a member of this team.
    /// </summary>
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Role ID: 0=Admin, 1=Editor, 2=Viewer.
    /// </summary>
    public int RoleId { get; set; }
    
    /// <summary>
    /// Timestamp when the user joined the team.
    /// </summary>
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// ID of the user who sent the invitation.
    /// </summary>
    public Guid? InvitedByUserId { get; set; }
    
    /// <summary>
    /// Indicates if the invite is still pending acceptance.
    /// </summary>
    public bool IsPendingInvite { get; set; } = true;
}
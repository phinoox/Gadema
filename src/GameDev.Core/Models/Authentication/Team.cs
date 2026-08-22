// =============================================================================
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Represents a team organization in the GaDeMa system.
/// Teams can have multiple members with different roles.
/// </summary>
public class Team
{
    /// <summary>
    /// Unique identifier for the team.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Team name (unique).
    /// </summary>
    [Required, Display(Name = "Team Name")]
    public string Name { get; set; } = "";
    
    /// <summary>
    /// URL-friendly team slug (unique).
    /// </summary>
    [Required, MaxLength(256), Display(Name = "Team Slug")]
    public string Slug { get; set; } = "";
    
    /// <summary>
    /// Team description.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; } = null!;
    
    /// <summary>
    /// ID of the user who created the team.
    /// </summary>
    public Guid CreatedByUserId { get; set; }
    
    /// <summary>
    /// Team creation timestamp.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Indicates if the team is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Collection of team members for this team.
    /// Enables lazy loading to access all members in the team.
    /// Foreign key: TeamId (matches FK in TeamMember)
    /// </summary>
    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

    /// <summary>
    /// Navigation property: User who created this team (Restrict to preserve team history).
    /// Foreign key: CreatedByUserId (matches FK in configuration)
    /// </summary>
    public virtual User CreatedByUser { get; set; }

}
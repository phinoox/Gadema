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
}
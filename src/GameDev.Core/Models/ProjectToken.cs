// =============================================================================
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Represents a project-level API token for automation access.
/// Tokens are hashed before storage for security.
/// </summary>
public class ProjectToken
{
    /// <summary>
    /// Unique identifier for the project token.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the project this token belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// Name of the token (e.g., "CI/CD Pipeline").
    /// </summary>
    [MaxLength(128), Required, Display(Name = "Token Name")]
    public string TokenName { get; set; } = "";
    
    /// <summary>
    /// Hashed token value (SHA256 + salt).
    /// </summary>
    [MaxLength(512)]
    public string TokenHash { get; set; } = "";
    
    /// <summary>
    /// Indicates if the token is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Expiration timestamp (optional).
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
    
    /// <summary>
    /// Permissions JSON array.
    /// </summary>
    [MaxLength(2048)]
    public string PermissionsJson { get; set; }  // JSON array of permissions
    
    /// <summary>
    /// Timestamp when the token was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
// =============================================================================
using Gadema.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models;

/// <summary>
/// Represents a project-level API token for automation access.
/// Tokens are hashed before storage for security.
/// </summary>
[ModelDependency(typeof(Project))]
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
    /// Navigation property: Project that owns this token.
    /// Foreign key: ProjectId (matches FK in ProjectToken)
    /// </summary>
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }
    
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

    // <summary>
    /// The maximum number of requests allowed with this token.
    /// 0 indicates an unlimited quota.
    /// </summary>
    public int MaxRequests { get; set; } = 0;

    /// <summary>
    /// The number of requests already consumed by this token.
    /// </summary>
    public int CurrentUsage { get; set; } = 0;
    
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

    /// <summary>
    /// Navigation property: Collection of usage logs for this project token.
    /// Enables lazy loading to track all API usage history.
    /// Foreign key: ProjectTokenId (matches FK in TokenUsageLog)
    /// </summary>
    public virtual ICollection<TokenUsageLog> UsageLogs { get; set; } = new List<TokenUsageLog>();

    

}
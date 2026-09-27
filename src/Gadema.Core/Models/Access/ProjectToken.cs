// =============================================================================
using Gadema.Core.Models.Base.Projects;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Access;

/// <summary>
/// Represents a project-level API token for automation access.
/// Tokens are hashed before storage for security.
/// </summary>
[ModelDependency(typeof(Project))]
public class ProjectToken
{
    /// <summary>
    /// Gets or sets the unique identifier for the project token.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Gets or sets the unique identifier of the associated project.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the associated project entity.
    /// </summary>
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }
    
    /// <summary>
    /// Gets or sets a descriptive name for the token (e.g., "CI/CD Pipeline").
    /// </summary>
    [MaxLength(128), Required, Display(Name = "Token Name")]
    public string TokenName { get; set; } = "";
    
    /// <summary>
    /// Gets or sets the hashed token value (e.g., SHA256 + salt).
    /// </summary>
    [MaxLength(512)]
    public string TokenHash { get; set; } = "";

    /// <summary>
    /// Gets or sets the maximum number of requests allowed with this token.
    /// A value of 0 indicates an unlimited quota.
    /// </summary>
    public int MaxRequests { get; set; } = 0;

    /// <summary>
    /// Gets or sets the number of requests already consumed by this token.
    /// </summary>
    public int CurrentUsage { get; set; } = 0;
    
    /// <summary>
    /// Gets or sets a value indicating whether the token is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Gets or sets the expiration timestamp (optional).
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
    
    /// <summary>
    /// Gets or sets a JSON string representing the permissions granted to this token.
    /// </summary>
    [MaxLength(2048)]
    public string PermissionsJson { get; set; }  // JSON array of permissions
    
    /// <summary>
    /// Gets or sets the timestamp when the token was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Provides statistics regarding the usage and remaining quota of a project token.
/// </summary>
[DependencyIgnore]
public class TokenUsageStats
{
    /// <summary>
        /// Gets or sets the total number of requests allowed by the token.
    /// </summary>
    public int TotalUsage { get;  set; }

    /// <summary>
    /// Gets or sets the remaining number of requests available for this token.
    /// </summary>
    public int RemainingUsage { get;  set; }

    /// <summary>
    /// Gets or sets the expiration timestamp as an object.
    /// </summary>
    public object ExpiresAt { get;  set; }

    /// <summary>
    /// Gets or sets a value indicating whether the token has expired.
    /// </summary>
    public bool IsExpired { get;  set; }
}
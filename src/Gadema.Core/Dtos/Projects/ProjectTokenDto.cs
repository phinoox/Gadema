// =============================================================================
using System.ComponentModel.DataAnnotations;
// =============================================================================

namespace Gadema.Core.Dtos.Projects;

/// <summary>
/// DTO for project token creation.
/// </summary>
public class ProjectTokenDto
{
    /// <summary>
    /// Token name (e.g., "CI/CD Pipeline").
    /// </summary>
    [Required, MaxLength(128), Display(Name = "Token Name")]
    public string TokenName { get; set; } = "";
    
    /// <summary>
    /// Permissions JSON array (optional).
    /// </summary>
    [MaxLength(2048)]
    public string? PermissionsJson { get; set; } = null!;
    
    /// <summary>
    /// Expiration timestamp (optional).
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
}
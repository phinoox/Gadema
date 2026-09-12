// =============================================================================
using System.ComponentModel.DataAnnotations;
// =============================================================================

namespace Gadema.Core.Dtos.Projects;

/// <summary>
/// DTO for updating a project.
/// </summary>
public class UpdateProjectDto
{
    /// <summary>
    /// Visibility: 1=Private, 2=Public (optional).
    /// </summary>
    [Range(1, 2)]
    public int? Visibility { get; set; } = null!;
    
    /// <summary>
    /// Enable user registration (optional).
    /// </summary>
    public bool? EnableUserRegistration { get; set; } = null!;
    
    /// <summary>
    /// Allow manual invites (optional).
    /// </summary>
    public bool? AllowManualInvites { get; set; } = null!;
}
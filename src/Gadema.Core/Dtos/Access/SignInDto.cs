// =============================================================================
// =============================================================================

namespace Gadema.Core.Dtos.Access;

/// <summary>
/// Data transfer object for signing in with either email/password or an external provider ID.
/// </summary>
public class SignInDto
{
    /// <summary>
    /// The user's email address or the unique subject identifier from an external provider (e.g., Google).
    /// </summary>
    [Required, MaxLength(256), EmailAddress, Display(Name = "Email Address")]
    public string Email { get; set; } = "";
    
    /// <summary>
    /// The password for authentication. This is required for email-based login but may be null for external providers.
    /// </summary>
    [MaxLength(128)]
    public string? Password { get; set; } = null!;
   
}
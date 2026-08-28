// =============================================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
// =============================================================================

namespace Gadema.Core.Dtos.Authentication;

/// <summary>
/// DTO for two-factor authentication login.
/// </summary>
public class SignInWith2FADto
{
    /// <summary>
    /// User email address.
    /// </summary>
    [Required, MaxLength(256), EmailAddress, Display(Name = "Email Address")]
    public string Email { get; set; } = "";
    
    /// <summary>
    /// Two-factor authentication token (6-digit code).
    /// </summary>
    [Required, MaxLength(64)]
    public string TwoFactorToken { get; set; } = "";
}
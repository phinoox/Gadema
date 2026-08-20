// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Dtos.Authentication;

/// <summary>
/// DTO for traditional login (email/password).
/// </summary>
public class SignInDto
{
    /// <summary>
    /// Email address or Google Subject ID.
    /// </summary>
    [Required, MaxLength(256), EmailAddress, Display(Name = "Email Address")]
    public string Email { get; set; } = "";
    
    /// <summary>
    /// Password for traditional login (optional for Google OAuth).
    /// </summary>
    [MaxLength(128)]
    public string? Password { get; set; } = null!;
    
    /// <summary>
    /// Two-factor authentication token (optional).
    /// </summary>
    [MaxLength(64)]
    public string? TwoFactorToken { get; set; } = null!;
}
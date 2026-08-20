// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Dtos.Authentication;

/// <summary>
/// DTO for disabling two-factor authentication.
/// </summary>
public class Disable2FADto
{
    /// <summary>
    /// Current two-factor token for verification.
    /// </summary>
    [Required, MaxLength(64)]
    public string TwoFactorToken { get; set; } = "";
}
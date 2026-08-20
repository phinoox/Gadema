// =============================================================================
using System.ComponentModel.DataAnnotations;
// =============================================================================

namespace GameDev.Core.Dtos.Authentication;

/// <summary>
/// DTO for Google OAuth callback.
/// </summary>
public class GoogleCallbackDto
{
    /// <summary>
    /// OAuth authorization code from Google.
    /// </summary>
    [Required, MaxLength(512)]
    public string Code { get; set; } = "";
}
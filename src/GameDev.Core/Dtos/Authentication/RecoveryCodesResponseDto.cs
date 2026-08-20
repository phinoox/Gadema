// =============================================================================
// RecoveryCodesResponseDto - Response for recovery codes retrieval
// =============================================================================

namespace GameDev.Core.Dtos.Authentication;

/// <summary>
/// Recovery codes response for 2FA.
/// </summary>
public class RecoveryCodesResponseDto
{
    public IEnumerable<string> Codes { get; set; } = Enumerable.Empty<string>();
}

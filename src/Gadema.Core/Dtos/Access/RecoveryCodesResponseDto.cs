// =============================================================================
// RecoveryCodesResponseDto - Response for recovery codes retrieval
// =============================================================================

namespace Gadema.Core.Dtos.Access;

/// <summary>
/// Recovery codes response for 2FA.
/// </summary>
public class RecoveryCodesResponseDto
{
    public IEnumerable<string> Codes { get; set; } = Enumerable.Empty<string>();
    public string Message { get; set; }
}

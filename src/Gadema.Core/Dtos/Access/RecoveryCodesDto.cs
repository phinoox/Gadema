// =============================================================================
using System.ComponentModel.DataAnnotations;
// =============================================================================

namespace Gadema.Core.Dtos.Access;

/// <summary>
/// DTO for viewing recovery codes.
/// </summary>
public class RecoveryCodesDto
{
    /// <summary>
    /// Format of recovery codes: "csv" (default) or "text".
    /// </summary>
    [MaxLength(16)]
    public string? Format { get; set; } = "csv";
    public Guid? UserId { get; set; }
}
// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Dtos.Reviews;

/// <summary>
/// DTO for approving/rejecting a content item.
/// </summary>
public class ApproveContentDto
{
    /// <summary>
    /// Review status: 0=Pending, 1=Approved, 2=Rejected (required).
    /// </summary>
    [Required, Range(0, 2)]
    public int Status { get; set; }
    
    /// <summary>
    /// Review comments (required if rejecting).
    /// </summary>
    [MaxLength(4096)]
    public string? ReviewComments { get; set; } = null!;
}
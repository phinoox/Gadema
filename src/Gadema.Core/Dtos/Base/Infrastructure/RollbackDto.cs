// =============================================================================

namespace Gadema.Core.Dtos.Base.Infrastructure;

/// <summary>
/// Data transfer object used to request a rollback of a content item's state.
/// </summary>
public class RollbackDto
{
    /// <summary>
    /// The specific version number (from a snapshot) that the content should be reverted to.
    /// </summary>
    [Required]
    public int TargetVersion { get; set; }
}
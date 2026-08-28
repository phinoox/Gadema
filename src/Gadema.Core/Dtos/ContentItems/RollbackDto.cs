// =============================================================================
using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.ContentItems;

/// <summary>
/// DTO for rolling back a content item to a previous version.
/// </summary>
public class RollbackDto
{
    /// <summary>
    /// Target snapshot version to rollback to (required).
    /// </summary>
    [Required]
    public int TargetVersion { get; set; }
}
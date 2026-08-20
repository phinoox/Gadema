// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Dtos.Tags;

/// <summary>
/// DTO for adding tags to a content item.
/// </summary>
public class AddTagsDto
{
    /// <summary>
    /// List of tag IDs to add (required).
    /// </summary>
    [Required]
    public ICollection<Guid> TagIds { get; set; } = new List<Guid>();
}
// =============================================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
// =============================================================================

namespace Gadema.Core.Dtos.Tags;

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
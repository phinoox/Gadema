// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Characters;

/// <summary>
/// DTO for creating a character background.
/// </summary>
public class CharacterBackgroundCreateDto
{
    /// <summary>
    /// ID of the content item this background belongs to.
    /// </summary>
    [Required]
    public Guid MetaInfoId { get; set; }

    /// <summary>
    /// ID of the character details this background is associated with.
    /// </summary>
    [Required]
    public Guid CharacterDetailsId { get; set; }

    /// <summary>
    /// Description of the background.
    /// </summary>
    [MaxLength(2048)]
    public string? Description { get; set; }
}

/// <summary>
/// Response DTO for character background.
/// </summary>
public class CharacterBackgroundResponseDto
{
    public Guid Id { get; set; }
    public Guid MetaInfoId { get; set; }
    public Guid CharacterDetailsId { get; set; }
    public string? Description { get; set; }
    public bool Published { get; set; }
}

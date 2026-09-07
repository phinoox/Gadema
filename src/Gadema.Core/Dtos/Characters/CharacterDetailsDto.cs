// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Characters;

/// <summary>
/// DTO for creating/updating character details.
/// </summary>
public class CharacterDetailsDto
{
    /// <summary>
    /// FK to MetaInfo (links to the content item).
    /// </summary>
    [Required]
    public Guid MetaInfoId { get; set; }

    /// <summary>
    /// Character name.
    /// </summary>
    [Required, MaxLength(128)]
    public string Name { get; set; } = "";

    /// <summary>
    /// ID of the class template for this character.
    /// </summary>
    public Guid? ClassTemplateId { get; set; }

    /// <summary>
    /// Character level.
    /// </summary>
    public int Level { get; set; } = 1;

    /// <summary>
    /// Role (Protagonist, Antagonist, etc.).
    /// </summary>
    public int? Role { get; set; }

    /// <summary>
    /// Status (Alive, Deceased, Missing).
    /// </summary>
    public int? Status { get; set; }
}

/// <summary>
/// Response DTO for character details.
/// </summary>
public class CharacterDetailsResponseDto
{
    public Guid Id { get; set; }
    public Guid MetaInfoId { get; set; }
    public string Name { get; set; } = "";
    public Guid? ClassTemplateId { get; set; }
    public int Level { get; set; }
    public int? Role { get; set; }
    public int? Status { get; set; }
}

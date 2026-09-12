// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Characters;


public class CharacterRelationCreateDto
{
    [Required] public Guid SourceCharacterId { get; set; }

    [Required] public Guid TargetCharacterId { get; set; }

    [Required] public int RelationType { get; set; }

    [Required] public Guid TriggerSceneId { get; set; }

    [MaxLength(1024)] public string? Description { get; set; }
}

/// <summary>
/// DTO for creating a character relation.
/// </summary>
public class CharacterRelationUpdateDto
{
    /// <summary>
    /// ID of the source character.
    /// </summary>
    [Required]
    public Guid SourceCharacterId { get; set; }

    /// <summary>
    /// ID of the target character.
    /// </summary>
    [Required]
    public Guid TargetCharacterId { get; set; }

    /// <summary>
    /// Type of relationship (Ally, Enemy, Family, etc.).
    /// </summary>
    [Required]
    public int RelationType { get; set; }

    /// <summary>
    /// The scene where this relationship evolved or was first established.
    /// </summary>
    [Required]
    public Guid TriggerSceneId { get; set; }

    /// <summary>
    /// Optional description of the relationship at this point in the story.
    /// </summary>
    [MaxLength(1024)]
    public string? Description { get; set; }
}

/// <summary>
/// Response DTO for character relation.
/// </summary>
public class CharacterRelationResponseDto
{
    public Guid Id { get; set; }
    public Guid SourceCharacterId { get; set; }
    public string? SourceCharacterName { get; set; }
    public Guid TargetCharacterId { get; set; }
    public string? TargetCharacterName { get; set; }
    public int RelationType { get; set; }
    public Guid TriggerSceneId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// List response for character relations.
/// </summary>
public class CharacterRelationListResponseDto
{
    public IEnumerable<CharacterRelationResponseDto> Items { get; set; } = Enumerable.Empty<CharacterRelationResponseDto>();
    public int TotalCount { get; set; }
}

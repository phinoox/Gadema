using Gadema.Core.Models.Writing.Characters;


/// <summary>
/// Data transfer object for creating a new relationship between two characters.
/// </summary>
public class CharacterRelationCreateDto
{
    /// <summary>
    /// The unique identifier of the source character (the subject of the relation).
    /// </summary>
    [Required] public Guid SourceCharacterId { get; set; }

    /// <summary>
    /// The unique identifier of the target character (the object of the relation).
    /// </summary>
    [Required] public Guid TargetCharacterId { get; set; }

    /// <summary>
    /// The type of relationship (e.g., Ally, Enemy, Family).
    /// </summary>
    [Required] public RelationTypeEnum RelationType { get; set; }

    /// <summary>
    /// The unique identifier of the scene that triggered or established this relation.
    /// </summary>
    [Required] public Guid TriggerSceneId { get; set; }

    /// <summary>
    /// An optional description explaining the nature of the relationship.
    /// </summary>
    [MaxLength(1024)] public string? Description { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing character relation.
/// </summary>
public class CharacterRelationUpdateDto
{
    /// <summary>
    /// The unique identifier of the source character.
    /// </summary>
    [Required]
    public Guid SourceCharacterId { get; set; }

    /// <summary>
    /// The unique identifier of the target character.
    /// </summary>
    [Required]
    public Guid TargetCharacterId { get; set; }

    /// <summary>
    /// The type of relationship (e.g., Ally, Enemy).
    /// </summary>
    [Required]
    public RelationTypeEnum RelationType { get; set; }

    /// <summary>
    /// The scene where this relationship was established or evolved.
    /// </summary>
    [Required]
    public Guid TriggerSceneId { get; set; }

    /// <summary>
    /// An optional updated description of the relationship.
    /// </summary>
    [MaxLength(1024)] public string? Description { get; set; }
}

/// <summary>
/// Represents a character relation, denormalizing names for easy UI display.
/// </summary>
public class CharacterRelationResponseDto
{
    /// <summary>
    /// The unique identifier of the relation record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the source character.
    /// </summary>
    public Guid SourceCharacterId { get; set; }

    /// <summary>
    /// The display name of the source character for UI convenience.
    /// </summary>
    public string? SourceCharacterName { get; set; }

    /// <summary>
    /// The ID of the target character.
    /// </summary>
    public Guid TargetCharacterId { get; set; }

    /// <summary>
    /// The display name of the target character for UI convenience.
    /// </summary>
    public string? TargetCharacterName { get; set; }

    /// <summary>
    /// The type of relationship between the two characters.
    /// </summary>
    public RelationTypeEnum RelationType { get; set; }

    /// <summary>
    /// The unique identifier of the scene that triggered this relation.
    /// </summary>
    public Guid TriggerSceneId { get; set; }

    /// <summary>
    /// A description of the relationship.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The timestamp when the relation was created in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// A collection of character relations, typically used for paginated lists.
/// </summary>
public class CharacterRelationListResponseDto
{
    /// <summary>
    /// The list of retrieved character relations.
    /// </summary>
    public IEnumerable<CharacterRelationResponseDto> Items { get; set; } = Enumerable.Empty<CharacterRelationResponseDto>();

    /// <summary>
    /// Total number of relations found across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}

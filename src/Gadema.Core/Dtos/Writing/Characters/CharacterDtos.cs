/// <summary>
/// Data transfer object for creating a new character in the system.
/// </summary>
public class CharacterCreateDto
{
    /// <summary>
    /// The metadata required to establish the character's identity (e.g., Title, Slug).
    /// </summary>
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();

    /// <summary>
    /// The display name of the character.
    /// </summary>
    [Required, MaxLength(128)] public string Name { get; set; } = "";

    /// <summary>
    /// An optional nickname for the character.
    /// </summary>
    [MaxLength(256)] public string? NickName { get; set; }

    /// <summary>
    /// The unique identifier of the current state record to link upon creation.
    /// </summary>
    public Guid? CurrentStateId { get; set; }

    /// <summary>
    /// The unique identifier of the story profile to link upon creation.
    /// </summary>
    public Guid? StoryProfileId { get; set; }
}

/// <summary>
/// Data transfer object for updating a character's core properties and linkages.
/// </summary>
public class CharacterUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// The updated name of the character.
    /// </summary>
    [MaxLength(128)] public string? Name { get; set; }

    /// <summary>
    /// The updated nickname for the character.
    /// </summary>
    [MaxLength(256)] public string? NickName { get; set; }

    /// <summary>
    /// Updates the current state identifier for this character.
    /// </summary>
    public Guid? CurrentStateId { get; set; }

    /// <summary>
    /// Updates the story profile identifier for this character.
    /// </summary>
    public Guid? StoryProfileId { get; set; }

    /// <summary>
    /// The identity payload used by the sync strategy to update meta-information (e.g., Name, Slug).
    /// </summary>
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }
}

/// <summary>
/// Represents a character, denormalizing properties from its MetaInfo anchor and core domain component.
/// </summary>
public class CharacterResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// The display name of the character.
    /// </summary>
    [Required, MaxLength(128)] public string Name { get; set; } = "";

    /// <summary>
    /// An optional nickname for the character.
    /// </summary>
    [MaxLength(256)] public string? NickName { get; set; }

    /// <summary>
    /// The unique identifier of the current state record.
    /// </summary>
    public Guid? CurrentStateId { get; set; }

    /// <summary>
    /// The unique identifier of the linked story profile.
    /// </summary>
    public Guid? StoryProfileId { get; set; }
}

/// <summary>
/// A collection of characters, typically used for paginated lists.
/// </summary>
public class CharacterListResponseDto
{
    /// <summary>
    /// The list of retrieved characters.
    /// </summary>
    public IEnumerable<CharacterResponseDto> Items { get; set; } = Enumerable.Empty<CharacterResponseDto>();

    /// <summary>
    /// Total number of characters found across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}

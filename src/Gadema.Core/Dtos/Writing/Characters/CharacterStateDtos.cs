using Gadema.Core.Models.Writing.Characters;


/// <summary>
/// Data transfer object for creating a new character state (e.g., "Alive and Well", "Injured").
/// </summary>
public class CharacterStateCreateDto
{
    /// <summary>
    /// The metadata required to establish the state's identity.
    /// </summary>
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();

    /// <summary>
    /// A descriptive name for this state (e.g., "Injured").
    /// </summary>
    [Required, MaxLength(128)] public string StateName { get; set; } = "";

    /// <summary>
    /// An optional detailed description of the current state.
    /// </summary>
    [MaxLength(2048)] public string? Description { get; set; }

    /// <summary>
    /// A numeric value representing the magnitude or intensity of this state (e.g., health level).
    /// </summary>
    public double CurrentValue { get; set; } = 0;


    /// <summary>
    /// The character's current role in the story.
    /// </summary>
    [Required] public CharacterRole Role { get; set; } = CharacterRole.Neutral;

    /// <summary>
    /// The character's current faction affiliation.
    /// </summary>
    public Guid? FactionId { get; set; }

    /// <summary>
    /// The character's current location in the world.
    /// </summary>
    public Guid? LocationId { get; set; }

    /// <summary>
    /// The character's current life status (e.g., Alive, Dead).
    /// </summary>
    [Required] public CharacterStatus LifeStatus { get; set; } = CharacterStatus.Alive;

    /// <summary>
    /// An optional note providing context about this state change.
    /// </summary>
    [MaxLength(1024)] public string? Note { get; set; }

    /// <summary>
    /// The unique identifier of the scene that triggered this state transition.
    /// </summary>
    public Guid? TriggerSceneId { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing character state.
/// </summary>
public class CharacterStateUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// The metadata payload for the sync strategy.
    /// </summary>
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    /// <summary>
    /// The updated name of the state.
    /// </summary>
    public string? StateName { get; set; }

    /// <summary>
    /// The updated description of the state.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The updated numeric value for this state.
    /// </summary>
    public double? CurrentValue { get; set; }

    /// <summary>
    /// The updated role of the character.
    /// </summary>
    public CharacterRole? Role { get; set; }

    /// <summary>
    /// The updated faction identifier.
    /// </summary>
    public Guid? FactionId { get; set; }

    /// <summary>
    /// The updated location identifier.
    /// </summary>
    public Guid? LocationId { get; set; }

    /// <summary>
    /// The updated life status.
    /// </summary>
    public CharacterStatus? LifeStatus { get; set; }

    /// <summary>
    /// An updated note for the state.
    /// </summary>
    [MaxLength(1024)] public string? Note { get; set; }

    /// <summary>
    /// The updated scene identifier that triggered this update.
    /// </summary>
    public Guid? TriggerSceneId { get; set; }
}

/// <summary>
/// Represents a character's state at a specific moment in time, including denormalized context.
/// Inherits standard meta-information state.
/// </summary>
public class CharacterStateResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// The descriptive name of this state.
    /// </summary>
    [Required, MaxLength(128)] public string StateName { get; set; } = "";

    /// <summary>
    /// A detailed description of the state.
    /// </summary>
    [MaxLength(2048)] public string? Description { get; set; }

    /// <summary>
    /// The numeric intensity or level of this state.
    /// </summary>
    public double CurrentValue { get; set; }

    /// <summary>
    /// The character's current role in the story.
    /// </summary>
    public CharacterRole Role { get; set; }

    /// <summary>
    /// The unique identifier of the associated faction.
    /// </summary>
    public Guid? FactionId { get; set; }

    /// <summary>
    /// The display name of the associated faction for UI convenience.
    /// </summary>
    public string? FactionName { get; set; }  // Denormalized for convenience

    /// <summary>
    /// The unique identifier of the associated location.
    /// </summary>
    public Guid? LocationId { get; set; }

    /// <summary>
    /// The display name of the associated location for UI convenience.
    /// </summary>
    public string? LocationName { get; set; }  // Denormalized for convenience

    /// <summary>
    /// The character's current life status.
    /// </summary>
    public CharacterStatus LifeStatus { get; set; }

    /// <summary>
    /// An optional note regarding this state.
    /// </summary>
    [MaxLength(1024)] public string? Note { get; set; }

    /// <summary>
    /// The unique identifier of the scene that triggered this state.
    /// </summary>
    public Guid? TriggerSceneId { get; set; }
}

/// <summary>
/// A collection of character states, typically used for paginated lists.
/// </summary>
public class CharacterStateListResponseDto
{
    /// <summary>
    /// The list of retrieved character states.
    /// </summary>
    public IEnumerable<CharacterStateResponseDto> Items { get; set; } = Enumerable.Empty<CharacterStateResponseDto>();

    /// <summary>
    /// Total number of states found across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}
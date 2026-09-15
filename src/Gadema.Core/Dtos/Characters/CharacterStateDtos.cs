// src/Gadema.Core/Dtos/Characters/CharacterStateDtos.cs
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Models.Characters;

namespace Gadema.Core.Dtos.Characters;

/// <summary>
/// DTO for creating a character state.
/// </summary>
public class CharacterStateCreateDto
{
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();

    [Required, MaxLength(128)] public string StateName { get; set; } = "";
    [MaxLength(2048)] public string? Description { get; set; }
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
    /// The character's current life status.
    /// </summary>
    [Required] public CharacterStatus LifeStatus { get; set; } = CharacterStatus.Alive;

    /// <summary>
    /// Optional note about the current state.
    /// </summary>
    [MaxLength(1024)] public string? Note { get; set; }
    public Guid? TriggerSceneId { get; set; }
}

/// <summary>
/// DTO for updating a character state.
/// </summary>
public class CharacterStateUpdateDto : UpdateRequestDto
{
    public MetaInfoUpdateData? ContentMetaInfo { get; set; }

    public string? StateName { get; set; }
    public string? Description { get; set; }
    public double? CurrentValue { get; set; }

    public CharacterRole? Role { get; set; }
    public Guid? FactionId { get; set; }
    public Guid? LocationId { get; set; }
    public CharacterStatus? LifeStatus { get; set; }
    [MaxLength(1024)] public string? Note { get; set; }
    public Guid? TriggerSceneId { get; set; }
}

/// <summary>
/// Response DTO for a character state. Inherits ContentMetaInfo state.
/// </summary>
public class CharacterStateResponseDto : MetaInfoResponseBaseDto
{
    public CharacterRole Role { get; set; }
    public Guid? FactionId { get; set; }
    public string? FactionName { get; set; }  // Denormalized for convenience
    public Guid? LocationId { get; set; }
    public string? LocationName { get; set; }  // Denormalized for convenience
    public CharacterStatus LifeStatus { get; set; }
    public string? Note { get; set; }

    public string StateName { get; set; } = ""; // Added
    public string? Description { get; set; }   // Added
    public double CurrentValue { get; set; }    // Added
    public Guid? TriggerSceneId { get; set; }
}

/// <summary>
/// List response for character states.
/// </summary>
public class CharacterStateListResponseDto
{
    public IEnumerable<CharacterStateResponseDto> Items { get; set; } = Enumerable.Empty<CharacterStateResponseDto>();
    public int TotalCount { get; set; }
}
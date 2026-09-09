// src/Gadema.Core/Dtos/Characters/CharacterDtos.cs
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Models.Characters;

namespace Gadema.Core.Dtos.Characters;

/// <summary>
/// DTO for creating a character (glue entity).
/// </summary>
public class CharacterCreateDto
{
    /// <summary>
    /// MetaInfo data for the character's identity.
    /// </summary>
    [Required] public MetaInfoCreateData CreateData { get; set; } = new();
    
    /// <summary>
    /// Optional story profile data to create alongside the character.
    /// If not provided, create separately via CharacterStoryProfile endpoint.
    /// </summary>
    public CharacterStoryProfileCreateDto? StoryProfile { get; set; }
}

/// <summary>
/// DTO for updating a character's linkage (not MetaInfo or details themselves).
/// </summary>
public class CharacterUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// Mark a specific state as the current state.
    /// </summary>
    public Guid? CurrentStateId { get; set; }
    
    /// <summary>
    /// Link an existing story profile to this character.
    /// </summary>
    public Guid? StoryProfileId { get; set; }
}

/// <summary>
/// Response DTO for a character, including linked story profile and current state.
/// </summary>
public class CharacterResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// The linked story profile (static backstory/traits).
    /// </summary>
    public CharacterStoryProfileResponseDto? StoryProfile { get; set; }
    
    /// <summary>
    /// The current state ID.
    /// </summary>
    public Guid? CurrentStateId { get; set; }
    
    /// <summary>
    /// The current state data (role, faction, location, status).
    /// </summary>
    public CharacterStateResponseDto? CurrentStateData { get; set; }
}

/// <summary>
/// List response for characters.
/// </summary>
public class CharacterListResponseDto
{
    public IEnumerable<CharacterResponseDto> Items { get; set; } = Enumerable.Empty<CharacterResponseDto>();
    public int TotalCount { get; set; }
}

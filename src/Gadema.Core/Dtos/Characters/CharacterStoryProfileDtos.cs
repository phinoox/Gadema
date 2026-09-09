// src/Gadema.Core/Dtos/Characters/CharacterStoryProfileDtos.cs
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Models.Characters;

namespace Gadema.Core.Dtos.Characters;

/// <summary>
/// DTO for creating a character story profile.
/// </summary>
public class CharacterStoryProfileCreateDto
{
    /// <summary>
    /// The character this profile belongs to.
    /// </summary>
    [Required] public Guid CharacterId { get; set; }
    
    // === Origin & Background ===
    
    [MaxLength(4096)] public string? OriginStory { get; set; }
    [MaxLength(4096)] public string? FamilyBackground { get; set; }
    [MaxLength(8192)] public string? Backstory { get; set; }
    
    // === Personality & Traits ===
    
    [MaxLength(2048)] public string? PersonalityTraits { get; set; }
    [MaxLength(1024)] public string? Motivation { get; set; }
    [MaxLength(1024)] public string? Fear { get; set; }
    [MaxLength(2048)] public string? Beliefs { get; set; }
    [MaxLength(1024)] public string? SpeechPattern { get; set; }
    [MaxLength(2048)] public string? Quirks { get; set; }
    
    // === Narrative Arc ===
    
    public CharacterRole StoryRole { get; set; } = CharacterRole.Neutral;
    [MaxLength(256)] public string? ArcType { get; set; }
    [MaxLength(4096)] public string? ArcSummary { get; set; }
    [MaxLength(8192)] public string? KeyRelationships { get; set; }
}

/// <summary>
/// DTO for updating a character story profile.
/// </summary>
public class CharacterStoryProfileUpdateDto : UpdateRequestDto
{
    // === Origin & Background ===
    
    [MaxLength(4096)] public string? OriginStory { get; set; }
    [MaxLength(4096)] public string? FamilyBackground { get; set; }
    [MaxLength(8192)] public string? Backstory { get; set; }
    
    // === Personality & Traits ===
    
    [MaxLength(2048)] public string? PersonalityTraits { get; set; }
    [MaxLength(1024)] public string? Motivation { get; set; }
    [MaxLength(1024)] public string? Fear { get; set; }
    [MaxLength(2048)] public string? Beliefs { get; set; }
    [MaxLength(1024)] public string? SpeechPattern { get; set; }
    [MaxLength(2048)] public string? Quirks { get; set; }
    
    // === Narrative Arc ===
    
    public CharacterRole? StoryRole { get; set; }
    [MaxLength(256)] public string? ArcType { get; set; }
    [MaxLength(4096)] public string? ArcSummary { get; set; }
    [MaxLength(8192)] public string? KeyRelationships { get; set; }
}

/// <summary>
/// Response DTO for a character story profile.
/// </summary>
public class CharacterStoryProfileResponseDto
{
    public Guid Id { get; set; }
    public Guid CharacterId { get; set; }
    
    // === Origin & Background ===
    public string? OriginStory { get; set; }
    public string? FamilyBackground { get; set; }
    public string? Backstory { get; set; }
    
    // === Personality & Traits ===
    public string? PersonalityTraits { get; set; }
    public string? Motivation { get; set; }
    public string? Fear { get; set; }
    public string? Beliefs { get; set; }
    public string? SpeechPattern { get; set; }
    public string? Quirks { get; set; }
    
    // === Narrative Arc ===
    public CharacterRole StoryRole { get; set; }
    public string? ArcType { get; set; }
    public string? ArcSummary { get; set; }
    public string? KeyRelationships { get; set; }
}

/// <summary>
/// List response for character story profiles.
/// </summary>
public class CharacterStoryProfileListResponseDto
{
    public IEnumerable<CharacterStoryProfileResponseDto> Items { get; set; } = Enumerable.Empty<CharacterStoryProfileResponseDto>();
    public int TotalCount { get; set; }
}

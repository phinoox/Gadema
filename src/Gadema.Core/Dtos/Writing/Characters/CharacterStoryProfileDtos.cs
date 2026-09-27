using Gadema.Core.Models.Writing.Characters;


/// <summary>
/// Data transfer object for creating a new character story profile.
/// </summary>
public class CharacterStoryProfileCreateDto
{
    // === Origin & Background ===
    
    /// <summary>
    /// A detailed account of the character's origins.
    /// </summary>
    [MaxLength(4096)] public string? OriginStory { get; set; }

    /// <summary>
    /// Information about the character's family and upbringing.
    /// </summary>
    [MaxLength(4096)] public string? FamilyBackground { get; set; }

    /// <summary>
    /// A comprehensive backstory of the character.
    /// </summary>
    [MaxLength(8192)] public string? Backstory { get; set; }
    
    // === Personality & Traits ===
    
    /// <summary>
    /// Key personality traits that define the character.
    /// </summary>
    [MaxLength(2048)] public string? PersonalityTraits { get; set; }

    /// <summary>
    /// The core driving force or motivation for the character.
    /// </summary>
    [MaxLength(1024)] public string? Motivation { get; set; }

    /// <summary>
    /// A primary fear that influences the character's decisions.
    /// </summary>
    [MaxLength(1024)] public string? Fear { get; set; }

    /// <summary>
    /// The fundamental beliefs held by the character.
    /// </summary>
    [MaxLength(2048)] public string? Beliefs { get; set; }

    /// <summary>
    /// Characteristics of how the character speaks (e.g., accent, slang).
    /// </summary>
    [MaxLength(1024)] public string? SpeechPattern { get; set; }

    /// <summary>
    /// Unique or unusual habits and mannerisms (quirks).
    /// </summary>
    [MaxLength(2048)] public string? Quirks { get; set; }
    
    // === Narrative Arc ===
    
    /// <summary>
    /// The intended narrative role for this character in the story.
    /// </summary>
    public CharacterRole StoryRole { get; set; } = CharacterRole.Neutral;

    /// <summary>
    /// The type of narrative arc (e.g., Hero's Journey, Tragic Fall).
    /// </summary>
    [MaxLength(256)] public string? ArcType { get; set; }

    /// <summary>
    /// A summary of the character's intended story progression.
    /// </summary>
    [MaxLength(4096)] public string? ArcSummary { get; set; }

    /// <summary>
    /// Key relationships that define the character's narrative path.
    /// </summary>
    [MaxLength(8192)] public string? KeyRelationships { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing character story profile.
/// </summary>
public class CharacterStoryProfileUpdateDto : UpdateRequestDto
{
    // === Origin & Background ===
    
    /// <summary>
    /// The updated origin story.
    /// </summary>
    [MaxLength(4096)] public string? OriginStory { get; set; }

    /// <summary>
    /// The updated family background.
    /// </summary>
    [MaxLength(4096)] public string? FamilyBackground { get; set; }

    /// <summary>
    /// The updated backstory.
    /// </summary>
    [MaxLength(8192)] public string? Backstory { get; set; }
    
    // === Personality & Traits ===
    
    /// <summary>
    /// The updated personality traits.
    /// </summary>
    [MaxLength(2048)] public string? PersonalityTraits { get; set; }

    /// <summary>
    /// The updated motivation.
    /// </summary>
    [MaxLength(1024)] public string? Motivation { get; set; }

    /// <summary>
    /// The updated fear.
    /// </summary>
    [MaxLength(1024)] public string? Fear { get; set; }

    /// <summary>
    /// The updated beliefs.
    /// </summary>
    [MaxLength(2048)] public string? Beliefs { get; set; }

    /// <summary>
    /// The updated speech pattern.
    /// </summary>
    [MaxLength(1024)] public string? SpeechPattern { get; set; }

    /// <summary>
    /// The updated quirks.
    /// </summary>
    [MaxLength(2048)] public string? Quirks { get; set; }
    
    // === Narrative Arc ===
    
    /// <summary>
    /// The updated story role.
    /// </summary>
    public CharacterRole? StoryRole { get; set; }

    /// <summary>
    /// The updated arc type.
    /// </summary>
    [MaxLength(256)] public string? ArcType { get; set; }

    /// <summary>
    /// The updated arc summary.
    /// </summary>
    [MaxLength(4096)] public string? ArcSummary { get; set; }

    /// <summary>
    /// The updated key relationships.
    /// </summary>
    [MaxLength(8192)] public string? KeyRelationships { get; set; }
}

/// <summary>
/// Represents a character's story profile, including origin, personality, and narrative arc.
/// </summary>
public class CharacterStoryProfileResponseDto
{
    /// <summary>
    /// The unique identifier of the story profile.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the character this profile belongs to.
    /// </summary>
    public Guid CharacterId { get; set; }
    
    // === Origin & Background ===
    /// <summary>
    /// Detailed origin story.
    /// </summary>
    public string? OriginStory { get; set; }

    /// <summary>
    /// Family background information.
    /// </summary>
    public string? FamilyBackground { get; set; }

    /// <summary>
    /// The character's complete backstory.
    /// </summary>
    public string? Backstory { get; set; }
    
    // === Personality & Traits ===
    /// <summary>
    /// Defined personality traits.
    /// </summary>
    public string? PersonalityTraits { get; set; }

    /// <summary>
    /// The character's core motivation.
    /// </summary>
    public string? Motivation { get; set; }

    /// <summary>
    /// A primary fear of the character.
    /// </summary>
    public string? Fear { get; set; }

    /// <summary>
    /// Core beliefs held by the character.
    /// </summary>
    public string? Beliefs { get; set; }

    /// <summary>
    /// The character's speech pattern.
    /// </summary>
    public string? SpeechPattern { get; set; }

    /// <summary>
    /// Unique quirks of the character.
    /// </summary>
    public string? Quirks { get; set; }
    
    // === Narrative Arc ===
    /// <summary>
    /// The intended narrative role (e.g., Protagonist, Antagonist).
    /// </summary>
    public CharacterRole StoryRole { get; set; }

    /// <summary>
    /// The type of narrative arc.
    /// </summary>
    public string? ArcType { get; set; }

    /// <summary>
    /// Summary of the character's planned arc.
    /// </summary>
    public string? ArcSummary { get; set; }

    /// <summary>
    /// Key relationships that define the character's journey.
    /// </summary>
    public string? KeyRelationships { get; set; }
}

/// <summary>
/// A collection of character story profiles, typically used for paginated lists.
/// </summary>
public class CharacterStoryProfileListResponseDto
{
    /// <summary>
    /// The list of retrieved character story profiles.
    /// </summary>
    public IEnumerable<CharacterStoryProfileResponseDto> Items { get; set; } = Enumerable.Empty<CharacterStoryProfileResponseDto>();

    /// <summary>
    /// Total number of profiles found across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}

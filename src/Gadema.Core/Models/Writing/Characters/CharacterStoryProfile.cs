namespace Gadema.Core.Models.Writing.Characters;

/// <summary>
/// Represents the permanent, static backstory and personality profile of a character.
/// This entity holds immutable traits that define who the character is fundamentally,
/// as opposed to their dynamic state which changes throughout the narrative.
/// </summary>
[ModelDependency(typeof(Character))]
public class CharacterStoryProfile
{
    /// <summary>
    /// Unique identifier for the character profile (matches the associated Character's Id).
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// The ID of the character this profile belongs to.
    /// </summary>
    [Required] public Guid CharacterId { get; set; }
    
    /// <summary>
    /// Navigation property for the associated character.
    /// </summary>
    [ForeignKey("CharacterId")]
    public virtual Character Character { get; set; } = null!;
    
    // === Origin & Background ===
    /// <summary>
    /// Details regarding the character's birth, upbringing, and early life.
    /// </summary>
    [MaxLength(4096)] public string? OriginStory { get; set; }
    
    /// <summary>
    /// Information about the character's family, social class, and heritage.
    /// </summary>
    [MaxLength(4096)] public string? FamilyBackground { get; set; }
    
    /// <summary>
    /// Significant life events that occurred before the main narrative began.
    /// </summary>
    [MaxLength(8192)] public string? Backstory { get; set; }
    
    // === Personality & Traits ===
    
    /// <summary>
    /// The core personality descriptors (e.g., brave, cynical, loyal).
    /// </summary>
    [MaxLength(2048)] public string? PersonalityTraits { get; set; }
    
    /// <summary>
        /// The character's primary driving force or ultimate goal.
    /// </summary>
    [MaxLength(1024)] public string? Motivation { get; set; }
    
    /// <summary>
    /// The character's deepest fears or anxieties.
    /// </summary>
    [MaxLength(1024)] public string? Fear { get; set; }
    
    /// <summary>
    /// The fundamental values, moral code, or philosophies the character follows.
    /// </summary>
    [MaxLength(2048)] public string? Beliefs { get; set; }
    
    /// <summary>
    /// Details on how the character speaks (accent, tone, common phrases).
    /// </summary>
    [MaxLength(1024)] public string? SpeechPattern { get; set; }
    
    /// <summary>
    /// Distinctive habits, mannerisms, or physical quirks.
    /// </summary>
    [MaxLength(2048)] public string? Quirks { get; set; }
    
    // === Narrative Arc ===
    
    /// <summary>
    /// The character's intended structural role in the overarching story (e.g., Protagonist, Antagonist).
    /// </summary>
    public CharacterRole StoryRole { get; set; } = CharacterRole.Neutral;
    /// <summary>
    /// The type of narrative journey the character is destined to undergo (e.g., Hero's Journey, Redemption).
    /// </summary>
    [MaxLength(256)] public string? ArcType { get; set; }
    
    /// <summary>
    /// A summary of how the character is expected to evolve throughout the story arc.
    /// </summary>
    [MaxLength(4096)] public string? ArcSummary { get; set; }
    
    /// <summary>
    /// General descriptions of key relationships that define the character's social landscape.
    /// </summary>
    [MaxLength(8192)] public string? KeyRelationships { get; set; }
}


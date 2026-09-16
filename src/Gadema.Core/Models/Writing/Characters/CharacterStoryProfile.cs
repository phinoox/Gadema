using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gadema.Core.Models.Base.MetaInfo;

namespace Gadema.Core.Models.Writing.Characters;

/// <summary>
/// Static backstory and personality information for a character.
/// One-to-one with Character — holds immutable traits that define who the character is,
/// not what they're doing at any given moment (that's CharacterState).
/// </summary>
[ModelDependency(typeof(Character))]
public class CharacterStoryProfile
{
    /// <summary>
    /// Primary key matches CharacterId (FK as PK pattern for one-to-one).
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Links to the Character this profile belongs to.
    /// </summary>
    [Required] public Guid CharacterId { get; set; }
    
    [ForeignKey("CharacterId")]
    public virtual Character Character { get; set; } = null!;
    
    // === Origin & Background ===
    
    /// <summary>
    /// Where and how the character was born/raised.
    /// </summary>
    [MaxLength(4096)] public string? OriginStory { get; set; }
    
    /// <summary>
    /// Family background, social class, upbringing details.
    /// </summary>
    [MaxLength(4096)] public string? FamilyBackground { get; set; }
    
    /// <summary>
    /// Key life events that shaped the character (before the story begins).
    /// </summary>
    [MaxLength(8192)] public string? Backstory { get; set; }
    
    // === Personality & Traits ===
    
    /// <summary>
    /// Core personality traits (e.g., brave, cynical, loyal, ambitious).
    /// </summary>
    [MaxLength(2048)] public string? PersonalityTraits { get; set; }
    
    /// <summary>
    /// What the character wants above all else.
    /// </summary>
    [MaxLength(1024)] public string? Motivation { get; set; }
    
    /// <summary>
    /// What the character fears most.
    /// </summary>
    [MaxLength(1024)] public string? Fear { get; set; }
    
    /// <summary>
    /// Core beliefs, values, or moral code.
    /// </summary>
    [MaxLength(2048)] public string? Beliefs { get; set; }
    
    /// <summary>
    /// How the character speaks, common phrases, accent, tone.
    /// </summary>
    [MaxLength(1024)] public string? SpeechPattern { get; set; }
    
    /// <summary>
    /// Habits, quirks, or distinctive mannerisms.
    /// </summary>
    [MaxLength(2048)] public string? Quirks { get; set; }
    
    // === Narrative Arc ===
    
    /// <summary>
    /// The character's role in the overarching story (Protagonist, Antagonist, etc.).
    /// This is static — their intended role, not their current state.
    /// </summary>
    public CharacterRole StoryRole { get; set; } = CharacterRole.Neutral;
    
    /// <summary>
    /// The character's arc type (Hero's Journey, Fall from Grace, Redemption, etc.).
    /// </summary>
    [MaxLength(256)] public string? ArcType { get; set; }
    
    /// <summary>
    /// Brief summary of how the character changes throughout the story.
    /// </summary>
    [MaxLength(4096)] public string? ArcSummary { get; set; }
    
    /// <summary>
    /// Key relationships and their nature (not scene-specific — general relationship descriptions).
    /// </summary>
    [MaxLength(8192)] public string? KeyRelationships { get; set; }
}

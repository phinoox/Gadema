using Gadema.Core.Models.Writing.Narrative;

namespace Gadema.Core.Models.Writing.Characters;

/// <summary>
/// Represents an evolving relationship between two characters within the narrative.
/// This is an event-driven junction entity that tracks how character connections change over time,
/// often triggered by specific narrative events in a scene.
/// </summary>
[ModelDependency(typeof(Character), typeof(Scene))]
public class CharacterRelation
{
    /// <summary>
    /// Unique identifier for this relationship record.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The ID of the source character (the primary subject of the relationship).
    /// </summary>
    [Required]
    public Guid SourceCharacterId { get; set; }

    /// <summary>
    /// Navigation property for the source character.
    /// </summary>
    // Navigation property: Source Character
    [ForeignKey("SourceCharacterId")]
    public virtual Character SourceCharacter { get; set; } = null!;

    /// <summary>
    /// The ID of the target character (the subject being related to).
    /// </summary>
    [Required]
    public Guid TargetCharacterId { get; set; }
    /// <summary>
    /// Navigation property for the target character.
    /// </summary>
    // Navigation property: Target Character
    [ForeignKey("TargetCharacterId")]
    public virtual Character TargetCharacter { get; set; } = null!;

    /// <summary>
    /// The nature of the relationship (e.g., Ally, Enemy, Family).
    /// </summary>
    [Required]
    public RelationTypeEnum RelationType { get; set; } = RelationTypeEnum.Alien;
    /// <summary>
    /// The ID of the scene where this relationship was established or underwent a significant change.
    /// </summary>
    [Required]
    public Guid TriggerSceneId { get; set; }
/// <summary>
    /// Navigation property for the trigger scene.
/// </summary>
    // Navigation property: Trigger Scene
    [ForeignKey("TriggerSceneId")]
    public virtual Scene TriggerScene { get; set; } = null!;

    /// <summary>
    /// An optional description or context for this specific relationship state.
    /// </summary>
    [MaxLength(1024)]
    public string? Description { get; set; }

    /// <summary>
    /// The timestamp when this relationship record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Defines the types of relationships that can exist between characters.
/// </summary>
public enum RelationTypeEnum
{
    /// <summary>No established relationship or connection.</summary>
    Alien = 0,
    
    /// <summary>A neutral contact or acquaintance without a formal bond.</summary>
    Acquaintance = 1,
    
    /// <summary>Characters who are cooperative or on the same side.</summary>
    Ally = 2,
    
    /// <summary>Characters with opposing interests or active hostility.</summary>
    Enemy = 3,
    
    /// <summary>A familial bond through blood, marriage, or adoption.</summary>
    Family = 4,
    
    /// <summary>A romantic or intimate relationship.</summary>
    Romantic = 5,
    
    /// <summary>A hierarchical relationship based on teaching or guidance.</summary>
    Mentor = 6,
    
    /// <summary>A connection based on work, trade, or professional roles.</summary>
    Professional = 7,
    
    /// <summary>A user-defined or non-standard relationship type.</summary>
    Custom = 8
}


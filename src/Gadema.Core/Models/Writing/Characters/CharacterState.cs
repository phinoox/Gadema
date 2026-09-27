using Gadema.Core.Models.Base.Projects;
using Gadema.Core.Models.Writing.Narrative;
using Gadema.Core.Models.Writing.WorldBuilding;

namespace Gadema.Core.Models.Writing.Characters;

/// <summary>
/// Represents a character's mutable state at a specific point in the narrative.
/// This entity tracks dynamic properties that can change as the story progresses,
/// such as role, faction affiliation, location, and life status.
/// A single character may have multiple <see cref="CharacterState"/> records to track their evolution over different arcs or chapters.
/// </summary>
[ModelDependency(typeof(Project), typeof(ContentMetaInfo), typeof(Character), typeof(Faction), typeof(WorldLocation))]
public class CharacterState
{
    /// <summary>
    /// Unique identifier for this character state record.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary>
    /// The ID of the associated ContentMetaInfo entity, used for identity anchoring.
    /// </summary>
    [Required] public Guid MetaInfoId { get; set; }
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;

    /// <summary>
    /// The character's narrative role in this state (e.g., Protagonist, Antagonist).
    /// </summary>
    [Required] public CharacterRole Role { get; set; } = CharacterRole.Neutral;
    /// <summary>
    /// The ID of the faction or organization the character belongs to in this state.
    /// </summary>
    public Guid? FactionId { get; set; }
    /// <summary>
    /// Navigation property for the character's current faction affiliation.
    /// </summary>
    [ForeignKey("FactionId")]
    public virtual Faction? Faction { get; set; }
    /// <summary>
    /// The ID of the world location where the character is situated in this state.
    /// </summary>
    public Guid? LocationId { get; set; }

    /// <summary>
    /// Navigation property for the character's current geographic location.
    /// </summary>
    [ForeignKey("LocationId")]
    public virtual WorldLocation? Location { get; set; }

    /// <summary>
    /// The character's life status in this state (e.g., Alive, Deceased).
    /// </summary>
    [Required]
     public CharacterStatus LifeStatus { get; set; } = CharacterStatus.Alive;
    /// <summary>
    /// An optional note providing context for this specific state (e.g., "Wounded in battle").
    /// </summary>
    [MaxLength(1024)] public string? Note { get; set; }
    /// <summary>
    /// The human-readable name of this specific narrative state (e.g., "The Great War Era").
    /// </summary>
    [Required, MaxLength(128)]
    public string StateName { get; set; } = "";
/// <summary>
    /// A detailed description of what this state implies for the character's journey.
/// </summary>
    [MaxLength(2048)]
    public string? Description { get; set; }
/// <summary>
    /// A numeric value representing a key attribute or power level in this specific state.
/// </summary>
    public double CurrentValue { get; set; } = 0;

    /// <summary>
    /// The ID of the scene where this state was established or last changed.
    /// </summary>
    public Guid? TriggerSceneId { get; set; }

    /// <summary>
    /// Navigation property for the trigger scene.
    /// </summary>
    [ForeignKey("TriggerSceneId")]
    public virtual Scene? TriggerScene { get; set; }

    /// <summary>
    /// The ID of the character this state belongs to.
    /// </summary>
    public Guid? CharacterId {get;set;}

    /// <summary>
    /// Navigation property for the associated character.
    /// </summary>
    [ForeignKey("CharacterId")]
    public virtual Character? Character { get; set; }

    /// <summary>
    /// The timestamp when this state record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Defines the narrative roles a character can occupy within the story.
/// </summary>
public enum CharacterRole
{
    /// <summary>A neutral or non-aligned entity.</summary>
    Neutral = 0,
    /// <summary>The primary driver of the narrative.</summary>
    Protagonist = 1,
    /// <summary>The primary force of opposition to the protagonist.</summary>
    Antagonist = 2,
    /// <summary>A character who provides guidance or teaching.</summary>
    Mentor = 3,
    /// <summary><typeparam name="T">A standard background character with limited impact.</typeparam></summary>
    SideCharacter = 4,
    /// <summary>A character that supports the protagonist's journey.</summary>
    Supporting = 5,
    /// <summary>A custom or non-standard role.</summary>
    Custom = 6
}

/// <summary>
/// Defines the life status of a character within a specific narrative state.
/// </summary>
public enum CharacterStatus
{
    /// <summary>The character is active and functioning in the story.</summary>
    Alive = 0,
    /// <summary>The character has died within the narrative.</summary>
    Deceased = 1,
    /// <summary>The character's status is unknown or they have disappeared.</summary>
    Missing = 2,
    /// <summary>The character is widely believed to be dead, though not confirmed.</summary>
    PresumedDead = 3,
    /// <summary>A custom life status.</summary>
    Custom = 4
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gadema.Core.Models.Base.MetaInfo;
using Gadema.Core.Models.Narrative;
using Gadema.Core.Models.Base.Projects;
using Gadema.Core.Models.Writing.WorldBuilding;
using Gadema.Core.Models.Writing;

namespace Gadema.Core.Models.Writing.Characters;

/// <summary>
/// Represents a character's mutable state at a point in the story.
/// Holds information that can change throughout the narrative: role, faction, location, status.
/// This is NOT the character's permanent identity — those live in Character and CharacterStoryProfile.
/// A character can have multiple CharacterState records over time (one per story arc/chapter).
/// </summary>
[ModelDependency(typeof(Project), typeof(ContentMetaInfo),typeof(Character), typeof(Faction), typeof(WorldLocation))]
public class CharacterState
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required] public Guid MetaInfoId { get; set; }
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;

    /// <summary>
    /// The character's current role in the story (Protagonist, Antagonist, Mentor, SideCharacter, etc.)
    /// This can change as the story progresses.
    /// </summary>
    [Required] public CharacterRole Role { get; set; } = CharacterRole.Neutral;

    /// <summary>
    /// The character's current faction or organization affiliation.
    /// Can be null if the character is unaffiliated.
    /// </summary>
    public Guid? FactionId { get; set; }

    [ForeignKey("FactionId")]
    public virtual Faction? Faction { get; set; }

    /// <summary>
    /// The character's current location in the world.
    /// Can be null if the character's whereabouts are unknown.
    /// </summary>
    public Guid? LocationId { get; set; }

    [ForeignKey("LocationId")]
    public virtual WorldLocation? Location { get; set; }

    /// <summary>
    /// The character's current life status (Alive, Deceased, Missing, etc.)
    /// This can change as the story progresses.
    /// </summary>
    [Required]
     public CharacterStatus LifeStatus { get; set; } = CharacterStatus.Alive;




    /// <summary>
    /// Optional note about the current state (e.g., "Wounded in battle", "In hiding").
    /// Provides context for why the character is in this particular state.
    /// </summary>
    [MaxLength(1024)] public string? Note { get; set; }


    
    /// <summary>
    /// The name of this specific state (e.g., "The Great War Era").
    /// </summary>
    [Required, MaxLength(128)]
    public string StateName { get; set; } = "";

    /// <summary>
    /// A description of what this state implies for the character.
    /// </summary>
    [MaxLength(2048)]
    public string? Description { get; set; }

    /// <summary>
    /// A numeric value representing a key attribute in this state (e.g., health, power level).
    /// </summary>
    public double CurrentValue { get; set; } = 0;

    
    /// <summary>
    /// The scene where this state was established or last changed.
    /// Links to scene for event-driven tracking of state changes.
    /// </summary>
    public Guid? TriggerSceneId { get; set; }
    
    [ForeignKey("TriggerSceneId")]
    public virtual Scene? TriggerScene { get; set; }
    

    public Guid? CharacterId {get;set;}

    /// <summary>
    /// Collection of Characters that reference this state (mostly CurrentState back-references).
    /// </summary>
    [ForeignKey("CharacterId")]
    public virtual Character? Character { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Enum for character roles in the story.
/// </summary>
public enum CharacterRole
{
    Neutral = 0,
    Protagonist = 1,
    Antagonist = 2,
    Mentor = 3,
    SideCharacter = 4,
    Supporting = 5,
    Custom = 6
}

/// <summary>
/// Enum for character life status.
/// </summary>
public enum CharacterStatus
{
    Alive = 0,
    Deceased = 1,
    Missing = 2,
    PresumedDead = 3,
    Custom = 4
}
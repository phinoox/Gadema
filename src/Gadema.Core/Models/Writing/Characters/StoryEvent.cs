// src/Gadema.Core/Models/Characters/StoryEvent.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Gadema.Core.Models.Content;
using Gadema.Core.Models.Narrative;

namespace Gadema.Core.Models.Characters;

/// <summary>
/// Generic event tracker for story changes. Links a Scene (the "when/context") to an actor MetaInfo
/// and a target entity that changed. Extensible via EventType discriminator.
/// Examples: CharacterStateChange, CharacterRelationChange, FactionShift, LocationMove, etc.
/// </summary>
[ModelDependency(typeof(Scene), typeof(MetaInfo))]
public class StoryEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// The scene where this event occurred (the "when/context").
    /// </summary>
    [Required] public Guid SceneId { get; set; }
    
    [ForeignKey("SceneId")]
    public virtual Scene Scene { get; set; } = null!;
    
    /// <summary>
    /// The actor/entity that was affected by this event (generic via MetaInfo).
    /// Could be a character, faction, location, or any MetaInfo-backed entity.
    /// </summary>
    [Required] public Guid ActorMetaInfoId { get; set; }
    
    [ForeignKey("ActorMetaInfoId")]
    public virtual MetaInfo ActorMetaInfo { get; set; } = null!;
    
    /// <summary>
    /// Event type discriminator for extensibility.
    /// Examples: CharacterStateChange, CharacterRelationChange, FactionShift, LocationMove, Custom
    /// </summary>
    [Required] public StoryEventType EventType { get; set; } = StoryEventType.Custom;
    
    /// <summary>
    /// The entity type that changed (e.g., "CharacterState", "CharacterRelation").
    /// Used for type-safe deserialization on the client side.
    /// </summary>
    [Required] public string TargetEntityTypeId { get; set; } = "";
    
    /// <summary>
    /// The ID of the specific entity that changed.
    /// Resolves to an instance of TargetEntityTypeId.
    /// </summary>
    [Required] public Guid TargetEntityId { get; set; }
    
    /// <summary>
    /// Optional description of what happened (e.g., "Became antagonist", "Moved to City X").
    /// </summary>
    [MaxLength(2048)] public string? Description { get; set; }
    
    /// <summary>
    /// When this event was recorded (typically when the scene was saved).
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Enum for story event types. Extensible for future change types.
/// </summary>
public enum StoryEventType
{
    /// <summary>
    /// Default/unspecified event type.
    /// </summary>
    Custom = 0,
    
    /// <summary>
    /// A character's state changed (role, faction, location, life status).
    /// TargetEntityTypeId: "CharacterState"
    /// </summary>
    CharacterStateChange = 1,
    
    /// <summary>
    /// A character relationship evolved.
    /// TargetEntityTypeId: "CharacterRelation"
    /// </summary>
    CharacterRelationChange = 2,
    
    /// <summary>
    /// A faction shifted its goals, ideology, or alliances.
    /// TargetEntityTypeId: "Faction"
    /// </summary>
    FactionShift = 3,
    
    /// <summary>
    /// A location changed (e.g., destroyed, discovered, renamed).
    /// TargetEntityTypeId: "WorldLocation"
    /// </summary>
    LocationChange = 4,
    
    /// <summary>
    /// A character's story profile was updated.
    /// TargetEntityTypeId: "CharacterStoryProfile"
    /// </summary>
    StoryProfileUpdate = 5
}

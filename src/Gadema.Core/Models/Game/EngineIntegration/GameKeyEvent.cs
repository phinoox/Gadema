using Gadema.Core.Models.Writing.Narrative;

namespace Gadema.Core.Models.Game.EngineIntegration;

    /// <summary>
/// Represents a game-related event triggered by a specific narrative segment.
/// This allows writers to embed interactive mechanics (like giving items or setting flags)
/// directly into the story flow via scene segments.
    /// </summary>
[ModelDependency(typeof(ContentMetaInfo), typeof(SceneSegment))]
public class GameKeyEvent
{
    /// <summary>
    /// Unique identifier for this game event.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The ID of the associated ContentMetaInfo entity, used for identity anchoring and searching.
    /// </summary>
    [Required] public Guid MetaInfoId { get; set; }

    /// <summary>
    /// Navigation property for the event's identity anchor.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;

    /// <summary>
    /// The ID of the scene segment that triggers this event.
    /// </summary>
    [Required] public Guid SceneSegmentId { get; set; }

    /// <summary>
    /// Navigation property for the triggering scene segment.
    /// </summary>
    [ForeignKey("SceneSegmentId")] public virtual SceneSegment SceneSegment { get; set; } = null!;

    /// <summary>
    /// The type of game mechanic to trigger (e.g., GiveItem, SetFlag).
    /// </summary>
    public GameTriggerType TriggerType { get; set; } = GameTriggerType.None;

    /// <summary>
    /// The ID of the target entity involved in the event (e.g., an ItemId or QuestId).
    /// </summary>
    [MaxLength(128)] public string TargetId { get; set; } = string.Empty;

    /// <summary>
    /// A JSON payload containing complex trigger data (e.g., quantities, boolean values).
    /// </summary>
    public string? Payload { get; set; }

    /// <summary>
    /// An optional description for debugging or previewing the event's purpose.
    /// </summary>
    [MaxLength(1024)] public string? Description { get; set; }
}

public enum GameTriggerType
/// <summary>
/// Defines the types of mechanical events that can be triggered within the narrative flow.
/// </summary>public enum GameTriggerType
{
    /// <summary>No trigger defined.</summary>
    None,
    /// <summary>Adds an item to a character's inventory.</summary>
    GiveItem,
    /// <summary>Removes an item from a character's inventory.</summary>
    RemoveItem,
    /// <summary>Sets a boolean flag in the project state.</summary>
    SetFlag,
    /// <summary>Clears/resets a previously set flag.</summary>
    ClearFlag,
    /// <summary>Initiates a combat encounter.</summary>
    TriggerCombat,
    /// <summary>Moves a character or entity to a new location.</summary>
    ChangeLocation,
    /// <summary>Plays a specific audio track or sound effect.</summary>
    PlayAudio,
    /// <summary>A user-defined custom trigger type.</summary>
    Custom
}
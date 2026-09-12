using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gadema.Core.Models.Writing;

namespace Gadema.Core.Models.Game;

[ModelDependency(typeof(MetaInfo), typeof(SceneSegment))]
public class GameKeyEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public Guid MetaInfoId { get; set; }
    [ForeignKey("MetaInfoId")]
    public virtual MetaInfo MetaInfo { get; set; } = null!;
    

    [Required] public Guid SceneSegmentId { get; set; }
    [ForeignKey("SceneSegmentId")] public virtual SceneSegment SceneSegment { get; set; } = null!;

    /// <summary>
    /// The type of game mechanic to trigger (e.g., GiveItem, SetFlag, TriggerCombat).
    /// </summary>
    public GameTriggerType TriggerType { get; set; } = GameTriggerType.None;

    /// <summary>
    /// The ID of the target entity (e.g., ItemId, QuestId, EnemyGroupId).
    /// </summary>
    [MaxLength(128)] public string TargetId { get; set; } = string.Empty;

    /// <summary>
    /// JSON payload for complex trigger data (e.g., amount to give, boolean value to set).
    /// </summary>
    public string? Payload { get; set; }

    /// <summary>
    /// Optional description for debugging or preview purposes.
    /// </summary>
    [MaxLength(1024)] public string? Description { get; set; }
}

public enum GameTriggerType
{
    None,
    GiveItem,
    RemoveItem,
    SetFlag,
    ClearFlag,
    TriggerCombat,
    ChangeLocation,
    PlayAudio,
    Custom
}
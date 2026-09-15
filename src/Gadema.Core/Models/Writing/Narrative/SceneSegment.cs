using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gadema.Core.Models.Game;

namespace Gadema.Core.Models.Writing.Narrative;

[ModelDependency(typeof(ContentMetaInfo), typeof(Scene))]
public class SceneSegment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public Guid MetaInfoId { get; set; }
    [ForeignKey("MetaInfoId")] public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;

    [Required] public Guid SceneId { get; set; }
    [ForeignKey("SceneId")] public virtual Scene Scene { get; set; } = null!;

    public SegmentType Type { get; set; } = SegmentType.Description;
    public int OrderIndex { get; set; } = 0;

    public int Position { get; set; } = 0;
    public string? Value { get; set; }


    // Game Mechanics Migration: Moved from generic KeyEvent
    public ICollection<GameKeyEvent> GameEvents { get; set; } = new List<GameKeyEvent>();
}

public enum SegmentType
{
    Dialogue,
    Action,
    Description,
    InternalMonologue
}
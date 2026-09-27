using Gadema.Core.Models.Game.EngineIntegration;

namespace Gadema.Core.Models.Writing.Narrative;

/// <summary>
/// Represents a specific structural marker or "token" within a <see cref="Scene"/>.
/// Segments allow for indexing interactive elements (like dialogue, actions, or monologue)
/// without relying on fragile text position indices.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo), typeof(Scene))]
public class SceneSegment
{
    /// <summary>
    /// Unique identifier for the scene segment.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The ID of the associated ContentMetaInfo entity, used for identity anchoring and searching.
    /// </summary>
    [Required] public Guid MetaInfoId { get; set; }
    [ForeignKey("MetaInfoId")] public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;

    /// <summary>
    /// The ID of the parent scene this segment belongs to.
    /// </summary>
    [Required] public Guid SceneId { get; set; }
    [ForeignKey("SceneId")] public virtual Scene Scene { get; set; } = null!;

    /// <summary>
    /// The functional type of the segment (e.g., Dialogue, Action).
    /// </summary>
    public SegmentType Type { get; set; } = SegmentType.Description;

    /// <summary>
    /// The sort order for segments within a scene.
    /// </summary>
    public int OrderIndex { get; set; } = 0;

    /// <summary>
    /// The relative position of the segment within its parent text block or area.
    /// </summary>
    public int Position { get; set; } = 0;

    /// <summary>
    /// The actual content or value associated with this segment (e.g., a line of dialogue).
    /// </summary>
    public string? Value { get; set; }



    /// <summary>
    /// Collection of game-related events triggered by this segment.
    /// </summary>
    public ICollection<GameKeyEvent> GameEvents { get; set; } = new List<GameKeyEvent>();
}

/// <summary>
/// Defines the functional types of segments available for scene structure.
/// </summary>
public enum SegmentType
{
    /// <summary>Spoken lines between characters.</summary>
    Dialogue,
    /// <summary>Physical movements or environmental changes within the narrative.</summary>
    Action,
    /// <summary>Atmospheric or setting-focused text.</summary>
    Description,
    /// <summary>A character's private thoughts or internal narration.</summary>
    InternalMonologue
}
namespace Gadema.Core.Models.Writing.Narrative;

/// <summary>
/// Junction table representing the many-to-many relationship between a <see cref="Scene"/> and its associated <see cref="StoryBeat"/>s.
/// This allows a single scene to fulfill multiple narrative beats, and a beat to be satisfied across different scenes.
/// </summary>
[ModelDependency(typeof(Scene), typeof(StoryBeat))]
public class SceneStoryBeatMapping
{
    /// <summary>
    /// The ID of the associated scene.
    /// </summary>
    [Required]
    public Guid SceneId { get; set; }

    /// <summary>
    /// Navigation property for the parent scene.
    /// </summary>
    // Navigation property: Scene
    [ForeignKey("SceneId")]
    public virtual Scene Scene { get; set; } = null!;

    /// <summary>
    /// The ID of the associated story beat.
    /// </summary>
    [Required]
    public Guid StoryBeatId { get; set; }

    /// <summary>
    /// Navigation property for the target story beat.
    /// </summary>
    // Navigation property: StoryBeat
    [ForeignKey("StoryBeatId")]
    public virtual StoryBeat StoryBeat { get; set; } = null!;

    /// <summary>
    /// The sort order of this specific beat mapping within the context of its scene.
    /// </summary>
    public int BeatOrderIndex { get; set; } = 0;

    /// <summary>
        /// The timestamp when this relationship was established.
        /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


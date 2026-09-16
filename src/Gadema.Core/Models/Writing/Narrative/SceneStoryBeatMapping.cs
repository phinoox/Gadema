namespace Gadema.Core.Models.Writing.Narrative;

/// <summary>
/// Junction table for Many-to-Many relationship between Scene and StoryBeat.
/// Allows a single scene to fulfill multiple narrative beats, and a beat to be referenced by many scenes.
/// </summary>
[ModelDependency(typeof(Scene), typeof(StoryBeat))]
public class SceneStoryBeatMapping
{
    /// <summary>
    /// ID of the scene.
    /// </summary>
    [Required]
    public Guid SceneId { get; set; }

    // Navigation property: Scene
    [ForeignKey("SceneId")]
    public virtual Scene Scene { get; set; } = null!;

    /// <summary>
    /// ID of the story beat.
    /// </summary>
    [Required]
    public Guid StoryBeatId { get; set; }

    // Navigation property: StoryBeat
    [ForeignKey("StoryBeatId")]
    public virtual StoryBeat StoryBeat { get; set; } = null!;

    /// <summary>
    /// Order index for sorting beats within a scene (if a scene triggers multiple beats).
    /// </summary>
    public int BeatOrderIndex { get; set; } = 0;

    /// <summary>
    /// Timestamp when this mapping was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

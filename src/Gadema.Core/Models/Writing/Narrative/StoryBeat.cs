namespace Gadema.Core.Models.Writing.Narrative;

/// <summary>
/// Represents a Story Beat - a "Landmark" or major keypoint within a story's structure.
/// Beats serve as structural anchors that can be fulfilled by one or many scenes,
/// acting as draggable cards in the planning interface.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo), typeof(Story))]
public class StoryBeat
{
    /// <summary>
    /// Unique identifier for the story beat.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The ID of the associated ContentMetaInfo entity, used for identity anchoring and searching.
    /// </summary>
    [Required] public Guid MetaInfoId { get; set; }
    /// <summary>
    /// Navigation property for the beat's identity anchor.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;
    /// <summary>
    /// The ID of the story this beat belongs to.
    /// </summary>
    [Required] public Guid StoryId { get; set; }
    /// <summary>
    /// Navigation property for the parent story.
    /// </summary>
    [ForeignKey("StoryId")] public virtual Story Story { get; set; } = null!;

    /// <summary>
    /// A description of the beat's narrative intent or purpose.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }

    /// <summary>
    /// The sort order for this beat within its story structure, determining its visual position in timeline views.
    /// </summary>
    public int OrderIndex { get; set; } = 0;

    // ========================================================================
    // Navigation Properties
    // ========================================================================

    /// <summary>
    /// Collection of <see cref="Scene"/> entities that fulfill this narrative beat.
    /// </summary>
    public virtual ICollection<Scene> Scenes { get; set; } = new List<Scene>();

    /// <summary>
    /// Collection of outline sections that reference or link to this specific beat.
    /// </summary>
    public virtual ICollection<OutlineSection> LinkedOutlineSections { get; set; } = new List<OutlineSection>();
}
using Gadema.Core.Models.Base.Projects;

namespace Gadema.Core.Models.Writing.Narrative;

/// <summary>
/// Represents the overarching narrative work (e.g., a novel, screenplay, or campaign).
/// A story is anchored by its identity and contains a hierarchical structure of
/// outlines, beats, and chapters.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo), typeof(Project))]
public class Story
{
    /// <summary>
    /// Unique identifier for the story.
    /// </summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The human-readable name of the story.
    /// </summary>
    [Required]
    public string Name { get; set; } = "New Story";

    /// <summary>
    /// The ID of the associated ContentMetaInfo entity, used for identity anchoring and searching.
    /// </summary>
    [Required] public Guid MetaInfoId { get; set; }

    /// <summary>
    /// Navigation property for the story's identity anchor.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;

    /// <summary>
    /// A detailed description of the story's premise or concept.
    /// </summary>
    public string? Description { get; set; }
    
    // Narrative structure (Siblings under Story)
    /// <summary>
    /// The structural outline that defines the high-level narrative progression.
    /// </summary>
    public virtual StoryOutline Outline { get; set; }

    /// <summary>
    /// Collection of major narrative landmarks (beats) that define the story's arc.
    /// </summary>
    public virtual ICollection<StoryBeat> Beats { get; set; } = new List<StoryBeat>();

    /// <summary>
    /// Collection of chapters that organize the story into manageable segments.
    /// </summary>
    public virtual ICollection<StoryChapter> Chapters { get; set; } = new List<StoryChapter>();
}
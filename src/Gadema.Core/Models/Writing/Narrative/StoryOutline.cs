using Gadema.Core.Models.Base.Projects;
namespace Gadema.Core.Models.Writing.Narrative;

/// <summary>
/// Represents a Story Outline - the high-level "Map" of a project's narrative intent.
/// It serves as a container for planning, defining key story beats and organizing
/// structural sections before full scenes are developed.
/// Hierarchy: Project → StoryOutline → StoryBeat → Scene
/// </summary>
[ModelDependency(typeof(Project), typeof(StoryBeat))]
public class StoryOutline
{
    /// <summary>
    /// Unique identifier for the story outline.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The ID of the associated ContentMetaInfo entity, used for identity anchoring and searching.
    /// </summary>
    [Required] public Guid MetaInfoId { get; set; }

    /// <summary>
    /// Navigation property for the outline's identity anchor.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;

    /// <summary>
    /// The ID of the story this outline belongs to.
    /// </summary>
    [Required] public Guid StoryId { get; set; }

    /// <summary>
    /// Navigation property for the parent story.
    /// </summary>
    [ForeignKey("StoryId")]
    public virtual Story Story { get; set; } = null!;

    /// <summary>
    /// A high-level summary of the outline's purpose or content.
    /// </summary>
    [MaxLength(4096)]
    public string? Summary { get; set; } = "";
       
    /// <summary>
    /// Collection of organized, reorderable sections that structure the outline.
    /// </summary>
    public virtual ICollection<OutlineSection> Sections { get; set; } = new List<OutlineSection>();
}


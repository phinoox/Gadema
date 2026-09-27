using Gadema.Core.Models.Base.Projects;

namespace Gadema.Core.Models.Writing.Narrative;

/// <summary>
/// Represents a major narrative segment (e.g., a chapter or act) within a story.
/// Chapters provide structural organization for scenes and beats.
/// </summary>
[ModelDependency(typeof(Project))]
public class StoryChapter
{
    /// <summary>
    /// Unique identifier for the story chapter.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The ID of the associated ContentMetaInfo entity, used for identity anchoring and searching.
    /// </summary>
    [Required]
    public Guid MetaInfoId { get; set; }  
    
    /// <summary>
    /// Navigation property for the chapter's identity anchor.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo? ContentMetaInfo { get; set; }
    /// <summary>
    /// The ID of the story this chapter belongs to.
    /// </summary>
    [Required] public Guid StoryId { get; set; }

    /// <summary>
    /// Navigation property for the parent story.
    /// </summary>
    [ForeignKey("StoryId")]
     public virtual Story Story { get; set; }= null!;

    /// <summary>
    /// A description of what occurs or is planned within this chapter.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    /// <summary>
    /// The sort order for chapters within the story structure.
    /// </summary>
    [Column("order_index"), Display(Name = "Order Index")]
    public int OrderIndex { get; set; } = 0;

    /// <summary>
    /// Collection of scenes that take place within this chapter.
    /// </summary>
    public virtual ICollection<Scene> Scenes { get; set; } = new List<Scene>();

}
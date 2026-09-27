namespace Gadema.Core.Models.Writing.Narrative;

/// <summary>
/// Represents a structured section within a <see cref="StoryOutline"/>.
/// Sections allow for organizing high-level ideas and beats into coherent groups.
/// </summary>
[ModelDependency(typeof(StoryOutline))]
public class OutlineSection
{
    /// <summary>
    /// Unique identifier for the outline section.
    /// </summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The human-readable title of this section.
    /// </summary>
    [Required]
    public string Title { get; set; } = "New Section";

    /// <summary>
    /// The raw content/notes for this section.
    /// </summary>
    public string RawText { get; set;} = "";

    /// <summary>
        /// The sort order of this section within the outline.
        /// </summary>
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// Collection of <see cref="StoryBeat"/> entities linked to this specific section.
    /// </summary>
    public List<StoryBeat> LinkedBeats { get; set; } = new();

    /// <summary>
    /// The ID of the parent story outline.
    /// </summary>
    public Guid StoryOutlineId { get; set; }

    /// <summary>
    /// Navigation property for the parent story outline.
    /// </summary>
    [ForeignKey("StoryOutlineId")]
    public virtual StoryOutline StoryOutline { get; set; }
}
// =============================================================================
using Gadema.Core.Enums;
using Gadema.Core.Models.Narrative;
using Gadema.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models;

/// <summary>
/// Represents a Story Outline - the "Map" of the project's narrative intent.
/// Acts as a container for high-level ideas and StoryBeats (the "Landmarks").
/// This is a distinct document where users write summaries and scene prototypes.
/// Hierarchy: Project → StoryOutline → StoryBeat → Scene
/// </summary>
[ModelDependency(typeof(Project), typeof(StoryBeat))]
public class StoryOutline
{
    /// <summary>
    /// Unique identifier for the story outline.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public Guid MetaInfoId { get; set; }
    [ForeignKey("MetaInfoId")]
    public virtual MetaInfo MetaInfo { get; set; } = null!;

    /// <summary>
    /// RawText - The "Prototype" document (Markdown).
    /// Users write high-level summaries, plot points, and scene prototypes here.
    /// Markers like [beat:123] can be used to reference StoryBeats within this text.
    /// </summary>
    [Required]
    public string RawText { get; set; } = "";

    /// <summary>
    /// Summary of the outline section (legacy field, superseded by RawText).
    /// Kept for backward compatibility.
    /// </summary>
    [MaxLength(4096)]
    public string? Summary { get; set; } = "";

    /// <summary>
    /// Snapshot of character state at this point in the story (legacy field).
    /// Kept for backward compatibility.
    /// </summary>
    [MaxLength(512)]
    public string? CharacterSnapshot { get; set; }

    /// <summary>
    /// Theme statement for this section (legacy field).
    /// Kept for backward compatibility.
    /// </summary>
    [MaxLength(1024)]
    public string? ThemeStatement { get; set; }

    /// <summary>
    /// Outline status (DraftOutline, Finalized, Published).
    /// </summary>
    [EnumDataType(typeof(OutlineStatusEnum)), Required, Display(Name = "Outline Status")]
    public OutlineStatusEnum OutlineStatus { get; set; } = OutlineStatusEnum.DraftOutline;
   
    // ========================================================================
    // Navigation Properties
    // ========================================================================

    /// <summary>
    /// Collection of StoryBeats (Landmarks) within this Outline.
    /// Beats are major keypoints that can be dragged and reordered in the UI.
    /// </summary>
    public virtual ICollection<StoryBeat> StoryBeats { get; set; } = new List<StoryBeat>();

    /// <summary>
    /// Collection of Scenes that belong to this Outline.
    /// Scenes are the execution units that fulfill the Outline's plan.
    /// </summary>
    public virtual ICollection<Scene> Scenes { get; set; } = new List<Scene>();
}

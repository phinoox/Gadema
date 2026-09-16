using Gadema.Core.Models.Base.MetaInfo;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gadema.Core.Models.Writing.Narrative;

/// <summary>
/// Represents a Story Beat - a "Landmark" or major keypoint within a StoryOutline.
/// Beats act as structural anchors for scenes and become draggable "Cards" in the Right Panel.
/// Hierarchy: StoryOutline → StoryBeat ↔ Scene (Many-to-Many via junction table)
/// </summary>
[ModelDependency(typeof(ContentMetaInfo), typeof(Story))]
public class StoryBeat
{
    /// <summary>
    /// Unique identifier for the story beat.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required] public Guid MetaInfoId { get; set; }
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;

    [Required] public Guid StoryId { get; set; }
    [ForeignKey("StoryId")] public virtual Story Story { get; set; } = null!;

    /// <summary>
    /// Description of the beat's narrative intent.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }

    /// <summary>
    /// Order index for sorting beats in the Timeline/Map views.
    /// Determines the visual order of "Cards" in the Right Panel.
    /// </summary>
    public int OrderIndex { get; set; } = 0;


    // ========================================================================
    // Navigation Properties
    // ========================================================================

    /// <summary>
    /// Collection of Scenes linked to this beat via the junction table.
    /// A beat can be referenced by many scenes (Many-to-Many).
    /// </summary>
    public virtual ICollection<Scene> Scenes { get; set; } = new List<Scene>();

    /// <summary>
    /// Collection of OutlineSections that link to this beat.
    /// </summary>
    public virtual ICollection<OutlineSection> LinkedOutlineSections { get; set; } = new List<OutlineSection>();
}
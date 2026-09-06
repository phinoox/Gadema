// =============================================================================
using Gadema.Core.Models.Narrative;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models;

/// <summary>
/// Represents a Story Beat - a "Landmark" or major keypoint within a StoryOutline.
/// Beats act as structural anchors for scenes and become draggable "Cards" in the Right Panel.
/// Hierarchy: StoryOutline → StoryBeat ↔ Scene (Many-to-Many via junction table)
/// </summary>
[DependencyResolver.ModelDependency(typeof(StoryOutline))]
public class StoryBeat
{
    /// <summary>
    /// Unique identifier for the story beat.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the StoryOutline this beat belongs to (The "Map").
    /// Beats are structural elements owned by an Outline.
    /// </summary>
    [Required, Display(Name = "Story Outline ID")]
    public Guid StoryOutlineId { get; set; }

    // Navigation property: StoryOutline (Many-to-One)
    [ForeignKey("StoryOutlineId")]
    public virtual StoryOutline StoryOutline { get; set; } = null!;
    
    /// <summary>
    /// Title of the beat (e.g., "The Discovery").
    /// </summary>
    [MaxLength(128), Required, Display(Name = "Beat Title")]
    public string BeatTitle { get; set; } = "";
    
    /// <summary>
    /// URL-friendly slug for the beat (unique within the Outline).
    /// </summary>
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
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
    
    /// <summary>
    /// Indicates if the beat is published/finalized.
    /// </summary>
    public bool Published { get; set; } = false;

    /// <summary>
    /// Timestamp when this beat was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when this beat was last modified.
    /// </summary>
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;

    // ========================================================================
    // Navigation Properties
    // ========================================================================

    /// <summary>
    /// Collection of Scenes linked to this beat via the junction table.
    /// A beat can be referenced by many scenes (Many-to-Many).
    /// </summary>
    public virtual ICollection<Scene> Scenes { get; set; } = new List<Scene>();
}
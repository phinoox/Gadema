// =============================================================================
using Gadema.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models;

/// <summary>
/// Represents a story sequence (chapter) within a project.
/// Used for organizing narrative structure and beat sheets.
/// </summary>
public class StorySequence
{
    /// <summary>
    /// Unique identifier for the story sequence.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? ContentItemId { get; set; }  
    
    [ForeignKey("ContentItemId")]
    public virtual ContentItem? ContentItem { get; set; }

    
    /// <summary>
    /// ID of the project this sequence belongs to.
    /// </summary>
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }

    // Navigation property: Project (Many-to-One)
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }
    
    /// <summary>
    /// Parent sequence ID for hierarchical organization.
    /// </summary>
    public Guid? ParentSequenceId { get; set; }

    // Navigation property: Parent Sequence (self-referencing)
    [ForeignKey("ParentSequenceId")]
    public virtual StorySequence? ParentSequence { get; set; }
    
    /// <summary>
    /// Name of the sequence (e.g., "Chapter 1").
    /// </summary>
    [MaxLength(128), Required, Display(Name = "Sequence Name")]
    public string SequenceName { get; set; } = "";
    
    /// <summary>
    /// URL-friendly slug for the sequence (unique).
    /// </summary>
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    /// <summary>
    /// Description of the sequence.
    /// </summary>
    [MaxLength(4096)]
    public string? SequenceDescription { get; set; }
    
    /// <summary>
    /// Indicates if the sequence is published.
    /// </summary>
    public bool Published { get; set; } = false;

    /// <summary>
    /// Order index for sorting sequences.
    /// </summary>
    [Column("order_index"), Display(Name = "Order Index")]
    public int OrderIndex { get; set; } = 0;

    /// <summary>
    /// Outline summary (newly added property).
    /// </summary>
    [MaxLength(256)]
    public string? OutlineSummary { get; set; }

    // Collection navigation properties
    /// <summary>
    /// Navigation property: Collection of child sequences in this hierarchy.
    /// Foreign key: ParentSequenceId (matches FK in StorySequence)
    /// </summary>
    public virtual ICollection<StorySequence> ChildSequences { get; set; } = new List<StorySequence>();

    /// <summary>
    /// Navigation property: Collection of story outlines/sections within this sequence.
    /// Foreign key: SequenceId (matches FK in StoryOutline)
    /// </summary>
    public virtual ICollection<StoryOutline> Outlines { get; set; } = new List<StoryOutline>();

    /// <summary>
    /// Navigation property: Collection of beats in this sequence.
    /// Foreign key: SequenceId (matches FK in StoryBeat)
    /// </summary>
    public virtual ICollection<StoryBeat> Beats { get; set; } = new List<StoryBeat>();

}
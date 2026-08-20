// =============================================================================
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

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
    
    /// <summary>
    /// ID of the project this sequence belongs to.
    /// </summary>
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }
    
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
}
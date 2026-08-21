// =============================================================================
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Represents a story beat within a sequence.
/// Used for detailed narrative structure and scene organization.
/// </summary>
public class StoryBeat
{
    /// <summary>
    /// Unique identifier for the story beat.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the sequence this beat belongs to.
    /// </summary>
    [Required, Display(Name = "Sequence ID")]
    public Guid SequenceId { get; set; }
    
    /// <summary>
    /// Title of the beat (e.g., "The Discovery").
    /// </summary>
    [MaxLength(128), Required, Display(Name = "Beat Title")]
    public string BeatTitle { get; set; } = "";
    
    /// <summary>
    /// URL-friendly slug for the beat (unique).
    /// </summary>
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    /// <summary>
    /// Description of the beat.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Order index for sorting beats.
    /// </summary>
    public int OrderIndex { get; set; } = 0;
    
    /// <summary>
    /// Indicates if the beat is published.
    /// </summary>
    public bool Published { get; set; } = false;
}
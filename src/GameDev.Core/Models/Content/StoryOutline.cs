// =============================================================================
using GameDev.Core.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Represents a story outline (chapter/sequence) within a project.
/// Used for narrative structure and beat sheet organization.
/// </summary>
public class StoryOutline
{
    /// <summary>
    /// Unique identifier for the story outline.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// ID of the sequence this outline belongs to.
    /// </summary>
    [Required]
    public Guid SequenceId { get; set; }

    public Guid? ContentItemId { get; set; }  
    
    [ForeignKey("ContentItemId")]
    public virtual ContentItem? ContentItem { get; set; }

    // Navigation property: StorySequence (Many-to-One)
    [ForeignKey("SequenceId")]
    public virtual StorySequence StorySequence { get; set; }

    /// <summary>
    /// Summary of the outline section.
    /// </summary>
    [MaxLength(4096)]
    public string Summary { get; set; } = "";

    /// <summary>
    /// Snapshot of character state at this point in the story.
    /// </summary>
    [MaxLength(512)]
    public string? CharacterSnapshot { get; set; }

    /// <summary>
    /// Theme statement for this section.
    /// </summary>
    [MaxLength(1024)]
    public string? ThemeStatement { get; set; }

    /// <summary>
    /// Outline status (DraftOutline, Finalized, Published).
    /// </summary>
    [EnumDataType(typeof(OutlineStatusEnum)), Required, Display(Name = "Outline Status")]
    public OutlineStatusEnum OutlineStatus { get; set; } = OutlineStatusEnum.DraftOutline;



}

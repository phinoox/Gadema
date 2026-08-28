// =============================================================================
using System.ComponentModel.DataAnnotations;
// =============================================================================

namespace Gadema.Core.Dtos.StoryOutlining;

/// <summary>
/// DTO for creating a new story sequence (chapter).
/// </summary>
public class CreateSequenceDto
{
    /// <summary>
    /// ID of the project this sequence belongs to (required).
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// Name of the sequence (e.g., "Chapter 1") (required).
    /// </summary>
    [MaxLength(128), Required, Display(Name = "Sequence Name")]
    public string SequenceName { get; set; } = "";
    
    /// <summary>
    /// URL-friendly slug for the sequence (optional, auto-generated if not provided).
    /// </summary>
    [MaxLength(128)]
    public string? Slug { get; set; }
}
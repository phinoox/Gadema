// =============================================================================
// SequenceResponseDto - Single story sequence response
// =============================================================================

using System.ComponentModel.DataAnnotations;

namespace GameDev.Core.Dtos.StoryOutlining;

/// <summary>
/// Single story outline response.
/// </summary>
public class SequenceResponseDto
{
    public Guid Id { get; set; }
    
    [Required, MaxLength(128)]
    public string SequenceName { get; set; } = "";
    
    [MaxLength(128)]
    public string Slug { get; set; } = "";
    
    [MaxLength(4096)]
    public string? SequenceDescription { get; set; }
    
    public bool Published { get; set; }
}

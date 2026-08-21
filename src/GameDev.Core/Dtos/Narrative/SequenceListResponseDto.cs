// =============================================================================
// SequenceListResponseDto - Response for story outline sequences
// =============================================================================

using System.ComponentModel.DataAnnotations;

namespace GameDev.Core.Dtos.StoryOutlining;

/// <summary>
/// List of story sequences response.
/// </summary>
public class SequenceListResponseDto
{
    public IEnumerable<SequenceResponseDto> Items { get; set; } = Enumerable.Empty<SequenceResponseDto>();
    
    public int TotalCount { get; set; }
    
    public int PageNumber { get; set; }
    
    public int PageSize { get; set; }
}

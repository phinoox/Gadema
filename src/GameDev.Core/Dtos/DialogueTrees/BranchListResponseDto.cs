// =============================================================================
// BranchListResponseDto - Response for listing dialogue branches
// =============================================================================

namespace GameDev.Core.Dtos.DialogueTrees;

/// <summary>
/// List of dialogue branches response.
/// </summary>
public class BranchListResponseDto
{
    public IEnumerable<BranchResponseDto> Items { get; set; } = Enumerable.Empty<BranchResponseDto>();
    
    public int TotalCount { get; set; }
    
    public int PageNumber { get; set; }
    
    public int PageSize { get; set; }
}

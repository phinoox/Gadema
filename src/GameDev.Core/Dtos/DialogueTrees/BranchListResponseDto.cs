// =============================================================================
// BranchListResponseDto - Response for listing dialogue branches
// =============================================================================

using System.ComponentModel.DataAnnotations;

namespace GameDev.Core.Dtos.DialogueTrees;

/// <summary>
/// List of dialogue branches response.
/// </summary>
public class BranchListResponseDto
{
    public IEnumerable<BranchResponseDto> Items { get; set; } = Enumerable.Empty<BranchResponseDto>();
    
    [Display(Name = "Total Count")]
    public int TotalCount { get; set; }
    
    [Display(Name = "Page Number")]
    public int PageNumber { get; set; }
    
    [Display(Name = "Page Size")]
    public int PageSize { get; set; }
}

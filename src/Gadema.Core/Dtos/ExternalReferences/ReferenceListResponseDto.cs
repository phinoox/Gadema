// =============================================================================
// ReferenceListResponseDto - Response for listing external references
// =============================================================================

namespace Gadema.Core.Dtos.ExternalReferences;

/// <summary>
/// List of external references response.
/// </summary>
public class ReferenceListResponseDto
{
    public IEnumerable<ReferenceResponseDto> Items { get; set; } = Enumerable.Empty<ReferenceResponseDto>();
    
    public int TotalCount { get; set; }
    
    public int PageNumber { get; set; }
    
    public int PageSize { get; set; }
}

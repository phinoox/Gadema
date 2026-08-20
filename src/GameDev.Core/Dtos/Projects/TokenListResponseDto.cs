// =============================================================================
// TokenListResponseDto - Response for project token list
// =============================================================================

namespace GameDev.Core.Dtos.Projects;

/// <summary>
/// List of project tokens response.
/// </summary>
public class TokenListResponseDto
{
    public IEnumerable<ProjectTokenResponseDto> Items { get; set; } = Enumerable.Empty<ProjectTokenResponseDto>();
    
    public int TotalCount { get; set; }
}

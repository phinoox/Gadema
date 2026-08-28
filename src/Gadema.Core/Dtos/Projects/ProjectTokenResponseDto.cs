// =============================================================================
// ProjectTokenResponseDto - Response for project token operations
// =============================================================================

using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Projects;

/// <summary>
/// Single project token response.
/// </summary>
public class ProjectTokenResponseDto
{
    public Guid Id { get; set; }
    
    [Required, MaxLength(128)]
    public string TokenName { get; set; } = "";
    
    public bool IsActive { get; set; }
    
    public DateTime? ExpiresAt { get; set; }
}

/// <summary>
/// List of project tokens response.
/// </summary>
public class ProjectTokenListResponseDto
{
    public IEnumerable<ProjectTokenResponseDto> Items { get; set; } = Enumerable.Empty<ProjectTokenResponseDto>();
    
    public int TotalCount { get; set; }
}

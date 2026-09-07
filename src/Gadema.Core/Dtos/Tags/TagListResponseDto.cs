// =============================================================================
// TagListResponseDto - Response for listing tags
// =============================================================================

using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Tags;

/// <summary>
/// List of tags response.
/// </summary>
public class TagListResponseDto
{
    public IEnumerable<TagResponseDto> Tags { get; set; } = Enumerable.Empty<TagResponseDto>();
    
    public int TotalCount { get; set; }
    
    public int PageNumber { get; set; }
    
    public int PageSize { get; set; }
}

/// <summary>
/// Single tag response.
/// </summary>
public class TagResponseDto
{
    public Guid Id { get; set; }
    
    [Required, MaxLength(128)]
    public string Name { get; set; } = "";
    
    [MaxLength(128)]
    public string Slug { get; set; } = "";
    
    [MaxLength(36)]
    public string? ColorHex { get; set; }
}

// =============================================================================
// CommentListResponseDto - Response for listing comments
// =============================================================================

using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Comments;

/// <summary>
/// List of content comments response.
/// </summary>
public class CommentListResponseDto
{
    public IEnumerable<CommentResponseDto> Items { get; set; } = Enumerable.Empty<CommentResponseDto>();
    
    public int TotalCount { get; set; }
    
    public int PageNumber { get; set; }
    
    public int PageSize { get; set; }
}

/// <summary>
/// Single comment response.
/// </summary>
public class CommentResponseDto
{
    public Guid Id { get; set; }
    
    [Required, Display(Name = "Content Item ID")]
    public Guid MetaInfoId { get; set; }
    
    [Required, MaxLength(256)]
    public string CommentedByUserName { get; set; } = "";
    
    [MaxLength(4096)]
    public string CommentText { get; set; } = "";
    
    public DateTime CreatedAt { get; set; }
}

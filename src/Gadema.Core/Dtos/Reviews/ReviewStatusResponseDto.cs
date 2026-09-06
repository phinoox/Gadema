// =============================================================================
// ReviewStatusResponseDto - Response for review status
// =============================================================================

using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Reviews;

/// <summary>
/// List of review statuses response.
/// </summary>
public class ReviewStatusListResponseDto
{
    public IEnumerable<ReviewStatusResponseDto> Items { get; set; } = Enumerable.Empty<ReviewStatusResponseDto>();
    
    public int TotalCount { get; set; }
    
    public int PageNumber { get; set; }
    
    public int PageSize { get; set; }
}

/// <summary>
/// Single review status response.
/// </summary>
public class ReviewStatusResponseDto
{
    public Guid Id { get; set; }
    
    [Required, Display(Name = "Content Item ID")]
    public Guid MetaInfoId { get; set; }
    
    public int Status { get; set; }  // Pending, Approved, Rejected
    
    public Guid? ReviewedByUserId { get; set; }
    
    [MaxLength(4096)]
    public string? ReviewComments { get; set; }
    
    public DateTime? ReviewedAt { get; set; }
}

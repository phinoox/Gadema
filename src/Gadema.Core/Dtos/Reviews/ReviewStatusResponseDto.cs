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
    
    /// <summary>
    /// Review status (0=Pending, 1=Approved, 2=Rejected).
    /// </summary>
    public int Status { get; set; }
    
    /// <summary>
    /// ID of the user who reviewed this content.
    /// </summary>
    public Guid? ReviewedByUserId { get; set; }
    
    /// <summary>
    /// Name of the reviewer.
    /// </summary>
    public string? ReviewerName { get; set; }
    
    [MaxLength(4096)]
    public string? ReviewComments { get; set; }
    
    public DateTime? ReviewedAt { get; set; }
}

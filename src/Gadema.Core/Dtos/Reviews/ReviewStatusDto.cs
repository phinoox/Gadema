// =============================================================================
// ReviewStatusResponseDto - Response for review status
// =============================================================================

using Gadema.Core.Enums;


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


public class ReviewStatusCreateDto
{
    [Required] public Guid TargetId { get; set; } // Pointing to the content being reviewed
    [Required] public ReviewStatusEnum Status { get; set; }
    public string? ReviewComments { get; set; }
}

/// <summary>
/// Single review status response.
/// </summary>
public class ReviewStatusResponseDto
{
    public Guid Id { get; set; }
    
    [Required, Display(Name = "Content Item ID")]
    public Guid TargetId { get; set; } // Replaced MetaInfoId
    
    public ReviewStatusEnum Status { get; set; }
    
    public Guid? ReviewedByUserId { get; set; }
    
    public string? ReviewerName { get; set; }
    
    [MaxLength(4096)]
    public string? ReviewComments { get; set; }
    
    public DateTime? ReviewedAt { get; set; }
}

// ... ListResponseDto remains unchanged ...
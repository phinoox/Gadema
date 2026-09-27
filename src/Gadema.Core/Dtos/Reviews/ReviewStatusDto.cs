/// <summary>
/// Data transfer object for creating a new review status entry.
/// </summary>
public class ReviewStatusCreateDto
{
    /// <summary>
    /// The unique identifier of the content item being reviewed (e.g., Project, Scene).
    /// </summary>
    [Required] public Guid TargetId { get; set; }

    /// <summary>
    /// The review status to be assigned (e.g., Pending, Approved, Rejected).
    /// </summary>
    [Required] public ReviewStatusEnum Status { get; set; }

    /// <summary>
    /// Optional comments or feedback regarding the review.
    /// </summary>
    public string? ReviewComments { get; set; }
}

/// <summary>
/// Represents a single review status entry for a content item.
/// </summary>
public class ReviewStatusResponseDto
{
    /// <summary>
    /// The unique identifier of the review record.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The ID of the target content item being reviewed.
    /// </summary>
    [Required, Display(Name = "Content Item ID")]
    public Guid TargetId { get; set; } 
    
    /// <summary>
    /// The current status of the review (e.g., Approved).
    /// </summary>
    public ReviewStatusEnum Status { get; set; }
    
    /// <summary>
    /// The unique identifier of the user who performed the review.
    /// </summary>
    public Guid? ReviewedByUserId { get; set; }
    
    /// <summary>
    /// The display name of the reviewer for UI convenience.
    /// </summary>
    public string? ReviewerName { get; set; }
    
    /// <summary>
    /// Feedback or notes provided by the reviewer.
    /// </summary>
    [MaxLength(4096)]
    public string? ReviewComments { get; set; }
    
    /// <summary>
    /// The timestamp when the review was performed in UTC.
    /// </summary>
    public DateTime? ReviewedAt { get; set; }
}

/// <summary>
/// A paginated list response for review statuses.
/// </summary>
public class ReviewStatusListResponseDto
{
    /// <summary>
    /// The collection of retrieved review status entries.
    /// </summary>
    public IEnumerable<ReviewStatusResponseDto> Items { get; set; } = Enumerable.Empty<ReviewStatusResponseDto>();
    
    /// <summary>
    /// Total number of reviews available across all pages.
    /// </summary>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// The current page index (1-based).
    /// </summary>
    public int PageNumber { get; set; }
    
    /// <summary>
    /// The number of items per page.
    /// </summary>
    public int PageSize { get; set; }
}

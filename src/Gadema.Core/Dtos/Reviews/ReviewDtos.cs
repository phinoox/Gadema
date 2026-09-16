// =============================================================================
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Dtos.Base.Infrastructure;

namespace Gadema.Core.Dtos.Reviews;

/// <summary>
/// DTO for submitting a review on content.
/// </summary>
public class ReviewCreateDto
{
    /// <summary>
    /// ContentMetaInfo data for the review's identity (used as the review's own record).
    /// </summary>
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();

    /// <summary>
    /// ID of the content item being reviewed.
    /// </summary>
    [Required] public Guid ContentItemId { get; set; }

    /// <summary>
    /// The reviewer's user ID.
    /// If null, the currently authenticated user is used.
    /// </summary>
    public Guid? ReviewerId { get; set; }

    /// <summary>
    /// Rating score (1-5 stars).
    /// </summary>
    [Range(1, 5)] public int? StarRating { get; set; }

    /// <summary>
    /// Detailed review text.
    /// </summary>
    [MaxLength(8192)] public string? ReviewText { get; set; }

    /// <summary>
    /// The review status (Pending, Approved, Rejected).
    /// Default is Pending unless this is an admin re-review.
    /// </summary>
    public ReviewStatusEnum Status { get; set; } = ReviewStatusEnum.Pending;
}

/// <summary>
/// DTO for updating a review (partial update).
/// </summary>
public class ReviewUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// ContentMetaInfo fields (nullable — omit to keep current values).
    /// </summary>
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    [Range(1, 5)] public int? StarRating { get; set; }
    [MaxLength(8192)] public string? ReviewText { get; set; }
    public ReviewStatusEnum? Status { get; set; }
}

/// <summary>
/// Response DTO for a single review. Inherits ContentMetaInfo state.
/// </summary>
public class ReviewResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// ID of the content item being reviewed.
    /// </summary>
    [Required] public Guid ContentItemId { get; set; }

    /// <summary>
    /// Title of the content item (denormalized for convenience).
    /// </summary>
    [MaxLength(256)] public string? ContentTypeTitle { get; set; }

    /// <summary>
    /// The reviewer's user ID.
    /// </summary>
    public Guid ReviewerId { get; set; }

    /// <summary>
    /// Name of the reviewer (denormalized).
    /// </summary>
    [MaxLength(128)] public string? ReviewerName { get; set; }

    /// <summary>
    /// Rating score (1-5 stars). Null if no rating was given.
    /// </summary>
    [Range(1, 5)] public int? StarRating { get; set; }

    /// <summary>
    /// Detailed review text.
    /// </summary>
    [MaxLength(8192)] public string? ReviewText { get; set; }

    /// <summary>
    /// The review status (Pending, Approved, Rejected).
    /// </summary>
    public ReviewStatusEnum Status { get; set; } = ReviewStatusEnum.Pending;
}

/// <summary>
/// List response for reviews on a single content item.
/// </summary>
public class ReviewListResponseDto
{
    /// <summary>
    /// The parent content item ID being reviewed.
    /// </summary>
    public Guid ContentItemId { get; set; }

    public IEnumerable<ReviewResponseDto> Items { get; set; } = Enumerable.Empty<ReviewResponseDto>();

    public int TotalCount { get; set; }
}

/// <summary>
/// Request DTO for creating a review assignment (admin workflow).
/// </summary>
public class ReviewAssignmentCreateDto
{
    /// <summary>
    /// ContentMetaInfo data for the assignment record.
    /// </summary>
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();

    /// <summary>
    /// ID of the content item needing review.
    /// </summary>
    [Required] public Guid ContentItemId { get; set; }

    /// <summary>
    /// The reviewer to assign. Null means "assign to me".
    /// </summary>
    public Guid? AssignedReviewerId { get; set; }

    /// <summary>
    /// Priority level (optional). 1=Low, 2=Medium, 3=High, 4=Critical.
    /// </summary>
    [Range(1, 4)] public int? Priority { get; set; } = 2;

    /// <summary>
    /// Optional: custom review questions/instructions.
    /// </summary>
    [MaxLength(4096)] public string? ReviewInstructions { get; set; }
}

/// <summary>
/// DTO for approving/rejecting a content item.
/// </summary>
public class ApproveContentDto
{
    /// <summary>
    /// ID of the content item being reviewed (required).
    /// </summary>
    [Required]
    public Guid MetaInfoId { get; set; }
    
    /// <summary>
    /// Review status: 0=Pending, 1=Approved, 2=Rejected (required).
    /// </summary>
    [Required, Range(0, 2)]
    public ReviewStatusEnum Status { get; set; }
    
    /// <summary>
    /// Review comments (required if rejecting).
    /// </summary>
    [MaxLength(4096)]
    public string? ReviewComment { get; set; } = null!;
}
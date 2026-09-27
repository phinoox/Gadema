// ... existing imports ...

using Gadema.Core.Models.Access;

namespace Gadema.Core.Models.Base.MetaInfo;

/// <summary>
/// Represents a review status for a specific piece of content.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo))] // Still dependency of ContentMetaInfo conceptually, but no direct navigation
public class ReviewStatus
{
    /// <summary>
    /// Unique identifier for this review record.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary>
    /// The ID of the content item being reviewed (the anchor).
    /// </summary>
    [Required]
    public Guid TargetId { get; set; }

    /// <summary>
    /// The current status of the review process.
    /// </summary>
    public ReviewStatusEnum Status { get; set; }
    
    /// <summary>
    /// The ID of the user who performed the review.
    /// </summary>
    public Guid? ReviewedByUserId { get; set; }

    /// <summary>
    /// Navigation property for the reviewer.
    /// </summary>
    [ForeignKey("ReviewedByUserId")]
    public virtual User? Reviewer { get; set; }
    
    /// <summary>
    /// The qualitative feedback or notes provided by the reviewer.
    /// </summary>
    [MaxLength(4096)]
    public string? ReviewComment { get; set; }
    
    /// <summary>
    /// The timestamp when the review was completed.
    /// </summary>
    public DateTime? ReviewedAt { get; set; }

    /// <summary>
    /// The timestamp when the review record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
// ... existing imports ...

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gadema.Core.Models.Base.MetaInfo;

/// <summary>
/// Represents a review status for content items.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo))] // Still dependency of ContentMetaInfo conceptually, but no direct navigation
public class ReviewStatus
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the content item this review belongs to (the Anchor).
    /// </summary>
    [Required]
    public Guid TargetId { get; set; }

    /// <summary>
    /// Status: 0=Pending, 1=Approved, 2=Rejected.
    /// </summary>
    public ReviewStatusEnum Status { get; set; }
    
    /// <summary>
    /// ID of the user who reviewed the content.
    /// </summary>
    public Guid? ReviewedByUserId { get; set; }

    [ForeignKey("ReviewedByUserId")]
    public virtual User? Reviewer { get; set; }
    
    [MaxLength(4096)]
    public string? ReviewComment { get; set; }
    
    public DateTime? ReviewedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Represents a review status for content items.
/// </summary>
public class ReviewStatus
{
    /// <summary>
    /// Unique identifier for the review status.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the content item this review belongs to.
    /// </summary>
    [Required]
    public Guid ContentItemId { get; set; }
    
    /// <summary>
    /// Status: 0=Pending, 1=Approved, 2=Rejected.
    /// </summary>
    public int Status { get; set; }  // Enum: Pending, Approved, Rejected
    
    /// <summary>
    /// ID of the user who reviewed the content.
    /// </summary>
    public Guid? ReviewedByUserId { get; set; }
    
    /// <summary>
    /// Review comments.
    /// </summary>
    [MaxLength(4096)]
    public string? ReviewComments { get; set; }
    
    /// <summary>
    /// Timestamp when the review was completed.
    /// </summary>
    public DateTime? ReviewedAt { get; set; }
}
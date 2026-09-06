// =============================================================================
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models;

/// <summary>
/// Represents a review status for content items.
/// </summary>
[DependencyResolver.ModelDependency(typeof(ContentItem))]
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

    // Navigation property: ContentItem (Many-to-One)
    [ForeignKey("ContentItemId")]
    public virtual ContentItem? ContentItem { get; set; }
    
    /// <summary>
    /// Status: 0=Pending, 1=Approved, 2=Rejected.
    /// </summary>
    public int Status { get; set; }  // Enum: Pending, Approved, Rejected
    
    /// <summary>
    /// ID of the user who reviewed the content.
    /// </summary>
    public Guid? ReviewerId { get; set; }

    // Navigation property: Reviewer (User) - Optional Many-to-One relationship
    [ForeignKey("ReviewedByUserId")]
    public virtual User? Reviewer { get; set; }
    
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
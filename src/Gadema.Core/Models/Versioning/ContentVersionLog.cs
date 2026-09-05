// =============================================================================
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models;

/// <summary>
/// Represents a content version log for rollback support.
/// </summary>
[DependencyResolver.ModelDependency(typeof(ContentItem))]
public class ContentVersionLog
{
    /// <summary>
    /// Unique identifier for the version log entry.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the content item this version log belongs to.
    /// </summary>
    [Required]
    public Guid ContentItemId { get; set; }

    // Navigation property: ContentItem (Many-to-One)
    [ForeignKey("ContentItemId")]
    public virtual ContentItem ContentItem { get; set; }
    
    /// <summary>
    /// ID of the user who made the change.
    /// </summary>
    [Required]
    public Guid ChangedByUserId { get; set; }
    
    /// <summary>
    /// Description of the change.
    /// </summary>
    [MaxLength(2048)]
    public string? ChangeDescription { get; set; }
    
    /// <summary>
    /// Version number.
    /// </summary>
    public int VersionNumber { get; set; }
    
    /// <summary>
    /// Timestamp when the version was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}

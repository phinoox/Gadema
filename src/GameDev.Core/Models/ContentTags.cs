// =============================================================================
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Junction table for many-to-many relationship between ContentItems and Tags.
/// Supports ordering of tags on content items.
/// </summary>
public class ContentTags
{
    /// <summary>
    /// Unique identifier for the junction record.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the content item this tag is associated with.
    /// </summary>
    [Required, Display(Name = "Content Item ID")]
    public Guid ContentItemId { get; set; }
    
    /// <summary>
    /// ID of the tag being applied to the content item.
    /// </summary>
    [Required, Display(Name = "Tag ID")]
    public Guid TagId { get; set; }
    
    /// <summary>
    /// Order index for displaying tags (nullable for future enhancements).
    /// </summary>
    public int? OrderIndex { get; set; }  // Nullable for future enhancements
    
    /// <summary>
    /// Timestamp when the tag was added.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
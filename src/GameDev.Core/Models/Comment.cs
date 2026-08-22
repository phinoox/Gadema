// =============================================================================
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Represents a comment on a content item.
/// Supports visibility control (private, team-only, public).
/// </summary>
public class Comment
{
    /// <summary>
    /// Unique identifier for the comment.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the content item this comment belongs to.
    /// </summary>
    [Required]
    public Guid ContentItemId { get; set; }

    // Navigation property: ContentItem (Many-to-One)
    [ForeignKey("ContentItemId")]
    public virtual ContentItem ContentItem { get; set; }
    
    /// <summary>
    /// ID of the user who commented.
    /// </summary>
    [Required]
    public Guid CommentedByUserId { get; set; }
    
    /// <summary>
    /// Comment text (Markdown/HTML).
    /// </summary>
    [MaxLength(4096)]
    public string CommentText { get; set; } = "";
    
    /// <summary>
    /// Visibility: private, team-only, public.
    /// </summary>
    [MaxLength(64)]
    public string? Visibility { get; set; }  // private, team-only, public
    
    /// <summary>
    /// Timestamp when the comment was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}
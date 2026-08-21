// =============================================================================
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Represents a comment on a task.
/// </summary>
public class TaskComments
{
    /// <summary>
    /// Unique identifier for the task comment.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the task this comment belongs to.
    /// </summary>
    [Required]
    public Guid TaskId { get; set; }
    
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
    /// Timestamp when the comment was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
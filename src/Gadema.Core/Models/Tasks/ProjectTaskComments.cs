// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Tasks;

/// <summary>
/// Represents a comment on a task.
/// </summary>
[ModelDependency(typeof(ProjectTask))]
public class ProjectTaskComment
{
    /// <summary>
    /// Unique identifier for the task comment.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// ID of the task this comment belongs to.
    /// </summary>
    [Required, Display(Name = "Project Task ID")]
    public Guid ProjectTaskId { get; set; }

    // Navigation property: ProjectTask (Many-to-One)
    [ForeignKey("ProjectTaskId")]
    public virtual ProjectTask ProjectTask { get; set; }

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

    /// <summary>
    /// FK to ProjectTask.Id (for junction table FK-as-PK pattern).
    /// </summary>
    public Guid TaskId { get; set; } = Guid.NewGuid();  // Initialize with Id after object creation

}
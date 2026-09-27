// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Tasks;

/// <summary>
/// Represents a user-generated comment attached to a <see cref="ProjectTask"/>.
/// </summary>
[ModelDependency(typeof(ProjectTask))]
public class ProjectTaskComment
{
    /// <summary>
    /// Unique identifier for the task comment.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The ID of the associated project task.
    /// </summary>
    [Required, Display(Name = "Project Task ID")]
    public Guid ProjectTaskId { get; set; }

    /// <summary>
    /// Navigation property for the parent project task.
    /// </summary>
    [ForeignKey("ProjectTaskId")]
    public virtual ProjectTask ProjectTask { get; set; }
    /// <summary>
    /// The ID of the user who authored this comment.
    /// </summary>
    [Required]
    public Guid CommentedByUserId { get; set; }

    /// <summary>
    /// The content of the comment, supporting Markdown or HTML formatting.
    /// </summary>
    [MaxLength(4096)]
    public string CommentText { get; set; } = "";

    /// <summary>
    /// The timestamp indicating when the comment was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// A secondary identifier used for internal linking or junction patterns.
    /// </summary>
    public Guid TaskId { get; set; } = Guid.NewGuid();  // Initialize with Id after object creation

}
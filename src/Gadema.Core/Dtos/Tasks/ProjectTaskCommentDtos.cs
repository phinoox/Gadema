// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Tasks;

/// <summary>
/// DTO for creating a task comment.
/// </summary>
public class ProjectTaskCommentCreateDto
{
    /// <summary>
    /// ID of the task this comment belongs to.
    /// </summary>
    [Required]
    public Guid ProjectTaskId { get; set; }

    /// <summary>
    /// Comment text (Markdown/HTML).
    /// </summary>
    [Required, MaxLength(4096)]
    public string CommentText { get; set; } = "";
}

/// <summary>
/// Response DTO for a task comment.
/// </summary>
public class ProjectTaskCommentResponseDto
{
    public Guid Id { get; set; }
    public Guid ProjectTaskId { get; set; }
    public Guid CommentedByUserId { get; set; }
    public string? CommentedByUserName { get; set; }
    public string CommentText { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// List response for task comments.
/// </summary>
public class ProjectTaskCommentListResponseDto
{
    public IEnumerable<ProjectTaskCommentResponseDto> Items { get; set; } = Enumerable.Empty<ProjectTaskCommentResponseDto>();
    public int TotalCount { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Tasks;

/// <summary>
/// DTO for creating a new comment on a task.
/// </summary>
public class ProjectTaskCommentCreateDto
{
    [Required]
    public string CommentText { get; set; } = string.Empty;

    [Required]
    public Guid CommentedByUserId { get; set; }
}

/// <summary>
/// DTO for updating an existing comment.
/// </summary>
public class ProjectTaskCommentUpdateDto
{
    [Required, MaxLength(4096)]
    public string CommentText { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for a task comment.
/// </summary>
public class ProjectTaskCommentResponseDto
{
    public Guid Id { get; set; }
    public Guid ProjectTaskId { get; set; }
    public Guid CommentedByUserId { get; set; }
    public string CommentText { get; set; } = string.Empty;
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
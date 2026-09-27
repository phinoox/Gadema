/// <summary>
/// Data transfer object for creating a new comment on a project task.
/// </summary>
public class ProjectTaskCommentCreateDto
{
    /// <summary>
    /// The actual text content of the comment.
    /// </summary>
    [Required]
    public string CommentText { get; set; } = string.Empty;

    /// <summary>
    /// The unique identifier of the user who authored this comment.
    /// </summary>
    [Required]
    public Guid CommentedByUserId { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing task comment.
/// </summary>
public class ProjectTaskCommentUpdateDto
{
    /// <summary>
    /// The updated text content of the comment.
    /// </summary>
    [Required, MaxLength(4096)]
    public string CommentText { get; set; } = string.Empty;
}

/// <summary>
/// Represents a single comment associated with a project task.
/// </summary>
public class ProjectTaskCommentResponseDto
{
    /// <summary>
    /// The unique identifier of the comment.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The unique identifier of the task this comment belongs to.
    /// </summary>
    public Guid ProjectTaskId { get; set; }

    /// <summary>
    /// The unique identifier of the user who wrote the comment.
    /// </summary>
    public Guid CommentedByUserId { get; set; }

    /// <summary>
    /// The text content of the comment.
    /// </summary>
    public string CommentText { get; set; } = string.Empty;

    /// <summary>
    /// The timestamp when the comment was created in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// A paginated list response for all comments on a task.
/// </summary>
public class ProjectTaskCommentListResponseDto
{
    /// <summary>
    /// The collection of comments retrieved.
    /// </summary>
    public IEnumerable<ProjectTaskCommentResponseDto> Items { get; set; } = Enumerable.Empty<ProjectTaskCommentResponseDto>();

    /// <summary>
    /// Total number of comments available for this task.
    /// </summary>
    public int TotalCount { get; set; }
}
/// <summary>
/// Data transfer object for creating a new social comment.
/// </summary>
public class CreateCommentDto
{
    /// <summary>
    /// The text content of the comment.
    /// </summary>
    [Required] public string CommentText { get; set; } = "";

    /// <summary>
    /// The unique identifier of the entity (e.g., Character, Scene, Task) being commented on.
    /// </summary>
    [Required] public Guid TargetId { get; set; }

    /// <summary>
    /// The ID of a parent comment if this is a reply. Null for top-level comments.
    /// </summary>
    public Guid? ParentCommentId { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing social comment.
/// </summary>
public class UpdateCommentDto
{
    /// <summary>
    /// The updated text content of the comment.
    /// </summary>
    public string? CommentText { get; set; }

    /// <summary>
    /// Updates the parent relationship (e.g., moving a reply to another thread).
    /// </summary>
    public Guid? ParentCommentId { get; set; }
}

/// <summary>
/// Represents a single social comment in the system.
/// </summary>
public class CommentResponseDto
{
    /// <summary>
    /// The unique identifier of the comment.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the target entity this comment is attached to.
    /// </summary>
    public Guid TargetId { get; set; }

    /// <summary>
    /// The text content of the comment.
    /// </summary>
    public string Text { get; set; } = "";

    /// <summary>
    /// The unique identifier of the user who authored the comment.
    /// </summary>
    public Guid AuthorUserId { get; set; }

    /// <summary>
    /// The timestamp when the comment was created in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The ID of the parent comment, if this is a reply.
    /// </summary>
    public Guid? ParentCommentId { get; set; }
}

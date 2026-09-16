using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Writing.Social;

public class CreateCommentDto
{
    [Required] public string CommentText { get; set; } = "";
    [Required] public Guid TargetId { get; set; }
    public Guid? ParentCommentId { get; set; }
}

public class UpdateCommentDto
{
    public string? CommentText { get; set; }
    public Guid? ParentCommentId { get; set; }
}

public class CommentResponseDto
{
    public Guid Id { get; set; }
    public Guid TargetId { get; set; }
    public string Text { get; set; } = "";
    public Guid AuthorUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? ParentCommentId { get; set; }
}

// ListResponseDto stays the same as it is a generic wrapper
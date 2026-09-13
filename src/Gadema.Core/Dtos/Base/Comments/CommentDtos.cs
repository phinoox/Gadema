// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Comments;

/// <summary>
/// DTO for creating a comment on content.
/// </summary>
public class CreateCommentDto
{
    /// <summary>
    /// MetaInfo data for the comment's identity (used as the comment's own record).
    /// </summary>
    [Required] public MetaInfoCreateData CreateData { get; set; } = new();

    /// <summary>
    /// ID of the content item this comment is on.
    /// </summary>
    [Required] public Guid ContentItemId { get; set; }

    /// <summary>
    /// The actual comment text.
    /// </summary>
    [Required, MaxLength(8192)] public string CommentText { get; set; } = "";

    /// <summary>
    /// Visibility: "Private" (owner only), "Team" (project members), "Public" (all users).
    /// Defaults to "Team".
    /// </summary>
    [Display(Name = "Visibility")]
    public CommentVisibilityEnum Visibility { get; set; } = CommentVisibilityEnum.Team;

    /// <summary>
    /// Optional: ID of the parent comment for threaded replies.
    /// Null means this is a top-level comment.
    /// </summary>
    [MaxLength(128)] public Guid? ParentCommentId { get; set; }

    /// <summary>
    /// Optional: mention specific users by their user IDs (comma-separated).
    /// They will be notified of the mention.
    /// </summary>
    [MaxLength(512)] public string? MentionedUserIds { get; set; }
}

/// <summary>
/// DTO for updating a comment (partial update).
/// </summary>
public class UpdateCommentDto : UpdateRequestDto
{
    /// <summary>
    /// MetaInfo fields (nullable — omit to keep current values).
    /// </summary>
    public MetaInfoUpdateData? MetaInfo { get; set; }

    [MaxLength(8192)] public string? CommentText { get; set; }
    public CommentVisibilityEnum? Visibility { get; set; }
}

/// <summary>
/// Response DTO for a single comment. Inherits MetaInfo state.
/// </summary>
public class CommentResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// ID of the content item this comment is on.
    /// </summary>
    [Required] public Guid ContentItemId { get; set; }

    /// <summary>
    /// Title of the content item (denormalized for convenience).
    /// </summary>
    [MaxLength(256)] public string? ContentTypeTitle { get; set; }

    /// <summary>
    /// The actual comment text.
    /// </summary>
    [MaxLength(8192)] public string CommentText { get; set; } = "";

    /// <summary>
    /// Visibility: Private, Team, or Public.
    /// </summary>
    public CommentVisibilityEnum Visibility { get; set; }

    /// <summary>
    /// Whether this is a reply to another comment (true) or top-level (false).
    /// </summary>
    public bool IsReply { get; set; }

    /// <summary>
    /// ID of the parent comment if this is a reply.
    /// </summary>
    [MaxLength(128)] public Guid? ParentCommentId { get; set; }

    /// <summary>
    /// User IDs mentioned in this comment (for notification purposes).
    /// </summary>
    public string[] MentionedUserIds { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Number of replies to this comment.
    /// </summary>
    public int ReplyCount { get; set; }

    /// <summary>
    /// Whether the current user has flagged this as spam or offensive.
    /// </summary>
    public bool IsFlagged { get; set; } = false;

    /// <summary>
    /// Whether the current user has liked this comment.
    /// </summary>
    public bool LikedByCurrentUser { get; set; } = false;
}

/// <summary>
/// List response for comments on a content item.
/// </summary>
public class CommentListResponseDto
{
    /// <summary>
    /// The parent content item ID being commented on.
    /// </summary>
    public Guid ContentItemId { get; set; }

    /// <summary>
    /// Whether to include only a specific visibility level (Private, Team, Public).
    /// If null, return all comments regardless of visibility.
    /// </summary>
    [Display(Name = "Visibility Filter")]
    public string? VisibilityFilter { get; set; }

    public IEnumerable<CommentResponseDto> Items { get; set; } = Enumerable.Empty<CommentResponseDto>();

    public int TotalCount { get; set; }
}

/// <summary>
/// Request DTO for creating a reply to an existing comment.
/// </summary>
public class ReplyCommentDto : CreateCommentDto
{
    /// <summary>
    /// ID of the parent comment being replied to (required for replies).
    /// </summary>
    [Required] public Guid ParentCommentId { get; set; }
}

/// <summary>
/// Enum for comment visibility levels.
/// </summary>
public enum CommentVisibilityEnum : int
{
    Private = 0,   // Only the owner can see this comment
    Team = 1,      // All project members can see it
    Public = 2     // Any logged-in user can see it
}
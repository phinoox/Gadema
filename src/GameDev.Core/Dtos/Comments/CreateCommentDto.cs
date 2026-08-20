// =============================================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
// =============================================================================

namespace GameDev.Core.Dtos.Comments;

/// <summary>
/// DTO for creating a comment on a content item.
/// </summary>
public class CreateCommentDto
{
    /// <summary>
    /// ID of the content item this comment belongs to (required).
    /// </summary>
    [Required]
    public Guid ContentItemId { get; set; }
    
    /// <summary>
    /// Comment text (Markdown/HTML) (required).
    /// </summary>
    [Required, MaxLength(4096), Display(Name = "Comment Text")]
    public string CommentText { get; set; } = "";
    
    /// <summary>
    /// Visibility: private, team-only, public.
    /// </summary>
    [MaxLength(64)]
    public string? Visibility { get; set; } = "private";
}
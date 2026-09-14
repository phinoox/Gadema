using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Models;

/// <summary>
/// Represents ancillary data attached to a content item.
/// </summary>
public class Comment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // The "Anchor" this comment belongs to (e.g., a LoreEntry or Branch)
    [Required] 
    public Guid TargetId { get; set; }

    [Required] 
    public Guid AuthorUserId { get; set; }
    
    [MaxLength(4096)] 
    public string Text { get; set; } = "";
    
    public Guid? ParentCommentId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
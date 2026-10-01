namespace Gadema.Core.Models.Base.MetaInfo;

using System.ComponentModel.DataAnnotations;


 /// <summary>
 /// Represents a user-provided comment attached to a specific content item or entity.
 /// </summary>
 [ModelDependency(typeof(ContentMetaInfo))]
public class Comment
{
    /// <summary>
    /// The unique identifier for this comment, which is also the ID of the target entity (The Soul).
    /// </summary>
    [Key]
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the identifier of the user who authored this comment.
    /// </summary>
    [Required] 
    public Guid AuthorUserId { get; set; }
    
    /// <summary>
    /// The text content of the comment.
    /// </summary>
    [MaxLength(4096)] 
    public string Text { get; set; } = "";
    
    /// <summary>
    /// The ID of the parent comment, if this is a reply in a threaded conversation.
    /// </summary>
    public Guid? ParentCommentId { get; set; }
    
    /// <summary>
    /// Gets or sets the timestamp when the comment was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

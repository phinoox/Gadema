namespace Gadema.Core.Models.Base.MetaInfo;

/// <summary>

/// Represents a user-provided comment attached to a specific content item or entity.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo))]
public class Comment
{
    /// <summary>
    /// Gets or sets the unique identifier for this comment.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    

    /// <summary>
    /// Gets or sets the identifier of the target entity (e.s. a LoreEntry, Scene, etc.) that this comment is attached to.
    /// </summary>
    [Required] 
    public Guid TargetId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user who authored this comment.
    /// </summary>
    [Required] 
    public Guid AuthorUserId { get; set; }
    
    /// <summary>
    /// Gets or sets the text content of the comment.
    /// </summary>
    [MaxLength(4096)] 
    public string Text { get; set; } = "";
    
    /// <summary>
    /// Gets or sets the identifier of the parent comment, if this is a reply in a threaded conversation.
    /// </summary>
    public Guid? ParentCommentId { get; set; }
    
    /// <summary>
    /// Gets or sets the timestamp when the comment was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
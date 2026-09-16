namespace Gadema.Core.Models.Base.Infrastructure;

/// <summary>
/// Junction table for many-to-many relationship between MediaAttachments and Tags.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo))]
public class MediaAttachmentTagRelation
{
    /// <summary>
    /// Unique identifier for the junction record.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// ID of the media attachment this tag is associated with.
    /// </summary>
    [Required, Display(Name = "Media Attachment ID")]
    public Guid MediaAttachmentId { get; set; }

    // Navigation property: MediaAttachment (Many-to-One)
    [ForeignKey("MediaAttachmentId")]
    public virtual MediaAttachment MediaAttachment { get; set; }

    /// <summary>
    /// ID of the tag being applied to the media attachment.
    /// </summary>
    [Required, Display(Name = "Tag ID")]
    public Guid TagId { get; set; }

    // Navigation property: Tag (Many-to-One)
    public virtual ContentMetaInfoTag Tag { get; set; }

}
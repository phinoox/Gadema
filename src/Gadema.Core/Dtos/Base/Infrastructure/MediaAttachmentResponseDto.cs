/// <summary>
/// Represents a media file attached to a content item.
/// </summary>
public class MediaAttachmentResponseDto
{
    /// <summary>
    /// The unique identifier of the attachment record.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// The ID of the meta-information record this media is attached to.
    /// </summary>
    [Required, Display(Name = "Content Item ID")]
    public Guid MetaInfoId { get; set; }
    
    /// <summary>
    /// The name of the file including its extension.
    /// </summary>
    [Required, MaxLength(256)]
    public string FileName { get; set; } = "";
    
    /// <summary>
    /// The MIME type of the file (e.g., "image/png").
    /// </summary>
    [MaxLength(4096)]
    public string ContentType { get; set; } = "application/octet-stream";
    
    /// <summary>
    /// The internal storage path or URL for the file.
    /// </summary>
    [MaxLength(2048)]
    public string StoragePath { get; set; } = "";
    
    /// <summary>
    /// The size of the file in bytes.
    /// </summary>
    [Required, Display(Name = "File Size")]
    public long FileSize { get; set; }
    
    /// <summary>
    /// The ID of the user who uploaded the file.
    /// </summary>
    public Guid UploadedByUserId { get; set; }
    
    /// <summary>
    /// The timestamp when the upload occurred in UTC.
    /// </summary>
    public DateTime UploadedAt { get; set; }
}

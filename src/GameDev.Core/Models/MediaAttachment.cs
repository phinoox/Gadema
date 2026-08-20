// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Represents a media file attached to a content item (images, PDFs, etc.).
/// Supports file upload with storage path tracking.
/// </summary>
public class MediaAttachment
{
    /// <summary>
    /// Unique identifier for the media attachment.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the content item this media is attached to.
    /// </summary>
    [Required, Display(Name = "Content Item ID")]
    public Guid ContentItemId { get; set; }
    
    /// <summary>
    /// Original file name.
    /// </summary>
    [Required, Display(Name = "File Name")]
    public string FileName { get; set; } = "";
    
    /// <summary>
    /// MIME type of the file.
    /// </summary>
    [Display(Name = "Content Type")]
    public string ContentType { get; set; } = "application/octet-stream";
    
    /// <summary>
    /// Storage path for the uploaded file.
    /// </summary>
    [Column("storage_path"), MaxLength(2048)]
    public string StoragePath { get; set; } = "";
    
    /// <summary>
    /// File size in bytes.
    /// </summary>
    [Display(Name = "File Size")]
    public long FileSize { get; set; }  // Bytes
    
    /// <summary>
    /// ID of the user who uploaded this file.
    /// </summary>
    public Guid UploadedByUserId { get; set; }
    
    /// <summary>
    /// Upload timestamp.
    /// </summary>
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
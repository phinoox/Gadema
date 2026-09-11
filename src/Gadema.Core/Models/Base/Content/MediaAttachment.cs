// =============================================================================
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models;

/// <summary>
/// Represents a media file attached to a content item (images, PDFs, etc.).
/// Supports file upload with storage path tracking.
/// </summary>
[ModelDependency(typeof(MetaInfo))]
public class MediaAttachment
{
    /// <summary>
    /// Unique identifier for the media attachment.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // ⭐ FIX: Change MediaAttachmentId to MetaInfoId (FK to MetaInfos)
    /// <summary>
    /// ID of the content item this media is attached to.
    /// </summary>
    [Required, Display(Name = "Content Item ID")]
    public Guid MetaInfoId { get; set; }  // ✅ FIX: Changed from MediaAttachmentId

    // Navigation property: MetaInfo (Many-to-One)
    [ForeignKey("MetaInfoId")]
    public virtual MetaInfo MetaInfo { get; set; }
    
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

    // ⭐ FIX: Add ExternalReferences collection for media attachments
    public ICollection<ExternalReference> ExternalReferences { get; set; }
}
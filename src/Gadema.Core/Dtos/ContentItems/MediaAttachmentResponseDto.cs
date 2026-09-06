// =============================================================================
// MediaAttachmentResponseDto - Response for media attachment
// =============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gadema.Core.Dtos.MetaInfos;

/// <summary>
/// Single media attachment response.
/// </summary>
public class MediaAttachmentResponseDto
{
    public Guid Id { get; set; }
    
    [Required, Display(Name = "Content Item ID")]
    public Guid MetaInfoId { get; set; }
    
    [Required, MaxLength(256)]
    public string FileName { get; set; } = "";
    
    [MaxLength(4096)]
    public string ContentType { get; set; } = "application/octet-stream";
    
    [Column("storage_path"), MaxLength(2048)]
    public string StoragePath { get; set; } = "";
    
    [Required, Display(Name = "File Size")]
    public long FileSize { get; set; }
    
    public Guid UploadedByUserId { get; set; }
    
    public DateTime UploadedAt { get; set; }
}

// =============================================================================
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Junction table for many-to-many relationship between MediaAttachments and Tags.
/// </summary>
public class MediaTags
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
    
    /// <summary>
    /// ID of the tag being applied to the media attachment.
    /// </summary>
    [Required, Display(Name = "Tag ID")]
    public Guid TagId { get; set; }
}
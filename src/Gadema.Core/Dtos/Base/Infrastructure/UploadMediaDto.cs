// =============================================================================
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

// =============================================================================

namespace Gadema.Core.Dtos.Base.Infrastructure;

/// <summary>
/// DTO for uploading media file to content item.
/// </summary>
public class UploadMediaDto
{
    /// <summary>
    /// Media file to upload (max 100MB).
    /// </summary>
    [Required]
    public IFormFile File { get; set; } = null!;
}
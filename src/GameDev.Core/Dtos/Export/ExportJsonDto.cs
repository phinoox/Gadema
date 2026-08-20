// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Dtos.Export;

/// <summary>
/// DTO for exporting project to JSON format.
/// </summary>
public class ExportJsonDto
{
    /// <summary>
    /// ID of the project to export (required).
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// List of content types to export (optional).
    /// </summary>
    [MaxLength(512)]
    public string? Sections { get; set; } = null!;  // JSON array string
    
    /// <summary>
    /// Include watermark for IP protection (optional).
    /// </summary>
    public bool IncludeWatermark { get; set; } = false;
}
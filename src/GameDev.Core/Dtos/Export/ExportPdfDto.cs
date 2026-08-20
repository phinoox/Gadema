// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Dtos.Export;

/// <summary>
/// DTO for exporting project to PDF format (GDD document).
/// </summary>
public class ExportPdfDto
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
    
    /// <summary>
    /// Watermark text (e.g., "© YourCompany") (optional).
    /// </summary>
    [MaxLength(256)]
    public string? WatermarkText { get; set; } = null!;
}
// =============================================================================
using System.ComponentModel.DataAnnotations;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Dtos.Export;

/// <summary>
/// Response DTO for JSON export operation.
/// </summary>
public class ExportJsonResponseDto
{
    /// <summary>
    /// Project ID being exported.
    /// </summary>
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// Generated file name.
    /// </summary>
    [MaxLength(256)]
    public string FileName { get; set; } = "";
    
    /// <summary>
    /// MIME type of the exported content.
    /// </summary>
    public string ContentType { get; set; } = "application/json";
}

/// <summary>
/// Response DTO for CSV export operation.
/// </summary>
public class ExportCsvResponseDto
{
    /// <summary>
    /// Project ID being exported.
    /// </summary>
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// Generated file name.
    /// </summary>
    [MaxLength(256)]
    public string FileName { get; set; } = "";
    
    /// <summary>
    /// MIME type of the exported content.
    /// </summary>
    public string ContentType { get; set; } = "text/csv";
}

/// <summary>
/// Response DTO for XML GDD export operation.
/// </summary>
public class ExportXmlGddResponseDto
{
    /// <summary>
    /// Project ID being exported.
    /// </summary>
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// Generated file name.
    /// </summary>
    [MaxLength(256)]
    public string FileName { get; set; } = "";
    
    /// <summary>
    /// MIME type of the exported content.
    /// </summary>
    public string ContentType { get; set; } = "application/xml";
}

/// <summary>
/// Response DTO for PDF export operation.
/// </summary>
public class ExportPdfResponseDto
{
    /// <summary>
    /// Project ID being exported.
    /// </summary>
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// Generated file name.
    /// </summary>
    [MaxLength(256)]
    public string FileName { get; set; } = "";
    
    /// <summary>
    /// MIME type of the exported content.
    /// </summary>
    public string ContentType { get; set; } = "application/pdf";
}
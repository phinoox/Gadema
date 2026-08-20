// =============================================================================
using System.ComponentModel.DataAnnotations;
using GameDev.Core.Enums;
using System.ComponentModel.DataAnnotations;
// =============================================================================

namespace GameDev.Core.Dtos.Export;

/// <summary>
/// DTO for exporting project to XML GDD format (Unreal compatible).
/// </summary>
public class ExportXmlGddDto
{
    /// <summary>
    /// ID of the project to export (required).
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// Content type to export (e.g., Character) (required).
    /// </summary>
    [EnumDataType(typeof(ContentTypeEnum)), Required, Display(Name = "Content Type")]
    public ContentTypeEnum ContentType { get; set; }
    
    /// <summary>
    /// Include watermark for IP protection (optional).
    /// </summary>
    public bool IncludeWatermark { get; set; } = false;
}
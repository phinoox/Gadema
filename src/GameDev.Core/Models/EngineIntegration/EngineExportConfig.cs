// =============================================================================
// EngineExportConfig - Entity for engine-specific export configurations
// =============================================================================

using GameDev.Core.Enums;
using GameDev.Core.Models.Projects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameDev.Core.Models;

/// <summary>
/// Configuration settings for exporting content items to game engines.
/// Stores engine type, format, and field mapping settings for each project.
/// </summary>
public class EngineExportConfig
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }

    // Navigation property for Project (Many-to-One)
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }
    
    public int EngineType { get; set; }  // Enum: Unity(0), Unreal(1), Both(2)
    
    [EnumDataType(typeof(ExportFormatEnum)), Required, Display(Name = "Export Format")]
    public ExportFormatEnum ExportFormat { get; set; }
    
    public bool IsDefaultConfig { get; set; } = true;
    
    [MaxLength(4096)]
    public string? FieldMappingsJson { get; set; }  // JSON for custom field mappings
    
    [MaxLength(1024)]
    public string? Description { get; set; }

    public bool IsEnabled { get;set;}

}

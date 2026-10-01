using System.ComponentModel.DataAnnotations;
using Gadema.Core.Models.Base.Projects;

namespace Gadema.Core.Models.Game.EngineIntegration;

/// <summary>
/// Defines the configuration settings for exporting project content to external game engines.
/// This entity stores the target engine type, export format, and activation status.
/// </summary>
[ModelDependency(typeof(Project))]
public class EngineExportConfig
{
    /// <summary>
    /// Unique identifier for the export configuration.
    /// </summary>
    [Key]
    public Guid Id { get; set; }
    
    /// <summary>
    /// The ID of the project this configuration belongs to.
    /// </summary>
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Navigation property for the parent project.
    /// </summary>
    // Navigation property for Project (Many-to-One)
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; } = null!;
    
    /// <summary>
    /// The target game engine (e.g., Unity, Unreal).
    /// </summary>
    public int EngineType { get; set; }  // Enum: Unity(0), Unreal(1), Both(2)
    
    /// <summary>
    /// The format used for the exported data (e.g., JSON, CSV).
    /// </summary>
    [EnumDataType(typeof(ExportFormatEnum)), Required, Display(Name = "Export Format")]
    public ExportFormatEnum ExportFormat { get; set; }
    
    /// <summary>
    /// Indicates if this is the default configuration for the project.
    /// </summary>
    public bool IsDefaultConfig { get; set; } = true;
    
    /// <summary>
    /// A JSON representation of custom field mappings that may not be explicitly defined in <see cref="EngineFieldMapping"/>.
    /// </summary>
    [MaxLength(4096)]
    public string? FieldMappingsJson { get; set; }  // JSON for custom field mappings
    
    /// <summary>
    /// A detailed description of the export configuration's purpose or specific settings.
    /// </summary>
    [MaxLength(1024)]
    public string? Description { get; set; }

    /// <summary>
    /// Indicates whether this export configuration is currently active and ready for use.
    /// </summary>
    public bool IsEnabled { get; set; }
}

// =============================================================================
// EngineFieldMapping - Entity for engine field name mappings per content type
// =============================================================================

using Gadema.Core.Models.Base.Projects;

namespace Gadema.Core.Models.Game.EngineIntegration;

/// <summary>
/// Maps content fields to their corresponding engine property names.
/// Example: ContentMetaInfo.Title -> "Player.Name" in Unreal Engine.
/// </summary>
[ModelDependency(typeof(EngineExportConfig))]
public class EngineFieldMapping
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }

    // Navigation property for Project (Many-to-One)
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }

    [EnumDataType(typeof(ContentTypeEnum))]
    public ContentTypeEnum ContentType { get; set; }

    [MaxLength(128), Required, Display(Name = "Content Field Name")]
    public string ContentFieldName { get; set; } = "";

    [MaxLength(256)]
    public string? EngineFieldName { get; set; }  // e.g., "Player.Health"

    public bool IsRequired { get; set; } = false;

    public int DataType { get; set; }  // Nullable: Int, Float, String, Boolean

    [MaxLength(1024)]
    public string? Description { get; set; }

    // Add these at the end of the class:

    /// <summary>
    /// FK to EngineExportConfig.
    /// </summary>
    [Required, Display(Name = "Engine Export Config ID")]
    public Guid EngineExportConfigId { get; set; }

    /// <summary>
    /// Source column name from game engine.
    /// </summary>
    [MaxLength(256)]
    public string? SourceColumn { get; set; } = null!;

    /// <summary>
    /// Target column name in database.
    /// </summary>
    [MaxLength(256)]
    public string? TargetColumn { get; set; } = null!;

    // Navigation: EngineExportConfig
    [ForeignKey("EngineExportConfigId")]
    public virtual EngineExportConfig EngineExportConfig { get; set; }

}

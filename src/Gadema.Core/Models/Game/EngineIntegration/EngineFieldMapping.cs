// =============================================================================
// EngineFieldMapping - Entity for engine field name mappings per content type
// =============================================================================

using Gadema.Core.Models.Base.Projects;

namespace Gadema.Core.Models.Game.EngineIntegration;

/// <summary>
/// Maps internal content fields to their corresponding property names in a game engine.
/// This facilitates the automated export of narrative data (e.g., Character Names, Item Stats)
/// into engines like Unity or Unreal.
/// </summary>
[ModelDependency(typeof(EngineExportConfig))]
public class EngineFieldMapping
{
    /// <summary>
    /// Unique identifier for this mapping entry.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary>
    /// The ID of the project this mapping applies to.
    /// </summary>
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }
    /// <summary>
    /// Navigation property for the parent project.
    /// </summary>
    // Navigation property for Project (Many-to-One)
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }

    /// <summary>
    /// The type of content being mapped (e.g., Character, Item, Location).
    /// </summary>
    [EnumDataType(typeof(ContentTypeEnum))]
    public ContentTypeEnum ContentType { get; set; }

    /// <summary>
    /// The name of the field within Gadema's internal models (e.g., "Name", "Health").
    /// </summary>
    [MaxLength(128), Required, Display(Name = "Content Field Name")]
    public string ContentFieldName { get; set; } = "";

    /// <summary>
    /// The corresponding property name within the target game engine (e.g., "Player.Health").
    /// </summary>
    [MaxLength(256)]
    public string? EngineFieldName { get; set; }  // e.g., "Player.Health"

    /// <summary>
    /// Indicates if this field is mandatory for a successful export.
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// The underlying data type of the value being mapped (e.g., Int, Float, String).
    /// </summary>
    public int DataType { get; set; }  // Nullable: Int, Float, String, Boolean

    /// <summary>
    /// A description of the mapping's purpose or specific logic applied.
    /// </summary>
    [MaxLength(1024)]
    public string? Description { get; set; }

    /// <summary>
    /// The ID of the associated engine export configuration.
    /// </summary>
    [Required, Display(Name = "Engine Export Config ID")]
    public Guid EngineExportConfigId { get; set; }

    /// <summary>
    /// The source column name used in the game engine's data schema.
    /// </summary>
    [MaxLength(256)]
    public string? SourceColumn { get; set; } = null!;

    /// <summary>
    /// The target column name for the exported dataset.
    /// </summary>
    [MaxLength(256)]
    public string? TargetColumn { get; set; } = null!;

    /// <summary>
    /// Navigation property for the engine export configuration.
    /// </summary>
    [ForeignKey("EngineExportConfigId")]
    public virtual EngineExportConfig EngineExportConfig { get; set; }
}


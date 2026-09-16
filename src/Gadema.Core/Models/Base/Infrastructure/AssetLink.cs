namespace Gadema.Core.Models.Base.Infrastructure;

/// <summary>
/// Links content items to engine asset systems.
/// Used for tracking exported assets and cross-referencing with game engines.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo))]
public class AssetLink
{
    /// <summary>
    /// Unique identifier for the asset link.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// FK to the content item this asset belongs to (Primary Key).
    /// </summary>
    [Required, Display(Name = "Content Item")]
    public Guid MetaInfoId { get; set; }

    // Navigation property for ContentMetaInfo (Many-to-One)
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; }

    /// <summary>
    /// Path in the game engine's asset directory.
    /// </summary>
    [MaxLength(2048)]
    public string EnginePath { get; set; } = "";

    /// <summary>
    /// Optional asset ID from the target game engine (cross-reference).
    /// Used for tracking changes when importing/exporting between systems.
    /// </summary>
    [MaxLength(128)]
    public string? EngineAssetId { get; set; }

    /// <summary>
    /// File type in the engine (e.g., "fbx", "uasset", "unitypackage").
    /// </summary>
    [MaxLength(64)]
    public string EngineFileType { get; set; } = "fbx";


}
// =============================================================================
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models;

/// <summary>
/// Links content items to engine asset systems.
/// Used for tracking exported assets and cross-referencing with game engines.
/// </summary>
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
    public Guid ContentItemId { get; set; }

    // Navigation property for ContentItem (Many-to-One)
    [ForeignKey("ContentItemId")]
    public virtual ContentItem ContentItem { get; set; }

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
// =============================================================================

using Gadema.Core.Models.Narrative;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models;

/// <summary>
/// Represents a content segment (unique token marker) within a Scene.
/// Acts as an index for shortcodes like [dialog:123] without storing text indices.
/// Provides robustness against text edits while enabling Hyperfocus Mode and Auto-Parse.
/// </summary>
[ModelDependency(typeof(Scene))]
public class ContentSegment
{
    /// <summary>
    /// Unique identifier used as the token in shortcodes (e.g., [dialog:Id]).
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// ID of the scene this segment belongs to.
    /// </summary>
    [Required]
    public Guid SceneId { get; set; }

    // Navigation property: Scene (Many-to-One)
    [ForeignKey("SceneId")]
    public virtual Scene Scene { get; set; } = null!;

    /// <summary>
    /// Type of segment (Dialogue, StoryBeat, Custom).
    /// </summary>
    [Required]
    public SegmentType Type { get; set; } = SegmentType.Dialogue;

    /// <summary>
    /// Optional metadata JSON for this segment (e.g., speaker, choices, triggers).
    /// </summary>
    [MaxLength(4096)]
    public string? MetadataJson { get; set; }

    /// <summary>
    /// Timestamp when this segment was created or last modified.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when this segment was last synced with the parent scene's RawText.
    /// </summary>
    public DateTime? LastSyncedAt { get; set; }
}

/// <summary>
/// Enum for content segment types.
/// </summary>
public enum SegmentType
{
    /// <summary>Interactive dialogue node</summary>
    Dialogue = 0,
    
    /// <summary>Linked story beat marker</summary>
    StoryBeat = 1,
    
    /// <summary>Custom user-defined segment</summary>
    Custom = 2
}

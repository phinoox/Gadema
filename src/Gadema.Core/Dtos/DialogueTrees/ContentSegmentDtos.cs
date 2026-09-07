// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.DialogueTrees;

/// <summary>
/// DTO for creating a content segment within a scene.
/// </summary>
public class ContentSegmentCreateDto
{
    /// <summary>
    /// ID of the scene this segment belongs to.
    /// </summary>
    [Required]
    public Guid SceneId { get; set; }

    /// <summary>
    /// Type of segment (Dialogue, StoryBeat, Custom).
    /// </summary>
    [Required]
    public int Type { get; set; }

    /// <summary>
    /// Optional metadata JSON for this segment.
    /// </summary>
    [MaxLength(4096)]
    public string? MetadataJson { get; set; }
}

/// <summary>
/// Response DTO for a content segment.
/// </summary>
public class ContentSegmentResponseDto
{
    public Guid Id { get; set; }
    public Guid SceneId { get; set; }
    public int Type { get; set; }
    public string? MetadataJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastSyncedAt { get; set; }
}

/// <summary>
/// List response for content segments.
/// </summary>
public class ContentSegmentListResponseDto
{
    public IEnumerable<ContentSegmentResponseDto> Items { get; set; } = Enumerable.Empty<ContentSegmentResponseDto>();
    public int TotalCount { get; set; }
}

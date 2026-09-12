// =============================================================================
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Models.Writing;

namespace Gadema.Core.Dtos.DialogueTrees;



public class SceneSegmentCreateDto
{
    [Required] public Guid SceneId { get; set; }

    [Required] public SegmentType Type { get; set; }

    public int? OrderIndex { get; set; }

    [MaxLength(4096)] public string? MetadataJson { get; set; }
}

/// <summary>
/// DTO for creating a content segment within a scene.
/// </summary>
public class SceneSegmentUpdateDto
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
    public SegmentType Type { get; set; }

    /// <summary>
    /// Optional metadata JSON for this segment.
    /// </summary>
    [MaxLength(4096)]
    public string? MetadataJson { get; set; }
}

/// <summary>
/// Response DTO for a content segment.
/// </summary>
public class SceneSegmentResponseDto
{
    public Guid Id { get; set; }
    public Guid SceneId { get; set; }
    public SegmentType Type { get; set; }
    public string? MetadataJson { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastSyncedAt { get; set; }
}

/// <summary>
/// List response for content segments.
/// </summary>
public class SceneSegmentListResponseDto
{
    public IEnumerable<SceneSegmentResponseDto> Items { get; set; } = Enumerable.Empty<SceneSegmentResponseDto>();
    public int TotalCount { get; set; }
}

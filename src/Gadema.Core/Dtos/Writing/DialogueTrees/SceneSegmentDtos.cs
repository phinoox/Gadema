// =============================================================================
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Models.Writing;
using Gadema.Core.Dtos; // For MetaInfoCreateData and MetaInfoUpdateData

namespace Gadema.Core.Dtos.Writing.DialogueTrees;

/// <summary>
/// DTO for creating a scene segment.
/// </summary>
public class SceneSegmentCreateDto
{
    [Required] public Guid SceneId { get; set; }

    /// <summary>
    /// The nested identity payload.
    /// </summary>
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();

    [Required] public SegmentType Type { get; set; }

    public int OrderIndex { get; set; }

    public int Position { get; set; }

    public string? Value { get; set; }
}

/// <summary>
/// DTO for updating a scene segment.
/// </summary>
public class SceneSegmentUpdateDto
{
    [Required] public Guid SceneId { get; set; }

    /// <summary>
    /// The nested identity payload for the sync strategy.
    /// </summary>
    public ContentMetaInfoUpdateData? ContentMetaInfo { get; set; }

    public SegmentType? Type { get; set; }
    public int? OrderIndex { get; set; }
    public int? Position { get; set; }
    public string? Value { get; set; }
}

/// <summary>
/// Response DTO for a scene segment.
/// </summary>
public class SceneSegmentResponseDto
{
    public Guid Id { get; set; }
    public Guid SceneId { get; set; }
    
    // Denormalized identity properties from the anchor
    public string Title { get; set; } = "";
    public string Slug { get; set; } = "";
    public bool IsPublic { get; set; }

    public SegmentType Type { get; set; }
    public int OrderIndex { get; set; }
    public int Position { get; set; }
    public string? Value { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// List response for scene segments.
/// </summary>
public class SceneSegmentListResponseDto
{
    public IEnumerable<SceneSegmentResponseDto> Items { get; set; } = Enumerable.Empty<SceneSegmentResponseDto>();
    public int TotalCount { get; set; }
}
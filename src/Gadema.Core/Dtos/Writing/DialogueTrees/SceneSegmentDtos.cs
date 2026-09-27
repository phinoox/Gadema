using Gadema.Core.Models.Writing.Narrative;


/// <summary>
/// Data transfer object for creating a new scene segment.
/// </summary>
public class SceneSegmentCreateDto
{
    /// <summary>
    /// The unique identifier of the scene this segment belongs to.
    /// </summary>
    [Required] public Guid SceneId { get; set; }

    /// <summary>
    /// The metadata required to establish the segment's identity (e.g., Title, Slug).
    /// </summary>
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();

    /// <summary>
    /// The type of segment (e.g., Dialogue, Action, Description).
    /// </summary>
    [Required] public SegmentType Type { get; set; }

    /// <summary>
    /// The sort order index for this segment within the scene.
    /// </summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// The chronological position of this segment in the narrative sequence.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// For dialogue segments, the actual text content spoken or described.
    /// </summary>
    public string? Value { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing scene segment.
/// </summary>
public class SceneSegmentUpdateDto
{
    /// <summary>
    /// The unique identifier of the scene this segment belongs to.
    /// </summary>
    [Required] public Guid SceneId { get; set; }

    /// <summary>
    /// The identity payload used by the sync strategy to update meta-information (e.g., Title, Slug).
    /// </summary>
    public ContentMetaInfoUpdateData? ContentMetaInfo { get; set; }

    /// <summary>
    /// The updated segment type.
    /// </summary>
    public SegmentType? Type { get; set; }

    /// <summary>
    /// The updated sort order index.
    /// </summary>
    public int? OrderIndex { get; set; }

    /// <summary>
    /// The updated chronological position.
    /// </summary>
    public int? Position { get; set; }

    /// <summary>
    /// The updated content value.
    /// </summary>
    public string? Value { get; set; }
}

/// <summary>
/// Represents a segment within a scene, including its identity and narrative position.
/// </summary>
public class SceneSegmentResponseDto
{
    /// <summary>
    /// The unique identifier of the segment.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the parent scene.
    /// </summary>
    public Guid SceneId { get; set; }
    
    // Denormalized identity properties from the anchor
    /// <summary>
    /// The title of the segment.
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// The URL-friendly slug for the segment.
    /// </summary>
    public string Slug { get; set; } = "";

    /// <summary>
    /// Indicates if this segment is public.
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// The type of segment (e.g., Dialogue, Action).
    /// </summary>
    public SegmentType Type { get; set; }

    /// <summary>
    /// The sort order index for this segment.
    /// </summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// The chronological position of the segment in the scene.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// The text content or value associated with this segment.
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// The timestamp when the segment was created in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// A collection of scene segments, typically used for paginated lists.
/// </summary>
public class SceneSegmentListResponseDto
{
    /// <summary>
    /// The list of retrieved scene segments.
    /// </summary>
    public IEnumerable<SceneSegmentResponseDto> Items { get; set; } = Enumerable.Empty<SceneSegmentResponseDto>();

    /// <summary>
    /// Total number of segments found across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}
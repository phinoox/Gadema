// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Narrative;

/// <summary>
/// DTO for creating a story beat.
/// </summary>
public class StoryBeatCreateDto
{
    /// <summary>
    /// ID of the StoryOutline this beat belongs to.
    /// </summary>
    [Required]
    public Guid StoryOutlineId { get; set; }

    /// <summary>
    /// Title of the beat.
    /// </summary>
    [Required, MaxLength(128)]
    public string BeatTitle { get; set; } = "";

    /// <summary>
    /// URL-friendly slug.
    /// </summary>
    [MaxLength(128)]
    public string? Slug { get; set; }

    /// <summary>
    /// Description of the beat's narrative intent.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }

    /// <summary>
    /// Order index for sorting beats in the Timeline/Map views.
    /// </summary>
    public int OrderIndex { get; set; } = 0;

    /// <summary>
    /// Indicates if the beat is published/finalized.
    /// </summary>
    public bool Published { get; set; } = false;
}

/// <summary>
/// Response DTO for a story beat.
/// </summary>
public class StoryBeatResponseDto
{
    public Guid Id { get; set; }
    public Guid StoryOutlineId { get; set; }
    public string BeatTitle { get; set; } = "";
    public string Slug { get; set; } = "";
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public bool Published { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
}

/// <summary>
/// List response for story beats.
/// </summary>
public class StoryBeatListResponseDto
{
    public IEnumerable<StoryBeatResponseDto> Items { get; set; } = Enumerable.Empty<StoryBeatResponseDto>();
    public int TotalCount { get; set; }
}

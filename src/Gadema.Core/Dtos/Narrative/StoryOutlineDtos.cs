// =============================================================================
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Enums;

namespace Gadema.Core.Dtos.Narrative;

/// <summary>
/// DTO for creating a story outline.
/// </summary>
public class StoryOutlineCreateDto
{
    /// <summary>
    /// ID of the project this outline belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Raw text content (Markdown).
    /// </summary>
    [Required]
    public string RawText { get; set; } = "";

    /// <summary>
    /// Summary of the outline section.
    /// </summary>
    [MaxLength(4096)]
    public string? Summary { get; set; }

    /// <summary>
    /// Snapshot of character state at this point in the story.
    /// </summary>
    [MaxLength(512)]
    public string? CharacterSnapshot { get; set; }

    /// <summary>
    /// Theme statement for this section.
    /// </summary>
    [MaxLength(1024)]
    public string? ThemeStatement { get; set; }

    /// <summary>
    /// Outline status (DraftOutline, Finalized, Published).
    /// </summary>
    public OutlineStatusEnum OutlineStatus { get; set; } = OutlineStatusEnum.DraftOutline;
}

/// <summary>
/// Response DTO for a story outline.
/// </summary>
public class StoryOutlineResponseDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string RawText { get; set; } = "";
    public string? Summary { get; set; }
    public string? CharacterSnapshot { get; set; }
    public string? ThemeStatement { get; set; }
    public OutlineStatusEnum OutlineStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
}

/// <summary>
/// List response for story outlines.
/// </summary>
public class StoryOutlineListResponseDto
{
    public IEnumerable<StoryOutlineResponseDto> Items { get; set; } = Enumerable.Empty<StoryOutlineResponseDto>();
    public int TotalCount { get; set; }
}

using Gadema.Core.Dtos.Base.Infrastructure;

namespace Gadema.Core.Dtos.Writing.Narrative;

/// <summary>
/// Data transfer object for creating a new section within a story outline.
/// </summary>
public class OutlineSectionCreateDto
{
    /// <summary>
    /// The unique identifier of the parent story outline.
    /// </summary>
    [Required] public Guid StoryOutlineId { get; set; }

    /// <summary>
    /// The title of the section.
    /// </summary>
    [Required] public string Title { get; set; } = "";

    /// <summary>
    /// The raw text content or summary of this section.
    /// </summary>
    public string RawText { get; set; } = "";

    /// <summary>
    /// The sort order for this section within the outline hierarchy.
    /// </summary>
    public int? SortOrder { get; set; }

    /// <summary>
    /// List of beat identifiers to associate with this section.
    /// </summary>
    public List<Guid>? LinkedBeatIds { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing outline section.
/// </summary>
public class OutlineSectionUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// The unique identifier of the parent story outline.
    /// </summary>
    [Required] public Guid StoryOutlineId { get; set; }

    /// <summary>
    /// The updated title of the section.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// The updated raw text content.
    /// </summary>
    public string? RawText { get; set; }

    /// <summary>
    /// The updated sort order for this section.
    /// </summary>
    public int? SortOrder { get; set; }

    /// <summary>
    /// The updated list of linked beat identifiers.
    /// </summary>
    public List<Guid>? LinkedBeatIds { get; set; }
}

/// <summary>
/// Represents a section within a story's outline, containing its content and hierarchy context.
/// </summary>
public class OutlineSectionResponseDto
{
    /// <summary>
    /// The unique identifier of the section.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the parent story outline.
    /// </summary>
    public Guid StoryOutlineId { get; set; }

    /// <summary>
    /// The title of the section.
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// The raw text content of the section.
    /// </summary>
    public string RawText { get; set; } = "";

    /// <summary>
    /// The sort order index for this section.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// The list of beat IDs linked to this section.
    /// </summary>
    public List<Guid> LinkedBeatIds { get; set; } = new();
}

/// <summary>
/// A collection of outline sections, typically used for paginated lists.
/// </summary>
public class OutlineSectionListResponseDto
{
    /// <summary>
    /// The list of retrieved outline sections.
    /// </summary>
    public IEnumerable<OutlineSectionResponseDto> Items { get; set; } = Enumerable.Empty<OutlineSectionResponseDto>();

    /// <summary>
    /// Total number of sections found across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}
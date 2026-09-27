using Gadema.Core.Dtos.Base.Infrastructure;

namespace Gadema.Core.Dtos.Writing.Narrative;

/// <summary>
/// Data transfer object for creating a new story.
/// </summary>
public class StoryCreateDto
{
    /// <summary>
    /// The metadata required to establish the story's identity (e.g., Title, Slug).
    /// </summary>
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();

    /// <summary>
    /// A high-level description of the story.
    /// </summary>
    [MaxLength(4096)] public string? Description { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing story.
/// </summary>
public class StoryUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// The identity payload used by the sync strategy to update meta-information.
    /// </summary>
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    /// <summary>
    /// The updated description of the story.
    /// </summary>
    [MaxLength(4096)] public string? Description { get; set; }
}

/// <summary>
/// Represents a story, including its identity and core description.
/// </summary>
public class StoryResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// A high-level description of the story.
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// A collection of stories, typically used for paginated lists.
/// </summary>
public class StoryListResponseDto
{
    /// <summary>
    /// The list of retrieved stories.
    /// </summary>
    public IEnumerable<StoryResponseDto> Items { get; set; } = Enumerable.Empty<StoryResponseDto>();

    /// <summary>
    /// Total number of stories found across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}
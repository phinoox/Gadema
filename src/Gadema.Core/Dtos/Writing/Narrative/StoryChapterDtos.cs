using Gadema.Core.Dtos.Base.Infrastructure;

namespace Gadema.Core.Dtos.Writing.Narrative;

/// <summary>
/// Data transfer object for creating a new story chapter.
/// </summary>
public class StoryChapterCreateDto
{
    /// <summary>
        /// The metadata required to establish the chapter's identity.
    /// </summary>
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();

    /// <summary>
    /// The unique identifier of the parent story this chapter belongs to.
    /// </summary>
    [Required] public Guid StoryId { get; set; }

    /// <summary>
    /// A description of the chapter's narrative content.
    /// </summary>
    [MaxLength(4096)] public string? Description { get; set; }

    /// <summary>
    /// The sort order index for this chapter within the story.
    /// </summary>
    public int? OrderIndex { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing story chapter.
/// </summary>
public class StoryChapterUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// The identity payload used by the sync strategy to update meta-information.
    /// </summary>
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    /// <summary>
    /// The unique identifier of the parent story.
    /// </summary>
    [Required] public Guid StoryId { get; set; }

    /// <summary>
    /// The updated description of the chapter.
    /// </summary>
    [MaxLength(4096)] public string? Description { get; set; }

    /// <summary>
    /// The updated sort order index for this chapter.
    /// </summary>
    public int? OrderIndex { get; set; }
}

/// <summary>
/// Represents a story chapter, including its identity and position within the story.
/// </summary>
public class StoryChapterResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// The unique identifier of the parent story.
    /// </summary>
    public Guid StoryId { get; set; }

    /// <summary>
    /// A description of the chapter's content.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The sort order index for this chapter within the story sequence.
    /// </summary>
    public int OrderIndex { get; set; }
}

/// <summary>
/// A collection of story chapters, typically used for paginated lists.
/// </summary>
public class StoryChapterListResponseDto
{
    /// <summary>
    /// The list of retrieved story chapters.
        /// </summary>
    public IEnumerable<StoryChapterResponseDto> Items { get; set; } = Enumerable.Empty<StoryChapterResponseDto>();

    /// <summary>
    /// Total number of chapters found across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}
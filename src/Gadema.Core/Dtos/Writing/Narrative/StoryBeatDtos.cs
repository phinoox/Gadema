using Gadema.Core.Dtos.Base.Infrastructure;

namespace Gadema.Core.Dtos.Writing.Narrative;

/// <summary>
/// Data transfer object for creating a new story beat within a story.
/// </summary>
public class StoryBeatCreateDto
{
/// <summary>
    /// The metadata required to establish the beat's identity.
/// </summary>
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();
    /// <summary>
    /// The unique identifier of the story this beat belongs to.
    /// </summary>
    [Required] public Guid StoryId { get; set; }
    /// <summary>
    /// A description of the narrative intent or action for this beat.
    /// </summary>
    [MaxLength(4096)] public string? Description { get; set; }

    /// <summary>
    /// The sort order index for this beat within its story context.
    /// </summary>
    public int? OrderIndex { get; set; }
}

/// <summary>
/// Represents a single beat in the narrative structure, including its identity and position.
/// </summary>
public class StoryBeatResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// The unique identifier of the story this beat belongs to.
    /// </summary>
    public Guid StoryId { get; set; }

    /// <summary>
    /// A description of the beat's narrative intent.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The sort order index for this beat.
    /// </summary>
    public int OrderIndex { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing story beat.
/// </summary>
public class StoryBeatUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// ContentMetaInfo fields (nullable — omit to keep current).
    /// </summary>
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    /// <summary>
    /// Description of the beat's narrative intent.
    /// </summary>
    [MaxLength(4096)] public string? Description { get; set; }

    /// <summary>
    /// Order index for sorting beats in the Timeline/Map views.
    /// </summary>
    public int? OrderIndex { get; set; }
}

/// <summary>
/// List response for story beats.
/// </summary>
public class StoryBeatListResponseDto
{
    public IEnumerable<StoryBeatResponseDto> Items { get; set; } = Enumerable.Empty<StoryBeatResponseDto>();
    public int TotalCount { get; set; }
}

using Gadema.Core.Dtos.Base.Infrastructure;

namespace Gadema.Core.Dtos.Writing.Narrative;


/// <summary>
/// Data transfer object for creating a new story outline.
/// </summary>
public class StoryOutlineCreateDto
{
    /// <summary>
    /// The metadata required to establish the outline's identity.
    /// </summary>
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();

    /// <summary>
    /// The unique identifier of the parent story this outline belongs to.
    /// </summary>
    [Required] public Guid StoryId { get; set; }   // ← NEW: explicit Story link

    /// <summary>
    /// A high-level summary of the story's overall progression.
    /// </summary>
    [MaxLength(4096)] public string? Summary { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing story outline.
/// </summary>
public class StoryOutlineUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// The identity payload used by the sync strategy to update meta-information.
    /// </summary>
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    /// <summary>
    /// The updated summary of the outline.
    /// </summary>
    [MaxLength(4096)] public string? Summary { get; set; }
}

/// <summary>
/// Represents a story outline, including its identity and structural summary.
/// </summary>
public class StoryOutlineResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// The unique identifier of the parent story.
    /// </summary>
    public Guid StoryId { get; set; }

    /// <summary>
    /// A summary describing the overall narrative structure of the story.
    /// </summary>
    public string? Summary { get; set; }
}

/// <summary>
/// A collection of story outlines, typically used for paginated lists.
/// </summary>
public class StoryOutlineListResponseDto
{
    /// <summary>
    /// The list of retrieved story outlines.
    /// </summary>
    public IEnumerable<StoryOutlineResponseDto> Items { get; set; } = Enumerable.Empty<StoryOutlineResponseDto>();

    /// <summary>
    /// Total number of outlines found across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}
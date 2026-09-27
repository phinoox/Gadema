using Gadema.Core.Dtos.Base.Infrastructure;

namespace Gadema.Core.Dtos.Writing.Narrative;

/// <summary>
/// Data transfer object for creating a new scene within a story chapter.
/// </summary>
public class SceneCreateDto
{
    /// <summary>
    /// The metadata required to establish the scene's identity.
    /// </summary>
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();

    /// <summary>
    /// The unique identifier of the parent story chapter.
    /// </summary>
    [Required] public Guid StoryChapterId { get; set; }

    /// <summary>
    /// The raw text content or description for this scene.
    /// </summary>
    [MaxLength(4096)] public string? RawText { get; set; }

    /// <summary>
    /// List of beat identifiers to associate with this scene.
    /// </summary>
    public List<Guid>? LinkedBeatIds { get; set; }

    /// <summary>
    /// The sort order index for this scene within the chapter.
    /// </summary>
    public int? OrderIndex { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing scene.
/// </summary>
public class SceneUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// The identity payload used by the sync strategy to update meta-information.
    /// </summary>
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    /// <summary>
    /// The unique identifier of the parent story chapter.
    /// </summary>
    [Required] public Guid StoryChapterId { get; set; }

    /// <summary>
    /// The updated raw text content.
    /// </summary>
    [MaxLength(4096)] public string? RawText { get; set; }

    /// <summary>
    /// The updated list of linked beat identifiers.
    /// </summary>
    public List<Guid>? LinkedBeatIds { get; set; }

    /// <summary>
    /// The updated sort order index.
    /// </summary>
    public int? OrderIndex { get; set; }
}

/// <summary>
/// Represents a scene, denormalizing properties from its MetaInfo anchor and narrative component.
/// </summary>
public class SceneResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// The unique identifier of the parent story chapter.
    /// </summary>
    public Guid StoryChapterId { get; set; }

    /// <summary>
    /// The raw text content of the scene.
    /// </summary>
    public string RawText { get; set; } = "";

    /// <summary>
    /// The list of beat IDs linked to this scene.
    /// </summary>
    public List<Guid> LinkedBeatIds { get; set; } = new();

    /// <summary>
    /// The sort order index for this scene within the chapter.
    /// </summary>
    public int? OrderIndex { get; set; }
}

/// <summary>
/// A collection of scenes, typically used for paginated lists.
/// </summary>
public class SceneListResponseDto
{
    /// <summary>
    /// The list of retrieved scenes.
    /// </summary>
    public IEnumerable<SceneResponseDto> Items { get; set; } = Enumerable.Empty<SceneResponseDto>();

    /// <summary>
    /// Total number of scenes found across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}
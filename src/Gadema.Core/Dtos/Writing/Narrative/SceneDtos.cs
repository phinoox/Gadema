using Gadema.Core.Dtos.Base.Infrastructure;

namespace Gadema.Core.Dtos.Writing.Narrative;

public class SceneCreateDto
{
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();

    [Required] public Guid StoryChapterId { get; set; }   // ← CHANGED from StoryOutlineId

    [MaxLength(4096)] public string? RawText { get; set; }

    public List<Guid>? LinkedBeatIds { get; set; }
    public int? OrderIndex { get; set; }
}

public class SceneUpdateDto : UpdateRequestDto
{
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    [Required] public Guid StoryChapterId { get; set; }   // ← CHANGED

    [MaxLength(4096)] public string? RawText { get; set; }

    public List<Guid>? LinkedBeatIds { get; set; }
    public int? OrderIndex { get; set; }
}

public class SceneResponseDto : MetaInfoResponseBaseDto
{
    public Guid StoryChapterId { get; set; }    // ← CHANGED from StoryOutlineId
    public string RawText { get; set; } = "";
    public List<Guid> LinkedBeatIds { get; set; } = new();
    public int? OrderIndex { get; set; }
}

public class SceneListResponseDto
{
    public IEnumerable<SceneResponseDto> Items { get; set; } = Enumerable.Empty<SceneResponseDto>();
    public int TotalCount { get; set; }
}
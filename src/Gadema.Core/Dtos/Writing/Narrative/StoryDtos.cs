using Gadema.Core.Dtos.Base.Infrastructure;

namespace Gadema.Core.Dtos.Writing.Narrative;

public class StoryCreateDto
{
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();

    [MaxLength(4096)] public string? Description { get; set; }
}

public class StoryUpdateDto : UpdateRequestDto
{
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    [MaxLength(4096)] public string? Description { get; set; }
}

public class StoryResponseDto : MetaInfoResponseBaseDto
{
    public string? Description { get; set; }
}

public class StoryListResponseDto
{
    public IEnumerable<StoryResponseDto> Items { get; set; } = Enumerable.Empty<StoryResponseDto>();
    public int TotalCount { get; set; }
}
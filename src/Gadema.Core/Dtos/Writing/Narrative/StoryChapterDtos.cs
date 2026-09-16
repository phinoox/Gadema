using System.ComponentModel.DataAnnotations;
using Gadema.Core.Dtos;
using Gadema.Core.Enums;

namespace Gadema.Core.Dtos.Writing.Narrative;

public class StoryChapterCreateDto
{
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();

    [Required] public Guid StoryId { get; set; }

    [MaxLength(4096)] public string? Description { get; set; }

    public int? OrderIndex { get; set; }
}

public class StoryChapterUpdateDto : UpdateRequestDto
{
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    [Required] public Guid StoryId { get; set; }

    [MaxLength(4096)] public string? Description { get; set; }

    public int? OrderIndex { get; set; }
}

public class StoryChapterResponseDto : MetaInfoResponseBaseDto
{
    public Guid StoryId { get; set; }
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
}

public class StoryChapterListResponseDto
{
    public IEnumerable<StoryChapterResponseDto> Items { get; set; } = Enumerable.Empty<StoryChapterResponseDto>();
    public int TotalCount { get; set; }
}
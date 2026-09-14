using System.ComponentModel.DataAnnotations;
using Gadema.Core.Dtos;
using Gadema.Core.Enums;

namespace Gadema.Core.Dtos.Narrative;

public class StoryCreateDto
{
    [Required] public MetaInfoCreateData CreateData { get; set; } = new();

    [MaxLength(4096)] public string? Description { get; set; }
}

public class StoryUpdateDto : UpdateRequestDto
{
    public MetaInfoUpdateData? ContentMetaInfo { get; set; }

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

using System.ComponentModel.DataAnnotations;
using Gadema.Core.Enums;

namespace Gadema.Core.Dtos.Narrative;


public class StoryOutlineCreateDto
{
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();

    [Required] public Guid StoryId { get; set; }   // ← NEW: explicit Story link

    [MaxLength(4096)] public string? Summary { get; set; }
}

public class StoryOutlineUpdateDto : UpdateRequestDto
{
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    [MaxLength(4096)] public string? Summary { get; set; }
}

public class StoryOutlineResponseDto : MetaInfoResponseBaseDto
{
    public Guid StoryId { get; set; }
    public string? Summary { get; set; }
}

public class StoryOutlineListResponseDto
{
    public IEnumerable<StoryOutlineResponseDto> Items { get; set; } = Enumerable.Empty<StoryOutlineResponseDto>();
    public int TotalCount { get; set; }
}
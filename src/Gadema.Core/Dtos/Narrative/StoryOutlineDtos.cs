
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Enums;

namespace Gadema.Core.Dtos.Narrative;


// CreateDto — nested MetaInfoCreateData + entity-specific fields
public class StoryOutlineCreateDto
{
    [Required] public MetaInfoCreateData CreateData { get; set; } = new();

    public string? Summary { get; set; }
    public string? CharacterSnapshot { get; set; }
    public string? ThemeStatement { get; set; }
    public OutlineStatusEnum? OutlineStatus { get; set; }
}

// UpdateDto — inherits UpdateRequestDto, all fields nullable + nested MetaInfoUpdateData
public class StoryOutlineUpdateDto : UpdateRequestDto
{
    public MetaInfoUpdateData? MetaInfo { get; set; }

    public string? RawText { get; set; }
    public string? Summary { get; set; }
    public string? CharacterSnapshot { get; set; }
    public string? ThemeStatement { get; set; }
    public OutlineStatusEnum? OutlineStatus { get; set; }
}

// ResponseDto — inherits MetaInfoResponseBaseDto (Id, MetaInfoId, MetaInfoTitle, Status, IsPublic, CreatedAt, LastModifiedAt)
public class StoryOutlineResponseDto : MetaInfoResponseBaseDto
{
    public string RawText { get; set; } = "";
    public string? Summary { get; set; }
    public string? CharacterSnapshot { get; set; }
    public string? ThemeStatement { get; set; }
    public OutlineStatusEnum OutlineStatus { get; set; }
}

// ListResponseDto — already correct (standard wrapper)
public class StoryOutlineListResponseDto
{
    public IEnumerable<StoryOutlineResponseDto> Items { get; set; } = Enumerable.Empty<StoryOutlineResponseDto>();
    public int TotalCount { get; set; }
}
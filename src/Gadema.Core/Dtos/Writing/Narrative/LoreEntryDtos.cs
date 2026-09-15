// CreateDto — mirrors SceneCreateDto pattern
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Dtos;
using Gadema.Core.Enums;

public class LoreEntryCreateDto
{
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();
    
    [EnumDataType(typeof(LoreTypeEnum)), Required]
    public LoreTypeEnum LoreType { get; set; }
}

// UpdateDto — inherits UpdateRequestDto, nullable fields + nested MetaInfoUpdateData
public class LoreEntryUpdateDto : UpdateRequestDto
{
    public MetaInfoUpdateData? ContentMetaInfo { get; set; }
    
    public string? RawText { get; set; }
    public bool? IsPublic { get; set; }
}

// ResponseDto — inherits MetaInfoResponseBaseDto (Id, MetaInfoId, MetaInfoTitle, Status, IsPublic, CreatedAt, LastModifiedAt)
public class LoreEntryResponseDto : MetaInfoResponseBaseDto
{
    public int LoreType { get; set; }
    public string? RawText { get; set; }
}

// ListResponseDto — standard wrapper (already correct)
public class LoreEntryListResponseDto
{
    public IEnumerable<LoreEntryResponseDto> Items { get; set; } = Enumerable.Empty<LoreEntryResponseDto>();
    public int TotalCount { get; set; }
}
// CreateDto — mirrors SceneCreateDto pattern
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Dtos;
using Gadema.Core.Enums;

public class LoreEntryCreateDto
{
    [Required] public MetaInfoCreateData CreateData { get; set; } = new();
    
    [EnumDataType(typeof(LoreTypeEnum)), Required]
    public LoreTypeEnum LoreType { get; set; }
}

public class LoreEntryCreateResponseDto
{
    public CreateResponseDto Data { get; set; } = new();
}

// UpdateDto — inherits UpdateRequestDto, nullable fields + nested MetaInfo
public class LoreEntryUpdateDto : UpdateRequestDto
{
    public MetaInfoUpdateData? MetaInfo { get; set; }
    
    public string? RawText { get; set; }
    public bool? Published { get; set; }
}

// ResponseDto — includes MetaInfoId, MetaInfoTitle (denormalized); no direct Title/Slug/ProjectId
public class LoreEntryResponseDto
{
    public Guid Id { get; set; }
    public Guid MetaInfoId { get; set; }
    public string? MetaInfoTitle { get; set; }
    public int LoreType { get; set; }
    public string? RawText { get; set; }
    public bool Published { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
}

// ListResponseDto — standard wrapper (already correct)
public class LoreEntryListResponseDto
{
    public IEnumerable<LoreEntryResponseDto> Items { get; set; } = Enumerable.Empty<LoreEntryResponseDto>();
    public int TotalCount { get; set; }
}
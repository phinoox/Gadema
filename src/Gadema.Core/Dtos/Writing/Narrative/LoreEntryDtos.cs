using System.ComponentModel.DataAnnotations;
using Gadema.Core.Dtos;
using Gadema.Core.Models.Base.Enums;
using Gadema.Core.Models.Base.MetaInfo;

namespace Gadema.Core.Dtos.Writing.Narrative;

public class LoreEntryCreateDto
{
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();
    
    [EnumDataType(typeof(LoreTypeEnum)), Required]
    public LoreTypeEnum LoreType { get; set; }
}

public class LoreEntryUpdateDto : UpdateRequestDto
{
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }
    
    public string? RawText { get; set; }
    public bool? IsPublic { get; set; }
}

public class LoreEntryResponseDto : MetaInfoResponseBaseDto
{
    public int LoreType { get; set; }
    public string? RawText { get; set; }
}

public class LoreEntryListResponseDto
{
    public IEnumerable<LoreEntryResponseDto> Items { get; set; } = Enumerable.Empty<LoreEntryResponseDto>();
    public int TotalCount { get; set; }
}
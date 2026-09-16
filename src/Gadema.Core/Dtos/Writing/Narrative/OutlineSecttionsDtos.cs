using System.ComponentModel.DataAnnotations;
using Gadema.Core.Dtos;
using Gadema.Core.Models.Base.Enums;

namespace Gadema.Core.Dtos.Writing.Narrative;

public class OutlineSectionCreateDto
{
    [Required] public Guid StoryOutlineId { get; set; }

    [Required] public string Title { get; set; } = "";

    public string RawText { get; set; } = "";

    public int? SortOrder { get; set; }

    public List<Guid>? LinkedBeatIds { get; set; }
}

public class OutlineSectionUpdateDto : UpdateRequestDto
{
    [Required] public Guid StoryOutlineId { get; set; }

    public string? Title { get; set; }

    public string? RawText { get; set; }

    public int? SortOrder { get; set; }

    public List<Guid>? LinkedBeatIds { get; set; }
}

public class OutlineSectionResponseDto
{
    public Guid Id { get; set; }
    public Guid StoryOutlineId { get; set; }
    public string Title { get; set; } = "";
    public string RawText { get; set; } = "";
    public int SortOrder { get; set; }
    public List<Guid> LinkedBeatIds { get; set; } = new();
}

public class OutlineSectionListResponseDto
{
    public IEnumerable<OutlineSectionResponseDto> Items { get; set; } = Enumerable.Empty<OutlineSectionResponseDto>();
    public int TotalCount { get; set; }
}
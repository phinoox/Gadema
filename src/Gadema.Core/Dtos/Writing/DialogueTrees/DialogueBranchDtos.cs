using System.ComponentModel.DataAnnotations;
using Gadema.Core.Dtos;

namespace Gadema.Core.Dtos.DialogueTrees;

public class DialogueBranchCreateDto
{
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();

    [Required, MaxLength(128)] public string Title { get; set; } = "";

    [MaxLength(128)] public string? Slug { get; set; }

    [MaxLength(1024)] public string? VisualNodeImageUri { get; set; }

    [MaxLength(512)] public string? CharacterIconUri { get; set; }

    public bool IsRoot { get; set; } = false;

    public int? OrderIndex { get; set; }
}

public class DialogueBranchUpdateDto : UpdateRequestDto
{
    public MetaInfoUpdateData? ContentMetaInfo { get; set; }

    [MaxLength(128)] public string? Title { get; set; }

    [MaxLength(128)] public string? Slug { get; set; }

    [MaxLength(1024)] public string? VisualNodeImageUri { get; set; }

    [MaxLength(512)] public string? CharacterIconUri { get; set; }

    public bool? IsRoot { get; set; }

    public int? OrderIndex { get; set; }
}

public class DialogueBranchResponseDto : MetaInfoResponseBaseDto
{
    public string Title { get; set; } = "";
    public string Slug { get; set; } = "";
    public string? VisualNodeImageUri { get; set; }
    public string? CharacterIconUri { get; set; }
    public bool IsRoot { get; set; }
    public int OrderIndex { get; set; }
}

public class DialogueBranchListResponseDto
{
    public IEnumerable<DialogueBranchResponseDto> Items { get; set; } = Enumerable.Empty<DialogueBranchResponseDto>();
    public int TotalCount { get; set; }
}
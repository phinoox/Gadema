using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.DialogueTrees;

public class DialogueNodeCreateDto
{
    [Required] public string NodeText { get; set; } = "";
    public Guid? SpeakerId { get; set; }
    public Guid? ParentNodeId { get; set; }
    // Note: ChoiceOptions/Conditions would be passed as raw strings or specific objects here
    public string? ChoiceOptions { get; set; }
    public string? Conditions { get; set; }
}

public class DialogueNodeUpdateDto : UpdateRequestDto
{
    public string? NodeText { get; set; }
    public Guid? SpeakerId { get; set; }
    public Guid? ParentNodeId { get; set; }
    public string? ChoiceOptions { get; set; }
    public string? Conditions { get; set; }
}

public class DialogueNodeResponseDto
{
    public Guid Id { get; set; }
    public Guid DialogueBranchId { get; set; }
    public string BranchTitle { get; set; } = ""; // Denormalized from Branch ContentMetaInfo
    public string NodeText { get; set; } = "";
    public Guid? SpeakerId { get; set; }
    public string? SpeakerName { get; set; }
    public string? ChoiceOptions { get; set; }
    public string? Conditions { get; set; }
    public Guid? ParentNodeId { get; set; }
}

public class DialogueNodeListResponseDto
{
    public IEnumerable<DialogueNodeResponseDto> Items { get; set; } = Enumerable.Empty<DialogueNodeResponseDto>();
    public int TotalCount { get; set; }
}
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.DialogueTrees;

/// <summary>
/// DTO for creating a dialogue node within a branch.
/// </summary>
public class DialogueNodeUpdateDto
{
    /// <summary>
    /// ID of the content item this node belongs to (optional).
    /// </summary>
    public Guid? MetaInfoId { get; set; }

    /// <summary>
    /// ID of the branch this node belongs to.
    /// </summary>
    [Required]
    public Guid DialogueBranchId { get; set; }

    /// <summary>
    /// Text content of the dialogue node.
    /// </summary>
    [MaxLength(4096)]
    public string NodeText { get; set; } = "";

    /// <summary>
    /// ID of the speaker (character) for this node.
    /// </summary>
    public Guid? SpeakerId { get; set; }

    /// <summary>
    /// Name of the speaker.
    /// </summary>
    [MaxLength(256)]
    public string? SpeakerName { get; set; }

    /// <summary>
    /// Choice options (JSON array).
    /// </summary>
    [MaxLength(4096)]
    public string? ChoiceOptions { get; set; }

    /// <summary>
    /// Conditions for this node (JSON).
    /// </summary>
    [MaxLength(4096)]
    public string? Conditions { get; set; }

    /// <summary>
    /// Parent node ID for hierarchical dialogue structure.
    /// </summary>
    public Guid? ParentNodeId { get; set; }
}

/// <summary>
/// Response DTO for a dialogue node.
/// </summary>
public class DialogueNodeResponseDto
{
    public Guid Id { get; set; }
    public Guid? MetaInfoId { get; set; }
    public Guid DialogueBranchId { get; set; }
    public string NodeText { get; set; } = "";
    public Guid? SpeakerId { get; set; }
    public string? SpeakerName { get; set; }
    public string? ChoiceOptions { get; set; }
    public string? Conditions { get; set; }
    public Guid? ParentNodeId { get; set; }
}

/// <summary>
/// List response for dialogue nodes.
/// </summary>
public class DialogueNodeListResponseDto
{
    public IEnumerable<DialogueNodeResponseDto> Items { get; set; } = Enumerable.Empty<DialogueNodeResponseDto>();
    public int TotalCount { get; set; }
}

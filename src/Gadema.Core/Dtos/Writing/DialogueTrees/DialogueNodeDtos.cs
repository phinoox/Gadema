using Gadema.Core.Dtos.Base.Infrastructure;

namespace Gadema.Core.Dtos.Writing.DialogueTrees;

/// <summary>
/// Data transfer object for creating a new dialogue node within a branch.
/// </summary>
public class DialogueNodeCreateDto
{
    /// <summary>
    /// The actual text spoken or described in this node.
    /// </summary>
    [Required] public string NodeText { get; set; } = "";

    /// <summary>
    /// The unique identifier of the character speaking this line (if applicable).
    /// </summary>
    public Guid? SpeakerId { get; set; }

    /// <summary>
    /// The parent node in the dialogue tree. Null if this is a root node.
    /// </summary>
    public Guid? ParentNodeId { get; set; }

    /// <summary>
    /// JSON-serialized options for player choices following this node.
    /// </summary>
    public string? ChoiceOptions { get; set; }

    /// <summary>
    /// JSON-serialized conditions required to access or trigger this node.
    /// </summary>
    public string? Conditions { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing dialogue node.
/// </summary>
public class DialogueNodeUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// The updated text content of the node.
    /// </summary>
    public string? NodeText { get; set; }

    /// <summary>
    /// The updated speaker identifier.
    /// </summary>
    public Guid? SpeakerId { get; set; }

    /// <summary>
    /// The updated parent node identifier.
    /// </summary>
    public Guid? ParentNodeId { get; set; }

    /// <summary>
    /// The updated JSON-serialized choice options.
    /// </summary>
    public string? ChoiceOptions { get; set; }

    /// <summary>
    /// The updated JSON-serialized conditions.
    /// </summary>
    public string? Conditions { get; set; }
}

/// <summary>
/// Represents a single dialogue node, including denormalized speaker and branch context.
/// </summary>
public class DialogueNodeResponseDto
{
    /// <summary>
    /// The unique identifier of the node.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the dialogue branch this node belongs to.
    /// </summary>
    public Guid DialogueBranchId { get; set; }

    /// <summary>
    /// The title of the parent dialogue branch for UI convenience.
    /// </summary>
    public string BranchTitle { get; set; } = ""; // Denormalized from Branch ContentMetaInfo

    /// <summary>
    /// The text spoken in this node.
    /// </summary>
    public string NodeText { get; set; } = "";

    /// <summary>
    /// The identifier of the speaker.
    /// </summary>
    public Guid? SpeakerId { get; set; }

    /// <summary>
    /// The display name of the speaker for UI convenience.
    /// </summary>
    public string? SpeakerName { get; set; }

    /// <summary>
    /// JSON-serialized options available after this node.
    /// </summary>
    public string? ChoiceOptions { get; set; }

    /// <summary>
    /// JSON-serialized conditions for this node.
    /// </summary>
    public string? Conditions { get; set; }

    /// <summary>
    /// The identifier of the parent node in the tree.
    /// </summary>
    public Guid? ParentNodeId { get; set; }
}

/// <summary>
/// A collection of dialogue nodes, typically used for paginated lists.
/// </summary>
public class DialogueNodeListResponseDto
{
    /// <summary>
    /// The list of retrieved dialogue nodes.
    /// </summary>
    public IEnumerable<DialogueNodeResponseDto> Items { get; set; } = Enumerable.Empty<DialogueNodeResponseDto>();

    /// <summary>
    /// Total number of nodes found across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}
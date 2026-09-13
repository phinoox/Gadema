// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Content.Branches;

/// <summary>
/// DTO for creating a dialogue node (a single branch point in the tree).
/// </summary>
public class NodeCreateDto
{
    /// <summary>
    /// MetaInfo data for this node's identity.
    /// </summary>
    [Required] public MetaInfoCreateData CreateData { get; set; } = new();

    /// <summary>
    /// ID of the parent branch/node in the tree (self-referencing).
    /// For root nodes, pass null or an empty GUID.
    /// </summary>
    [Required] public Guid? ParentNodeId { get; set; }

    /// <summary>
    /// Title displayed at this node.
    /// </summary>
    [Required, MaxLength(128)] public string Title { get; set; } = "";

    /// <summary>
    /// The spoken text of this node (dialogue line).
    /// </summary>
    [Required, MaxLength(4096)] public string NodeText { get; set; } = "";

    /// <summary>
    /// ID of the character speaking at this node.
    /// Null means "unspecified" or "narrator".
    /// </summary>
    public Guid? SpeakerId { get; set; }

    /// <summary>
    /// Optional display name for the speaker (for UI).
    /// Used when SpeakerId is null or unavailable.
    /// </summary>
    [MaxLength(256)] public string? SpeakerName { get; set; }

    /// <summary>
    /// Choice options available at this node (JSON array of choice objects).
    /// Each object: { id, label, nextNodeId, conditions?, icon? }
    /// </summary>
    [MaxLength(8192)] public string? ChoiceOptions { get; set; }

    /// <summary>
    /// Conditions required for this node to be reachable (JSON).
    /// E.g., { "requiredTraits": ["brave"], "minLevel": 5, "itemOwned": "sword" }
    /// </summary>
    [MaxLength(8192)] public string? Conditions { get; set; }

    /// <summary>
    /// Background image for this node.
    /// </summary>
    [MaxLength(2048)] public string? NodeBackgroundImageUri { get; set; }

    /// <summary>
    /// Optional description text shown above the dialogue (contextual).
    /// </summary>
    [MaxLength(1024)] public string? DescriptionText { get; set; }

    /// <summary>
    /// Whether this node is marked as an "important" beat.
    /// </summary>
    public bool IsImportantBeat { get; set; } = false;
}

/// <summary>
/// DTO for updating a dialogue node (partial update).
/// </summary>
public class NodeUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// MetaInfo fields (nullable — omit to keep current values).
    /// </summary>
    public MetaInfoUpdateData? MetaInfo { get; set; }

    [MaxLength(128)] public string? Title { get; set; }
    [MaxLength(4096)] public string? NodeText { get; set; }
    public Guid? SpeakerId { get; set; }
    [MaxLength(256)] public string? SpeakerName { get; set; }

    // Choice options are stored as JSON — provide the full array or null to remove all choices
    [MaxLength(8192)] public string? ChoiceOptions { get; set; }

    // Conditions are stored as JSON — provide the full object or null to remove conditions
    [MaxLength(8192)] public string? Conditions { get; set; }

    [MaxLength(2048)] public string? NodeBackgroundImageUri { get; set; }
    [MaxLength(1024)] public string? DescriptionText { get; set; }
    public bool? IsImportantBeat { get; set; }
}

/// <summary>
/// Response DTO for a dialogue node. Inherits MetaInfo state.
/// </summary>
public class NodeResponseDto : MetaInfoResponseBaseDto
{
    [Required, MaxLength(128)] public string Title { get; set; } = "";
    [MaxLength(4096)] public string NodeText { get; set; } = "";

    /// <summary>
    /// ID of the parent node in the tree. Null for root nodes.
    /// </summary>
    public Guid? ParentNodeId { get; set; }

    /// <summary>
    /// Number of child nodes directly connected to this one.
    /// </summary>
    public int ChildNodeCount { get; set; }

    /// <summary>
    /// Total size of the subtree rooted at this node.
    /// </summary>
    public int TreeSize { get; set; }

    public Guid? SpeakerId { get; set; }
    [MaxLength(256)] public string? SpeakerName { get; set; }

    /// <summary>
    /// Choice options as a JSON array (e.g., [{"id":"a","label":"Yes","nextNodeId":"node-456"}]).
    /// </summary>
    [MaxLength(8192)] public string? ChoiceOptions { get; set; }

    /// <summary>
    /// Reachability conditions as JSON.
    /// </summary>
    [MaxLength(8192)] public string? Conditions { get; set; }

    [MaxLength(2048)] public string? NodeBackgroundImageUri { get; set; }
    [MaxLength(1024)] public string? DescriptionText { get; set; }
    public bool IsImportantBeat { get; set; }
}

/// <summary>
/// List response for dialogue nodes.
/// </summary>
public class NodeListResponseDto
{
    public IEnumerable<NodeResponseDto> Items { get; set; } = Enumerable.Empty<NodeResponseDto>();
    public int TotalCount { get; set; }
}
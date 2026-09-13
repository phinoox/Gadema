// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Content.Branches;

/// <summary>
/// DTO for creating a dialogue branch (visual node in the narrative tree).
/// </summary>
public class CreateBranchDto
{
    /// <summary>
    /// MetaInfo data for the branch identity.
    /// </summary>
    [Required] public MetaInfoCreateData CreateData { get; set; } = new();

    /// <summary>
    /// Title of this node in the dialogue tree.
    /// </summary>
    [Required, MaxLength(128)] public string Title { get; set; } = "";

    /// <summary>
    /// URL-friendly slug for the branch.
    /// </summary>
    [MaxLength(128)] public string? Slug { get; set; }

    /// <summary>
    /// Optional image URI displayed when this node is active in the visual tree.
    /// </summary>
    [MaxLength(1024)] public string? VisualNodeImageUri { get; set; }

    /// <summary>
    /// Character icon to display at this branch point.
    /// </summary>
    [MaxLength(512)] public string? CharacterIconUri { get; set; }

    /// <summary>
    /// Whether this is a root node in the tree.
    /// </summary>
    public bool IsRoot { get; set; } = false;

    /// <summary>
    /// Order index for sorting branches visually.
    /// </summary>
    public int? OrderIndex { get; set; }
}

/// <summary>
/// DTO for updating a dialogue branch (partial update).
/// </summary>
public class BranchUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// MetaInfo fields (nullable — omit to keep current values).
    /// </summary>
    public MetaInfoUpdateData? MetaInfo { get; set; }

    [MaxLength(128)] public string? Title { get; set; }
    [MaxLength(128)] public string? Slug { get; set; }
    [MaxLength(1024)] public string? VisualNodeImageUri { get; set; }
    [MaxLength(512)] public string? CharacterIconUri { get; set; }
    public bool? IsRoot { get; set; }
    public int? OrderIndex { get; set; }
}

/// <summary>
/// Response DTO for a dialogue branch. Inherits MetaInfo state.
/// </summary>
public class BranchResponseDto : MetaInfoResponseBaseDto
{
    [Required, MaxLength(128)] public string Title { get; set; } = "";
    [MaxLength(128)] public string Slug { get; set; } = "";
    [MaxLength(1024)] public string? VisualNodeImageUri { get; set; }
    [MaxLength(512)] public string? CharacterIconUri { get; set; }
    public bool IsRoot { get; set; }
    public int OrderIndex { get; set; }

    /// <summary>
    /// Number of child nodes (descendants) in this branch's subtree.
    /// </summary>
    public int ChildNodeCount { get; set; }

    /// <summary>
    /// Total number of nodes in the entire tree rooted at this branch.
    /// </summary>
    public int TreeSize { get; set; }
}

/// <summary>
/// List response for dialogue branches.
/// </summary>
public class BranchListResponseDto
{
    public IEnumerable<BranchResponseDto> Items { get; set; } = Enumerable.Empty<BranchResponseDto>();
    public int TotalCount { get; set; }
}
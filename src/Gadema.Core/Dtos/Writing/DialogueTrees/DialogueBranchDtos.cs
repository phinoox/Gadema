/// <summary>
/// Data transfer object for creating a new dialogue branch.
/// </summary>
public class DialogueBranchCreateDto
{
    /// <summary>
    /// The metadata required to establish the branch's identity (e.g., Title, Slug).
    /// </summary>
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();

    /// <summary>
    /// The display title of the dialogue branch.
    /// </summary>
    [Required, MaxLength(128)] public string Title { get; set; } = "";

    /// <summary>
    /// A URL-friendly slug for the branch.
    /// </summary>
    [MaxLength(128)] public string? Slug { get; set; }

    /// <summary>
    /// URI to an image representing this node in a visual graph.
    /// </summary>
    [MaxLength(1024)] public string? VisualNodeImageUri { get; set; }

    /// <summary>
    /// URI to an icon of the character associated with this branch.
    /// </summary>
    [MaxLength(512)] public string? CharacterIconUri { get; set; }

    /// <summary>
    /// Indicates if this is a root node in the dialogue tree.
    /// </summary>
    public bool IsRoot { get; set; } = false;

    /// <summary>
    /// The sort order index for this branch within its parent.
    /// </summary>
    public int? OrderIndex { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing dialogue branch.
/// </summary>
public class DialogueBranchUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// The identity payload used by the sync strategy to update meta-information.
    /// </summary>
    public BaseMetaInfoUpdateData? ContentMetaInfo { get; set; }

    /// <summary>
    /// The updated title of the branch.
    /// </summary>
    [MaxLength(128)] public string? Title { get; set; }

    /// <summary>
    /// The updated slug for the branch.
    /// </summary>
    [MaxLength(128)] public string? Slug { get; set; }

    /// <summary>
    /// The updated visual node image URI.
    /// </summary>
    [MaxLength(1024)] public string? VisualNodeImageUri { get; set; }

    /// <summary>
    /// The updated character icon URI.
    /// </summary>
    [MaxLength(512)] public string? CharacterIconUri { get; set; }

    /// <summary>
    /// Updates whether this is a root node.
    /// </summary>
    public bool? IsRoot { get; set; }

    /// <summary>
    /// The updated sort order index.
    /// </summary>
    public int? OrderIndex { get; set; }
}

/// <summary>
/// Represents a dialogue branch, including its visual and identity metadata.
/// </summary>
public class DialogueBranchResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// The display title of the branch.
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// The URL-friendly slug for the branch.
    /// </summary>
    public string Slug { get; set; } = "";

    /// <summary>
    /// URI to an image representing this node in a visual graph.
    /// </summary>
    public string? VisualNodeImageUri { get; set; }

    /// <summary>
    /// URI to an icon of the character associated with this branch.
    /// </summary>
    public string? CharacterIconUri { get; set; }

    /// <summary>
    /// Indicates if this is a root node in the dialogue tree.
    /// </summary>
    public bool IsRoot { get; set; }

    /// <summary>
    /// The sort order index for this branch within its parent.
    /// </summary>
    public int OrderIndex { get; set; }
}

/// <summary>
/// A collection of dialogue branches, typically used for paginated lists.
/// </summary>
public class DialogueBranchListResponseDto
{
    /// <summary>
    /// The list of retrieved dialogue branches.
    /// </summary>
    public IEnumerable<DialogueBranchResponseDto> Items { get; set; } = Enumerable.Empty<DialogueBranchResponseDto>();

    /// <summary>
    /// Total number of branches found across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}
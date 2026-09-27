namespace Gadema.Core.Dtos.Writing.DialogueTrees;

/// <summary>
/// Data transfer object used to update an existing dialogue branch's properties and metadata.
/// </summary>
public class UpdateBranchDto
{
    /// <summary>
    /// The unique identifier of the project this branch belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// The updated title of the dialogue branch.
    /// </summary>
    [MaxLength(128)]
    public string? Title { get; set; }

    /// <summary>
    /// The updated URL-friendly slug for the branch.
    /// </summary>
    [MaxLength(128)]
    public string? Slug { get; set; }

    /// <summary>
    /// The updated image URI for visual node representation.
    /// </summary>
    [MaxLength(1024)]
    public string? VisualNodeImageUri { get; set; }

    /// <summary>
    /// The updated character icon URI for this branch.
    /// </summary>
    [MaxLength(512)]
    public string? CharacterIconUri { get; set; }

    /// <summary>
    /// The updated sort order index for the branch.
    /// </summary>
    public int? OrderIndex { get; set; }
}

using System.ComponentModel.DataAnnotations;
using Gadema.Core.Models.Base.Projects;

namespace Gadema.Core.Dtos.Writing.DialogueTrees;

/// <summary>
/// DTO for updating a dialogue branch.
/// </summary>
public class UpdateBranchDto
{
    /// <summary>
    /// ID of the project this branch belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Title of the dialogue branch.
    /// </summary>
    [MaxLength(128)]
    public string? Title { get; set; }

    /// <summary>
    /// URL-friendly slug for the branch.
    /// </summary>
    [MaxLength(128)]
    public string? Slug { get; set; }

    /// <summary>
    /// Image URI for visual node representation.
    /// </summary>
    [MaxLength(1024)]
    public string? VisualNodeImageUri { get; set; }

    /// <summary>
    /// Character icon URI for this branch.
    /// </summary>
    [MaxLength(512)]
    public string? CharacterIconUri { get; set; }

    /// <summary>
    /// Order index for sorting branches.
    /// </summary>
    public int? OrderIndex { get; set; }
}

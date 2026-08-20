// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Dtos.DialogueTrees;

/// <summary>
/// DTO for creating a new dialogue branch.
/// </summary>
public class CreateBranchDto
{
    /// <summary>
    /// ID of the project this branch belongs to (required).
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// Title of the dialogue branch (required).
    /// </summary>
    [MaxLength(128), Required, Display(Name = "Branch Title")]
    public string Title { get; set; } = "";
    
    /// <summary>
    /// URL-friendly slug for the branch (optional, auto-generated if not provided).
    /// </summary>
    [MaxLength(128)]
    public string? Slug { get; set; } = null!;
    
    /// <summary>
    /// Image URI for visual node representation (optional).
    /// </summary>
    [MaxLength(1024)]
    public string? VisualNodeImageUri { get; set; } = null!;
    
    /// <summary>
    /// Character icon URI for this branch (optional).
    /// </summary>
    [MaxLength(512)]
    public string? CharacterIconUri { get; set; } = null!;
}
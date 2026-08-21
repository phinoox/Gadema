// =============================================================================
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Represents a dialogue branch (node) in a branching narrative tree.
/// Used for visual novel and interactive story development.
/// </summary>
public class DialogueBranch
{
    /// <summary>
    /// Unique identifier for the dialogue branch.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the project this branch belongs to.
    /// </summary>
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// Title of the dialogue branch.
    /// </summary>
    [MaxLength(128), Required, Display(Name = "Branch Title")]
    public string Title { get; set; } = "";
    
    /// <summary>
    /// URL-friendly slug for the branch (unique).
    /// </summary>
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
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
    /// Indicates if this is a root node in the tree.
    /// </summary>
    public bool IsRoot { get; set; } = false;
    
    /// <summary>
    /// ID of parent node (for self-referencing FK).
    /// </summary>
    public Guid? ParentNodeId { get; set; }
    
    /// <summary>
    /// Order index for sorting branches.
    /// </summary>
    public int OrderIndex { get; set; } = 0;
}
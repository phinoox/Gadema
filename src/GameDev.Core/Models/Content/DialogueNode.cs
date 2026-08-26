// =============================================================================
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Represents a dialogue node within a branch.
/// Contains the actual dialogue text and choices.
/// </summary>
public class DialogueNode
{
    /// <summary>
    /// Unique identifier for the dialogue node.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? ContentItemId { get; set; }  
    
    [ForeignKey("ContentItemId")]
    public virtual ContentItem? ContentItem { get; set; }


    /// <summary>
    /// ID of the branch this node belongs to.
    /// </summary>
    [Required, Display(Name = "Branch ID")]
    public Guid BranchId { get; set; }

    // Navigation property: DialogueBranch (Many-to-One)
    [ForeignKey("BranchId")]
    public virtual DialogueBranch Branch { get; set; }

    /// <summary>
    /// Text content of the dialogue node.
    /// </summary>
    [MaxLength(4096)]
    public string NodeText { get; set; } = "";

    /// <summary>
    /// ID of the speaker (character) for this node.
    /// </summary>
    public Guid? SpeakerId { get; set; }

    // Navigation property: Speaker (User - Many-to-One)
    [ForeignKey("SpeakerId")]
    public virtual User? Speaker { get; set; }

    /// <summary>
    /// Choice options (JSON array).
    /// </summary>
    [MaxLength(4096)]
    public string? ChoiceOptions { get; set; }  // JSON array

    /// <summary>
    /// Conditions for this node (JSON).
    /// </summary>
    [MaxLength(4096)]
    public string? Conditions { get; set; }  // JSON conditions

    // Self-referencing navigation properties for tree hierarchy
    /// <summary>
    /// Parent node ID for hierarchical dialogue structure.
    /// Used for branching conversations in visual novels.
    /// </summary>
    public Guid? ParentNodeId { get; set; }

    // Navigation property: Parent Node (self-referencing, optional)
    [ForeignKey("ParentNodeId")]
    public virtual DialogueNode? ParentNode { get; set; }
    
    /// <summary>
    /// Collection of child nodes (for dialogue tree).
    /// Foreign key: ParentNodeId (matches FK in DialogueNode)
    /// </summary>
    public virtual ICollection<DialogueNode> ChildNodes { get; set; } = new List<DialogueNode>();

}
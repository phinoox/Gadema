// =============================================================================
using Gadema.Core.DependencyResolver;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models;

/// <summary>
/// Represents a dialogue node within a branch.
/// Contains the actual dialogue text and choices.
/// </summary>
[ModelDependency(typeof(DialogueBranch),typeof(ContentItem))]
public class DialogueNode
{
    /// <summary>
    /// Unique identifier for the dialogue node.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? ContentItemId { get; set; }  
    
    [ForeignKey("ContentItemId")]
    [Fixture(FixtureHintEnum.Omit)]
    public virtual ContentItem? ContentItem { get; set; }


    /// <summary>
    /// ID of the branch this node belongs to.
    /// </summary>
    [Required, Display(Name = "Branch ID")]
    [Fixture(FixtureHintEnum.Omit)]
    public Guid DialogueBranchId { get; set; }

    // Navigation property: DialogueBranch (Many-to-One)
    [ForeignKey("BranchId")]
    public virtual DialogueBranch DialogueBranch { get; set; }

    /// <summary>
    /// Text content of the dialogue node.
    /// </summary>
    [MaxLength(4096)]
    public string NodeText { get; set; } = "";

    /// <summary>
    /// ID of the speaker (character) for this node.
    /// </summary>
    //[Fixture(FixtureHintEnum.Omit)]
    //public Guid? SpeakerId { get; set; }

    // Navigation property: Speaker (User - Many-to-One)
    //[ForeignKey("SpeakerId")]
    public virtual string? Speaker { get; set; } //ToDo: this should be a character

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
    [Fixture(FixtureHintEnum.Omit)]
    public Guid? ParentNodeId { get; set; }

    // Navigation property: Parent Node (self-referencing, optional)
    [ForeignKey("ParentNodeId")]
    public virtual DialogueNode? ParentNode { get; set; }
    
    /// <summary>
    /// Collection of child nodes (for dialogue tree).
    /// Foreign key: ParentNodeId (matches FK in DialogueNode)
    /// </summary>
    [Fixture(FixtureHintEnum.Omit)]
    public virtual ICollection<DialogueNode> ChildNodes { get; set; } = new List<DialogueNode>();

}
// =============================================================================

using Gadema.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models;

/// <summary>
/// Represents a dialogue branch (node) in a branching narrative tree.
/// Used for visual novel and interactive story development.
/// </summary>
[ModelDependency(typeof(Project),typeof(ContentMetaInfo))]
public class DialogueBranch
{
    /// <summary>
    /// Unique identifier for the dialogue branch.
    /// </summary>
    public Guid Id { get; set; }
    
   
    [Fixture(FixtureHintEnum.Omit)]
    public Guid? MetaInfoId { get; set; }  
    
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo? ContentMetaInfo { get; set; }
   
    
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
/*
    [Fixture(FixtureHintEnum.Omit)]
    public Guid? ParentNodeId { get; set; }

    // Navigation property: Parent Node (self-referencing)
    [ForeignKey("ParentNodeId")]
    public virtual DialogueBranch? ParentNode { get; set; }
  */  
    /// <summary>
    /// Order index for sorting branches.
    /// </summary>
    public int OrderIndex { get; set; } = 0;

    // Collection navigation properties
    /// <summary>
    /// Navigation property: Collection of dialogue nodes in this branch.
    /// Foreign key: BranchId (matches FK in DialogueNode)
    /// </summary>
    [Fixture(FixtureHintEnum.Omit)]
    public virtual ICollection<DialogueNode> ChildNodes { get; set; } = new List<DialogueNode>();

}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gadema.Core.Models.Writing.Characters;

namespace Gadema.Core.Models.Writing.Narrative;

/// <summary>
/// Represents a dialogue node within a branch.
/// Contains the actual dialogue text and choices.
/// </summary>
[ModelDependency(typeof(DialogueBranch), typeof(Character))]
public class DialogueNode
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// ID of the branch this node belongs to.
    /// </summary>
    [Required]
    public Guid DialogueBranchId { get; set; }

    [ForeignKey("DialogueBranchId")]
    public virtual DialogueBranch DialogueBranch { get; set; } = null!;

    [MaxLength(4096)]
    public string NodeText { get; set; } = "";

    public Guid? SpeakerId { get; set; }

    [ForeignKey("SpeakerId")]
    public virtual Character? Speaker { get; set; } 

    // In a real implementation, this would be a serialized JSON or a separate entity.
    // For this refactor, we treat it as the structural choice data.
    [MaxLength(4096)]
    public string? ChoiceOptions { get; set; } 

    [MaxLength(4096)]
    public string? Conditions { get; set; }

    public Guid? ParentNodeId { get; set; }

    [ForeignKey("ParentNodeId")]
    public virtual DialogueNode? ParentNode { get; set; }
    
    public virtual ICollection<DialogueNode> ChildNodes { get; set; } = new List<DialogueNode>();
}
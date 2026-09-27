using Gadema.Core.Models.Writing.Characters;

namespace Gadema.Core.Models.Writing.Narrative;

/// <summary>
/// Represents a single point in a dialogue tree.
/// Contains the spoken text, the speaker, and potential branches for player/character choices.
/// </summary>
[ModelDependency(typeof(DialogueBranch), typeof(Character))]
public class DialogueNode
{
    /// <summary>
    /// Unique identifier for this dialogue node.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The ID of the branch this node belongs to.
    /// </summary>
    [Required]
    public Guid DialogueBranchId { get; set; }

    /// <summary>
    /// Navigation property for the parent dialogue branch.
    /// </summary>
    [ForeignKey("DialogueBranchId")]
    public virtual DialogueBranch DialogueBranch { get; set; } = null!;

    /// <summary>
    /// The spoken text or narrative content at this node.
    /// </summary>
    [MaxLength(4096)]
    public string NodeText { get; set; } = "";

    /// <summary>
    /// The ID of the character speaking at this node.
    /// </summary>
    public Guid? SpeakerId { get; set; }

    /// <summary>
    /// Navigation property for the character who is speaking.
    /// </summary>
    [ForeignKey("SpeakerId")]
    public virtual Character? Speaker { get; set; } 

    /// <summary>
    /// Serialized data representing the available choices or paths following this node.
    /// </summary>
    [MaxLength(4096)]
    public string? ChoiceOptions { get; set; } 

    /// <summary>
    /// Requirements or logic that must be met for this node to be accessible (e.g., skill checks).
    /// </summary>
    [MaxLength(4096)]
    public string? Conditions { get; set; }

    /// <summary>
    /// The ID of the preceding dialogue node in the tree.
    /// </summary>
    public Guid? ParentNodeId { get; set; }

    /// <summary>
    /// Navigation property for the parent dialogue node.
    /// </summary>
    [ForeignKey("ParentNodeId")]
    public virtual DialogueNode? ParentNode { get; set; }
    
    /// <summary>
    /// Collection of child nodes branching out from this one.
    /// </summary>
    public virtual ICollection<DialogueNode> ChildNodes { get; set; } = new List<DialogueNode>();
}
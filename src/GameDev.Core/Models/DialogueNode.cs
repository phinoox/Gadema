// =============================================================================
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
    
    /// <summary>
    /// ID of the branch this node belongs to.
    /// </summary>
    [Required, Display(Name = "Branch ID")]
    public Guid BranchId { get; set; }
    
    /// <summary>
    /// Text content of the dialogue node.
    /// </summary>
    [MaxLength(4096)]
    public string NodeText { get; set; } = "";
    
    /// <summary>
    /// ID of the speaker (character) for this node.
    /// </summary>
    public Guid? SpeakerId { get; set; }
    
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
}
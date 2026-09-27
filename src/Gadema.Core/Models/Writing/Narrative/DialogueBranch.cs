using Gadema.Core.Models.Base.Projects;
using Gadema.Core.Utils;

namespace Gadema.Core.Models.Writing.Narrative;

/// <summary>
/// Represents a distinct path or tree within a branching dialogue system.
/// Used for interactive storytelling, visual novels, and RPG-style conversation trees.
/// </summary>
[ModelDependency(typeof(Project), typeof(ContentMetaInfo))]
public class DialogueBranch
{
    /// <summary>
    /// Unique identifier for the dialogue branch.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// The ID of the associated ContentMetaInfo entity, used for identity anchoring and searching.
    /// </summary>
    [Required]
    public Guid? MetaInfoId { get; set; }
    /// <summary>
    /// Navigation property for the branch's identity anchor.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo? ContentMetaInfo { get; set; }
    /// <summary>
    /// The human-readable title of this dialogue path.
    /// </summary>
    [MaxLength(128), Required, Display(Name = "Branch Title")]
    public string Title { get; set; } = "";
    /// <summary>
    /// A unique, URL-friendly slug for the branch.
    /// </summary>
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    /// <summary>
    /// URI for a visual representation (e.g., background image) of this dialogue node/branch.
    /// </summary>
    [MaxLength(1024)]
    public string? VisualNodeImageUri { get; set; }
    /// <summary>
    /// URI for the character icon associated with this branch.
    /// </summary>
    [MaxLength(512)]
    public string? CharacterIconUri { get; set; }
    
    /// <summary>
    /// Indicates if this is a starting (root) node in the dialogue tree.
    /// </summary>
    public bool IsRoot { get; set; } = false;

    /// <summary>
    /// The sort order for branches within a parent or list.
    /// </summary>
    public int OrderIndex { get; set; } = 0;

    /// <summary>
    /// Collection of <see cref="DialogueNode"/> entities that make up the tree structure of this branch.
    /// </summary>
    [Fixture(FixtureHintEnum.Omit)]
    public virtual ICollection<DialogueNode> ChildNodes { get; set; } = new List<DialogueNode>();

}
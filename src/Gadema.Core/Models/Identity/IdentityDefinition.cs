// =============================================================================
using Gadema.Core.Enums;
using Gadema.Core.Models.Base.Projects;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Identity;

/// <summary>
/// Defines a template for identity types (e.g., Race, Faction, Alignment) within a project.
/// This configuration dictates what character traits can be assigned to content items.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo), typeof(Project))]
public class IdentityDefinition
{
    /// <summary>
    /// Unique identifier for the identity definition.
    /// </summary>

    public Guid Id { get; set; } = Guid.NewGuid();

    // --- Identity Anchor (The "Soul") ---
    /// <summary>
    /// The ID of the associated ContentMetaInfo entity.
    /// </summary>
    [Required]
    public Guid MetaInfoId { get; set; }

    /// <summary>
    /// Navigation property for the content meta information anchor.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;

    // --- Domain Properties (The "Body") ---
    
    /// <summary>
    /// The category of this identity (e.g., Race, Faction).
    /// </summary>
    [Required]
    public IdentityDataTypeEnum DataType { get; set; }

    /// <summary>
    ///    The human-readable name for the definition (e.g., "Race", "Faction").
    /// </summary>
    [MaxLength(128), Required]
    public string Name { get; set; } = "";

    /// <summary>
    /// A brief description of what this identity category represents.
    /// </summary>
    [MaxLength(256)]
    public string? Description { get; set; }

    /// <summary>
    /// Indicates if an identity assignment is mandatory for content items using this definition.
    /// </summary>
    public bool IsRequired { get; set; } = false;

    /// <summary>
    /// Indicates whether this definition is currently active and available for selection.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// The ID of the project this identity definition belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Navigation property for the associated project.
    /// </summary>
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; } = null!;
}
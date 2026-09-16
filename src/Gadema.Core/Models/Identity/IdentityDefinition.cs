// =============================================================================
using Gadema.Core.Enums;
using Gadema.Core.Models.Base.Projects;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Identity;

/// <summary>
/// Definition of identity types (race, faction, alignment, guild) for a project.
/// Configures what character identities can be selected within this specific project.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo), typeof(Project))]
public class IdentityDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // --- Identity Anchor (The "Soul") ---
    [Required]
    public Guid MetaInfoId { get; set; }

    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;

    // --- Domain Properties (The "Body") ---
    
    /// <summary>
    /// The category of this identity (e.g., Race, Faction).
    /// </summary>
    [Required]
    public IdentityDataTypeEnum DataType { get; set; }

    /// <summary>
    /// The human-readable name (e.g., "Human", "Elf").
    /// </summary>
    [MaxLength(128), Required]
    public string Name { get; set; } = "";

    [MaxLength(256)]
    public string? Description { get; set; }

    public bool IsRequired { get; set; } = false;

    public bool IsActive { get; set; } = true;

    // Link back to the project for scoping
    [Required]
    public Guid ProjectId { get; set; }

    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; } = null!;
}
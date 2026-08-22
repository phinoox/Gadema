// =============================================================================
// IdentityValue - Entity for selectable identity values (specific races, factions, etc.)
// =============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GameDev.Core.Enums;

namespace GameDev.Core.Models;

/// <summary>
/// Specific identity values that can be selected for character identities.
/// Contains all the available options (e.g., "Human", "Elf", "Orc" for race selection).
/// </summary>
public class IdentityValue
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, Display(Name = "Identity Definition ID")]
    public Guid IdentityDefinitionId { get; set; }

    // Navigation property: IdentityDefinition (Many-to-One)
    [ForeignKey("IdentityDefinitionId")]
    public virtual IdentityDefinition IdentityDefinition { get; set; }

    [MaxLength(128), Required, Display(Name = "Value Name")]
    public string Name { get; set; } = "";

    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";

    [MaxLength(4096)]
    public string? Description { get; set; }

    public int OrderIndex { get; set; } = 0;

    public bool IsDefault { get; set; } = false;

    // ⬇️ ADD THESE NEW PROPERTIES (for FK-as-PK pattern with Project) ⬇️

    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }  // Foreign key to Project.Id

    // Navigation property: Project (Many-to-One relationship)
    [ForeignKey("ProjectId")]
    public virtual Projects.Project Project { get; set; }

    [EnumDataType(typeof(ProjectTemplateTypeEnum)), Required, Display(Name = "Project Template ID")]
    public int ProjectTemplateId { get; set; }

    /// <summary>
    /// Name of the identity type (e.g., "Race", "Faction").
    /// </summary>
    [MaxLength(128), Required, Display(Name = "Identity Name")]
    public string IdentityName { get; set; } = "";

    /// <summary>
    /// The actual identity value (e.g., "Human Male", "Elf Warrior").
    /// </summary>
    [MaxLength(4096)]
    public string? Value { get; set; } = null!;

    /// <summary>
    /// FK to ProjectTemplate for template-based identity system.
    /// </summary>
    [ForeignKey("ProjectTemplateId")]
    public virtual ProjectTemplate? ProjectTemplate { get; set; }

    [EnumDataType(typeof(IdentityTypeEnum)), Required, Display(Name = "Identity Type")]
    public int IdentityTypeId { get; set; }  // FK to IdentityDefinition.IdentityType

    [Display(Name = "Is Required?")]
    public bool IsRequired { get; set; } = false;
}
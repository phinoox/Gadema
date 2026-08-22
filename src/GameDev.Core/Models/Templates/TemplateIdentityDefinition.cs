// =============================================================================
// TemplateIdentityDefinition - Entity for identity system definitions in project templates
// =============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameDev.Core.Models;

/// <summary>
/// Definition of identity systems (race, faction, guild, etc.) for project templates.
/// Configures what identities can be selected for characters in templated projects.
/// </summary>
public class TemplateIdentityDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, Display(Name = "Project Template ID")]
    public Guid ProjectTemplateId { get; set; }

    // Navigation property: ProjectTemplate (Many-to-One)
    [ForeignKey("ProjectTemplateId")]
    public virtual ProjectTemplate ProjectTemplate { get; set; }

    [Required, Display(Name = "Identity Definition ID")]
    public Guid IdentityDefinitionId { get; set; }

    // Navigation property: IdentityDefinition (Many-to-One)
    [ForeignKey("IdentityDefinitionId")]
    public virtual IdentityDefinition IdentityDefinition { get; set; }

    [MaxLength(128)]
    public string? Description { get; set; }

    /// <summary>
    /// Name of the identity type (e.g., "Race", "Faction").
    /// </summary>
    [MaxLength(128), Required, Display(Name = "Identity Type Name")]
    public string IdentityName { get; set; } = "";
}

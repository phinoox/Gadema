// =============================================================================
// TemplateIdentityDefinition - Entity for identity system definitions in project templates
// =============================================================================

using Gadema.Core.Models.Identity;

namespace Gadema.Core.Models.Templates;

/// <summary>
/// Defines the identity systems (e.g., Race, Faction, Guild) that are pre-configured for a project template.
/// This ensures that any project created from this template has the appropriate identity categories ready for use.
/// </summary>
[ModelDependency(typeof(IdentityDefinition), typeof(IdentityValue), typeof(ProjectTemplate))]
public class TemplateIdentityDefinition
{
    /// <summary>
    /// Unique identifier for the template identity definition.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The ID of the project template this definition belongs to.
    /// </summary>
    [Required, Display(Name = "Project Template ID")]
    public Guid ProjectTemplateId { get; set; }

    /// <summary>
    /// Navigation property for the parent project template.
    /// </summary>
    // Navigation property: ProjectTemplate (Many-to-One)
    [ForeignKey("ProjectTemplateId")]
    public virtual ProjectTemplate ProjectTemplate { get; set; }

    /// <summary>
    /// The ID of the identity definition this configuration relates to.
    /// </summary>
    [Required, Display(Name = "Identity Definition ID")]
    public Guid IdentityDefinitionId { get; set; }

    /// <summary>
    /// Navigation property for the associated identity definition.
    /// </summary>
    // Navigation property: IdentityDefinition (Many-to-One)
    [ForeignKey("IdentityDefinitionId")]
    public virtual IdentityDefinition IdentityDefinition { get; set; }

    /// <summary>
    /// A brief description of this identity system within the template context.
    /// </summary>
    [MaxLength(128)]
    public string? Description { get; set; }

    /// <summary>
    /// The human-readable name for the identity type (e.g., "Race", "Faction").
    /// </summary>
    [MaxLength(128), Required, Display(Name = "Identity Type Name")]
    public string IdentityName { get; set; } = "";
}


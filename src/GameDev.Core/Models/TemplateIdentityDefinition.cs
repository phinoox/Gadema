// =============================================================================
// TemplateIdentityDefinition - Entity for identity system definitions in project templates
// =============================================================================

using System.ComponentModel.DataAnnotations;

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
    
    [Required, Display(Name = "Identity Definition ID")]
    public Guid IdentityDefinitionId { get; set; }
    
    [MaxLength(128)]
    public string? Description { get; set; }
}

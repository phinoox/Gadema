// =============================================================================
// ProjectIdentityDefinition - Entity for identity definitions linked to projects
// =============================================================================

using System.ComponentModel.DataAnnotations;

namespace GameDev.Core.Models;

/// <summary>
/// Definition of identity types (race, faction, alignment, guild) for a project.
/// Configures what character identities can be selected within this specific project.
/// </summary>
public class ProjectIdentityDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }
    
    public int IdentityType { get; set; }  // Enum: Race, Faction, Alignment, Guild
    
    [MaxLength(128), Required, Display(Name = "Identity Type Name")]
    public string IdentityTypeName { get; set; } = "";
    
    public bool IsRequired { get; set; } = false;
    
    [MaxLength(256)]
    public string? DefaultValue { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    [MaxLength(1024)]
    public string? Description { get; set; }
}

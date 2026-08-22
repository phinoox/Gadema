// =============================================================================
// IdentityDefinition - Base entity for identity definitions linked to projects
// This is the base class used by IdentityValue navigation property
// =============================================================================

using System.ComponentModel.DataAnnotations;

namespace GameDev.Core.Models;

/// <summary>
/// Definition of identity types (race, faction, alignment, guild) for a project.
/// Configures what character identities can be selected within this specific project.
/// </summary>
public class IdentityDefinition
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

// Keep ProjectIdentityDefinition for specific project-scoped identity definitions
public class ProjectIdentityDefinition : IdentityDefinition
{
    // Inherits from IdentityDefinition but can add project-specific properties if needed
}
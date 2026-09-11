// =============================================================================
// IdentityDefinition - Base entity for identity definitions linked to projects
// This is the base class used by IdentityValue navigation property
// =============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gadema.Core.Enums;
using Gadema.Core.Models.Projects;

namespace Gadema.Core.Models;

/// <summary>
/// Definition of identity types (race, faction, alignment, guild) for a project.
/// Configures what character identities can be selected within this specific project.
/// </summary>
[ModelDependency(typeof(RootMarker))]
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
[ModelDependency(typeof(IdentityDefinition),typeof(IdentityValue))]
public class ProjectIdentityDefinition 
{

    public Guid Id { get; set; } = Guid.NewGuid();
    // Add these properties at the end of the class:

    [EnumDataType(typeof(IdentityTypeEnum)), Required, Display(Name = "Identity Type")]
    public int IdentityType { get; set; }

    /// <summary>
    /// Name of the identity (e.g., "Human", "Elf").
    /// </summary>
    [MaxLength(128), Required, Display(Name = "Identity Name")]
    public string IdentityName { get; set; } = "";

    /// <summary>
    /// FK to Project.
    /// </summary>
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }

    // Navigation property: Project
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }
}
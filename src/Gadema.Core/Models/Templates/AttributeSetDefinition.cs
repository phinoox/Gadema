// =============================================================================
// AttributeSetDefinition - Entity for attribute set definitions in project templates
// =============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gadema.Core.Models;

/// <summary>
/// Definition of attribute sets for project templates.
/// Stores the structure and formulas for character ability systems.
/// </summary>
[DependencyResolver.ModelDependency(typeof(ProjectTemplate))]
public class AttributeSetDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Project Template ID")]
    public Guid ProjectTemplateId { get; set; }

    // Navigation property: ProjectTemplate (Many-to-One)
    [ForeignKey("ProjectTemplateId")]
    public virtual ProjectTemplate ProjectTemplate { get; set; }
    
    [Required, MaxLength(128), Display(Name = "Attribute Set Name")]
    public string AttributeSetName { get; set; } = "";
    
    public int BaseLevel { get; set; } = 1;
    
    public bool IsDefaultSet { get; set; } = true;
    
    [MaxLength(4096)]
    public string? Description { get; set; }
}

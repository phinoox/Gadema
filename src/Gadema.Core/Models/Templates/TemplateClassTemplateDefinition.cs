// =============================================================================
// TemplateClassTemplateDefinition - Entity for class template configurations in project templates
// =============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gadema.Core.Models;

/// <summary>
/// Definition of character class templates for project templates.
/// Stores class structure information for game templates.
/// </summary>
[DependencyResolver.ModelDependency(typeof(ClassTemplate),typeof(AttributeDefinition))]
public class TemplateClassTemplateDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Project Template ID")]
    public Guid ProjectTemplateId { get; set; }

    // Navigation property for ProjectTemplate (Many-to-One)
    [ForeignKey("ProjectTemplateId")]
    public virtual ProjectTemplate ProjectTemplate { get; set; }
    
    [Required, MaxLength(128), Display(Name = "Class Template Name")]
    public string ClassTemplateName { get; set; } = "";
    
    public int BaseLevel { get; set; } = 1;
    
    public int? MaxLevel { get; set; }
    
    [MaxLength(1024)]
    public string? Description { get; set; }

}

// =============================================================================
// TemplateClassTemplateDefinition - Entity for class template configurations in project templates
// =============================================================================

using System.ComponentModel.DataAnnotations;

namespace GameDev.Core.Models;

/// <summary>
/// Definition of character class templates for project templates.
/// Stores class structure information for game templates.
/// </summary>
public class TemplateClassTemplateDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Project Template ID")]
    public Guid ProjectTemplateId { get; set; }
    
    [Required, MaxLength(128), Display(Name = "Class Template Name")]
    public string ClassTemplateName { get; set; } = "";
    
    public int BaseLevel { get; set; } = 1;
    
    public int? MaxLevel { get; set; }
    
    [MaxLength(1024)]
    public string? Description { get; set; }
}

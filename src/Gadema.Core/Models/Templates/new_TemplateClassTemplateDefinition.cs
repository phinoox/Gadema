using System.ComponentModel.DataAnnotations;
using Gadema.Core.Models.Game.Attributes;

namespace Gadema.Core.Models.Templates;

/// <summary>
/// Defines a template for character classes within a project template.
/// This allows for pre-configuring class structures (e.g., "Warrior", "Mage") including their level ranges and descriptions.
/// </summary>
[ModelDependency(typeof(ClassTemplate), typeof(AttributeDefinition))]
public class TemplateClassTemplateDefinition
{
    /// <summary>
    /// Unique identifier for the class template definition.
    /// </summary>
    [Key]
    public Guid Id { get; set; }
    
    /// <summary>
    /// The ID of the project template this class template belongs to.
    /// </summary>
    [Required, Display(Name = "Project Template ID")]
    public Guid ProjectTemplateId { get; set; }

    /// <summary>
    /// Navigation property for the parent project template.
    /// </summary>
    // Navigation property for ProjectTemplate (Many-to-One)
    [ForeignKey("ProjectTemplateId")]
    public virtual ProjectTemplate ProjectTemplate { get; set; }
    
    /// <summary>
    /// The human-readable name of the class template (e.g., "Paladin", "Rogue").
    /// </summary>
    [Required, MaxLength(128), Display(Name = "Class Template Name")]
    public string ClassTemplateName { get; set; } = "";
    
    /// <summary>
    /// The starting level for this class template.
    /// </summary>
    public int BaseLevel { get; set; } = 1;
    
    /// <summary>
    /// The maximum possible level for characters of this class template, if applicable.
    /// </summary>
    public int? MaxLevel { get; set; }
    
    /// <summary>
    /// A detailed description of the class role and characteristics.
    /// </summary>
    [MaxLength(1024)]
    public string? Description { get; set; }

}

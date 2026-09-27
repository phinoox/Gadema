// =============================================================================
// AttributeSetDefinition - Entity for attribute set definitions in project templates
// =============================================================================

namespace Gadema.Core.Models.Templates;

/// <summary>
/// Defines a predefined group of attributes (e.g., Strength, Dexterity, Intelligence) used in project templates.
/// This allows for templated ability systems tailored to specific genres or game mechanics.
/// </summary>
[ModelDependency(typeof(ProjectTemplate))]
public class AttributeSetDefinition
{
    /// <summary>
    /// Unique identifier for the attribute set definition.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// The ID of the project template this attribute set belongs to.
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
    /// The human-readable name of the attribute set (e.g., "Core Stats", "Magic Attributes").
    /// </summary>
    [Required, MaxLength(128), Display(Name = "Attribute Set Name")]
    public string AttributeSetName { get; set; } = "";
    
    /// <summary>
    /// The starting level or base value applied to attributes in this set.
    /// </summary>
    public int BaseLevel { get; set; } = 1;
    
    /// <summary>
    /// Indicates if this is the default attribute set for the template.
    /// </summary>
    public bool IsDefaultSet { get; set; } = true;
    
    /// <summary>
    /// A description of the purpose and usage of this attribute set.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
}


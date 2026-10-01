namespace Gadema.Core.Models.Templates;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Links a <see cref="AttributeSetDefinition"/> to its parent <see cref="ProjectTemplate"/>.
/// This entity facilitates the mapping of attribute sets within the templating system.
/// </summary>
[ModelDependency(typeof(AttributeSetDefinition), typeof(ProjectTemplate))]
public class TemplateAttributeSetDefinition
{
    /// <summary>
    /// Unique identifier for this link.
    /// </summary>
    [Key]
    public Guid Id { get; set; }
    
    /// <summary>
    /// The ID of the project template this definition belongs to.
    /// </summary>
    [Required]
    public Guid ProjectTemplateId { get; set; }

    /// <summary>
    /// Navigation property for the parent project template.
    /// </summary>
    // Navigation property for ProjectTemplate (Many-to-One)
    [ForeignKey("ProjectTemplateId")]
    public virtual ProjectTemplate ProjectTemplate { get; set; }
    
    /// <summary>
    /// The ID of the attribute set definition.
    /// </summary>
    [Required]
    public Guid AttributeSetDefinitionId { get; set; }

    /// <summary>
        /// Navigation property for the attribute set definition.
    /// </summary>
    // Navigation property for AttributeSetDefinition (Many-to-One)
    [ForeignKey("AttributeSetDefinitionId")]
    public virtual AttributeSetDefinition AttributeSetDefinition { get; set; }

    /// <summary>
    /// An optional name or label for this specific association.
    /// </summary>
    public string Name { get;  set; }
}

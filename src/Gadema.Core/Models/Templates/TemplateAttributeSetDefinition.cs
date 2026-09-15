// =============================================================================
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Templates;

/// <summary>
/// Template attribute set definition for project templates.
/// </summary>
[ModelDependency(typeof(AttributeSetDefinition),typeof(ProjectTemplate))]
public class TemplateAttributeSetDefinition
{
    /// <summary>
    /// Unique identifier for the template attribute set definition.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the project template this definition belongs to.
    /// </summary>
    [Required]
    public Guid ProjectTemplateId { get; set; }

    // Navigation property for ProjectTemplate (Many-to-One)
    [ForeignKey("ProjectTemplateId")]
    public virtual ProjectTemplate ProjectTemplate { get; set; }
    
    /// <summary>
    /// ID of the attribute set definition.
    /// </summary>
    [Required]
    public Guid AttributeSetDefinitionId { get; set; }

    // Navigation property for AttributeSetDefinition (Many-to-One)
    [ForeignKey("AttributeSetDefinitionId")]
    public virtual AttributeSetDefinition AttributeSetDefinition { get; set; }
    public string Name { get;  set; }
}
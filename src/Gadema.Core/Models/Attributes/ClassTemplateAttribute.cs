// =============================================================================
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models;

/// <summary>
/// Attribute definition for a specific class template.
/// Allows per-class overrides of base attribute formulas.
/// </summary>
public class ClassTemplateAttribute
{
    /// <summary>
    /// Unique identifier for the template attribute definition.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? ContentItemId { get; set; }  
    
    [ForeignKey("ContentItemId")]
    public virtual ContentItem? ContentItem { get; set; }

    
    /// <summary>
    /// FK to the parent class template (composite key with AttributeSetId).
    /// </summary>
    [Required, Display(Name = "Class Template")]
    public Guid ClassTemplateId { get; set; }

    // Navigation property for ClassTemplate (Many-to-One)
    [ForeignKey("ClassTemplateId")]
    public virtual ClassTemplate ClassTemplate { get; set; }
    
    /// <summary>
    /// FK to the attribute definition this applies to.
    /// </summary>
    [Required, Display(Name = "Attribute Definition")]
    public Guid AttributeDefinitionId { get; set; }

    // Navigation property for AttributeDefinition (Many-to-One)
    [ForeignKey("AttributeDefinitionId")]
    public virtual AttributeDefinition AttributeDefinition { get; set; }
    
    /// <summary>
    /// Optional formula override for this specific attribute in this class.
    /// </summary>
    [MaxLength(4096)]
    public string? OverrideFormulaExpression { get; set; }
    
    /// <summary>
    /// Default minimum value for the attribute (for template defaults).
    /// </summary>
    public decimal? DefaultMinValue { get; set; }
    
    /// <summary>
    /// Default maximum value for the attribute (for template defaults).
    /// </summary>
    public decimal? DefaultMaxValue { get; set; }

}
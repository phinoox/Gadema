// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Game.Attributes;

/// <summary>
/// Represents a specific attribute configuration for a <see cref="ClassTemplate"/>.
/// This allows individual classes to have unique scaling formulas or value constraints
/// for attributes defined in their parent <see cref="AttributeSet"/>.
/// </summary>
[ModelDependency(typeof(ClassTemplate), typeof(AttributeDefinition))]
public class ClassTemplateAttribute
{
    /// <summary>
    /// Unique identifier for the template attribute configuration.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The ID of the associated ContentMetaInfo entity, used for identity anchoring and searching.
    /// </summary>
    [Required]
    public Guid? MetaInfoId { get; set; }

    /// <summary>
    /// Navigation property for the template attribute's identity anchor.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo? ContentMetaInfo { get; set; }
    /// <summary>
    /// The ID of the parent class template this configuration applies to.
    /// </summary>
    [Required, Display(Name = "Class Template")]
    public Guid ClassTemplateId { get; set; }
    /// <summary>
    /// Navigation property for the parent class template.
    /// </summary>
    [ForeignKey("ClassTemplateId")]
    public virtual ClassTemplate ClassTemplate { get; set; }
    
    /// <summary>
    /// The ID of the attribute definition this override pertains to.
    /// </summary>
    [Required, Display(Name = "Attribute Definition")]
    public Guid AttributeDefinitionId { get; set; }

    /// <summary>
    /// Navigation property for the associated attribute definition.
    /// </summary>
    [ForeignKey("AttributeDefinitionId")]
    public virtual AttributeDefinition AttributeDefinition { get; set; }
    
    /// <summary>
    /// An optional mathematical formula that overrides the default scaling logic
    /// specifically for this class and attribute combination.
    /// </summary>
    [MaxLength(4096)]
    public string? OverrideFormulaExpression { get; set; }
    
    /// <summary>
    /// The minimum possible value for this attribute when used within this specific class template.
    /// </summary>
    public decimal? DefaultMinValue { get; set; }
    
    /// <summary>
    /// The maximum possible value for this attribute when used within this specific class template.
    /// </summary>
    public decimal? DefaultMaxValue { get; set; }

}
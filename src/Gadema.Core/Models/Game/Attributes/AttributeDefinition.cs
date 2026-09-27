// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Game.Attributes;

/// <summary>
/// Represents a blueprint for a character attribute (e.g., "Strength", "Health", "Mana").
/// Defines the core mechanics, including data type, scaling formulas, and value constraints.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo))]
public class AttributeDefinition
{
    /// <summary>
    /// Unique identifier for the attribute definition.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The ID of the associated ContentMetaInfo entity, used for identity anchoring and searching.
    /// </summary>
    [Required]
    public Guid? MetaInfoId { get; set; }
    
    /// <summary>
    /// Navigation property for the attribute's identity anchor.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo? ContentMetaInfo { get; set; }
    /// <summary>
    /// The human-readable name of the attribute (e.g., "Strength", "Agility").
    /// </summary>
    [Required]
    public string Name { get; set; } = "";
    
    /// <summary>
    /// A unique, URL-friendly slug for the attribute.
    /// </summary>
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    /// <summary>
    /// A detailed description of what this attribute represents and how it functions.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    /// <summary>
    /// The underlying numeric data type (e.g., Integer, Float, Decimal).
    /// </summary>
    public int ValueType { get; set; }  // Enum: Int, Float, Decimal
    /// <summary>
    /// A mathematical expression used for calculating attribute values dynamically.
    /// </summary>
    [MaxLength(4096)]
    public string? FormulaExpression { get; set; }
    
    /// <summary>
    /// A JSON representation of how the attribute scales across different levels.
    /// </summary>
    [MaxLength(1024)]
    public string? LevelMappingJson { get; set; }  // JSON for scaling
    /// <summary>
    /// The default minimum possible value for this attribute.
    /// </summary>
    public decimal? DefaultMin { get; set; }
    
    /// <summary>
    /// The default maximum possible value for this attribute.
    /// </summary>
    public decimal? DefaultMax { get; set; }
    
    /// <summary>
    /// The sort order used for displaying attributes in UI lists or character sheets.
    /// </summary>
    public int DisplayOrder { get; set; } = 0;
}
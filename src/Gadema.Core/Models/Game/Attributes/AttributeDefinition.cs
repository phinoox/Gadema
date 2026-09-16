// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Game.Attributes;

/// <summary>
/// Represents an attribute definition for character/class templates.
/// Used for defining ability systems and character stats with scaling formulas.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo))]
public class AttributeDefinition
{
    /// <summary>
    /// Unique identifier for the attribute definition.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? MetaInfoId { get; set; }  
    
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo? ContentMetaInfo { get; set; }

    
    /// <summary>
    /// Name of the attribute (e.g., "Health", "AttackPower").
    /// </summary>
    [Required]
    public string Name { get; set; } = "";
    
    /// <summary>
    /// URL-friendly slug for the attribute (unique).
    /// </summary>
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    /// <summary>
    /// Description of the attribute.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Type of value (Int, Float, Decimal).
    /// </summary>
    public int ValueType { get; set; }  // Enum: Int, Float, Decimal
    
    /// <summary>
    /// Formula expression for calculating attribute values.
    /// </summary>
    [MaxLength(4096)]
    public string? FormulaExpression { get; set; }
    
    /// <summary>
    /// Level mapping JSON for scaling per level.
    /// </summary>
    [MaxLength(1024)]
    public string? LevelMappingJson { get; set; }  // JSON for scaling
    
    /// <summary>
    /// Default minimum value.
    /// </summary>
    public decimal? DefaultMin { get; set; }
    
    /// <summary>
    /// Default maximum value.
    /// </summary>
    public decimal? DefaultMax { get; set; }
    
    /// <summary>
    /// Display order for sorting.
    /// </summary>
    public int DisplayOrder { get; set; } = 0;
}
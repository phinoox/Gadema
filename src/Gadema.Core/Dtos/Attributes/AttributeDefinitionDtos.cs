// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Attributes;

/// <summary>
/// DTO for creating an attribute definition.
/// </summary>
public class AttributeDefinitionCreateDto
{
    /// <summary>
    /// ID of the content item this attribute belongs to (optional).
    /// </summary>
    public Guid? MetaInfoId { get; set; }

    /// <summary>
    /// Name of the attribute.
    /// </summary>
    [Required, MaxLength(128)]
    public string Name { get; set; } = "";

    /// <summary>
    /// URL-friendly slug.
    /// </summary>
    [MaxLength(128)]
    public string? Slug { get; set; }

    /// <summary>
    /// Description of the attribute.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }

    /// <summary>
    /// Type of value (Int, Float, Decimal).
    /// </summary>
    public int ValueType { get; set; }

    /// <summary>
    /// Formula expression for calculating attribute values.
    /// </summary>
    [MaxLength(4096)]
    public string? FormulaExpression { get; set; }

    /// <summary>
    /// Level mapping JSON for scaling per level.
    /// </summary>
    [MaxLength(1024)]
    public string? LevelMappingJson { get; set; }

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

/// <summary>
/// Response DTO for an attribute definition.
/// </summary>
public class AttributeDefinitionResponseDto
{
    public Guid Id { get; set; }
    public Guid? MetaInfoId { get; set; }
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string? Description { get; set; }
    public int ValueType { get; set; }
    public string? FormulaExpression { get; set; }
    public string? LevelMappingJson { get; set; }
    public decimal? DefaultMin { get; set; }
    public decimal? DefaultMax { get; set; }
    public int DisplayOrder { get; set; }
}

/// <summary>
/// List response for attribute definitions.
/// </summary>
public class AttributeDefinitionListResponseDto
{
    public IEnumerable<AttributeDefinitionResponseDto> Items { get; set; } = Enumerable.Empty<AttributeDefinitionResponseDto>();
    public int TotalCount { get; set; }
}

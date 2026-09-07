// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Attributes;

/// <summary>
/// DTO for creating a class template attribute override.
/// </summary>
public class ClassTemplateAttributeCreateDto
{
    /// <summary>
    /// ID of the class template this attribute belongs to.
    /// </summary>
    [Required]
    public Guid ClassTemplateId { get; set; }

    /// <summary>
    /// ID of the attribute definition being overridden.
    /// </summary>
    [Required]
    public Guid AttributeDefinitionId { get; set; }

    /// <summary>
    /// Override formula expression for this specific attribute in this class.
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

/// <summary>
/// Response DTO for a class template attribute.
/// </summary>
public class ClassTemplateAttributeResponseDto
{
    public Guid Id { get; set; }
    public Guid ClassTemplateId { get; set; }
    public string ClassTemplateName { get; set; } = "";
    public Guid AttributeDefinitionId { get; set; }
    public string AttributeName { get; set; } = "";
    public string? OverrideFormulaExpression { get; set; }
    public decimal? DefaultMinValue { get; set; }
    public decimal? DefaultMaxValue { get; set; }
}

/// <summary>
/// List response for class template attributes.
/// </summary>
public class ClassTemplateAttributeListResponseDto
{
    public IEnumerable<ClassTemplateAttributeResponseDto> Items { get; set; } = Enumerable.Empty<ClassTemplateAttributeResponseDto>();
    public int TotalCount { get; set; }
}

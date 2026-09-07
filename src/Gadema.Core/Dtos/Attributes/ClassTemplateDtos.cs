// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Attributes;

/// <summary>
/// DTO for creating a class template.
/// </summary>
public class ClassTemplateCreateDto
{
    /// <summary>
    /// ID of the content item this class template belongs to (optional).
    /// </summary>
    public Guid? MetaInfoId { get; set; }

    /// <summary>
    /// Name of the class.
    /// </summary>
    [Required, MaxLength(128)]
    public string Name { get; set; } = "";

    /// <summary>
    /// Description of the class and its role.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }

    /// <summary>
    /// ID of the attribute set this template belongs to.
    /// </summary>
    [Required]
    public Guid AttributeSetId { get; set; }

    /// <summary>
    /// Base level for all attributes in this class.
    /// </summary>
    public int BaseLevel { get; set; } = 1;

    /// <summary>
    /// Maximum level this class can reach.
    /// </summary>
    public int? MaxLevel { get; set; }
}

/// <summary>
/// Response DTO for a class template.
/// </summary>
public class ClassTemplateResponseDto
{
    public Guid Id { get; set; }
    public Guid? MetaInfoId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public Guid AttributeSetId { get; set; }
    public int BaseLevel { get; set; }
    public int? MaxLevel { get; set; }
}

/// <summary>
/// List response for class templates.
/// </summary>
public class ClassTemplateListResponseDto
{
    public IEnumerable<ClassTemplateResponseDto> Items { get; set; } = Enumerable.Empty<ClassTemplateResponseDto>();
    public int TotalCount { get; set; }
}

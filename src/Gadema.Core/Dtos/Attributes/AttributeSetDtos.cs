// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Attributes;

/// <summary>
/// DTO for creating an attribute set.
/// </summary>
public class AttributeSetCreateDto
{
    /// <summary>
    /// ID of the content item this attribute set belongs to (optional).
    /// </summary>
    public Guid? MetaInfoId { get; set; }

    /// <summary>
    /// ID of the project this attribute set belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Name of the attribute set.
    /// </summary>
    [Required, MaxLength(128)]
    public string Name { get; set; } = "";

    /// <summary>
    /// Display order for sorting.
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// Whether the attribute set is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Response DTO for an attribute set.
/// </summary>
public class AttributeSetResponseDto
{
    public Guid Id { get; set; }
    public Guid? MetaInfoId { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = "";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// List response for attribute sets.
/// </summary>
public class AttributeSetListResponseDto
{
    public IEnumerable<AttributeSetResponseDto> Items { get; set; } = Enumerable.Empty<AttributeSetResponseDto>();
    public int TotalCount { get; set; }
}

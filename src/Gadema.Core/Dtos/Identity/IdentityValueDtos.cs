// =============================================================================
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Enums;

namespace Gadema.Core.Dtos.Identity;

/// <summary>
/// DTO for creating an identity value (specific race, faction option, etc.).
/// </summary>
public class IdentityValueUpdateDto
{
    /// <summary>
    /// ID of the identity definition this value belongs to.
    /// </summary>
    [Required]
    public Guid IdentityDefinitionId { get; set; }

    /// <summary>
    /// ID of the project this value belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Display name for this identity value (e.g., "Human", "Elf").
    /// </summary>
    [Required, MaxLength(128)]
    public string Name { get; set; } = "";

    /// <summary>
    /// URL-friendly slug.
    /// </summary>
    [MaxLength(128)]
    public string? Slug { get; set; }

    /// <summary>
    /// Description of this identity value.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }

    /// <summary>
    /// Display order index.
    /// </summary>
    public int OrderIndex { get; set; } = 0;

    /// <summary>
    /// Whether this is a default/selected value.
    /// </summary>
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// The actual identity value text (e.g., "Human Male").
    /// </summary>
    [MaxLength(4096)]
    public string? Value { get; set; }

    /// <summary>
    /// Whether this identity is required for characters.
    /// </summary>
    public bool IsRequired { get; set; } = false;
}

/// <summary>
/// Response DTO for an identity value.
/// </summary>
public class IdentityValueResponseDto
{
    public Guid Id { get; set; }
    public Guid IdentityDefinitionId { get; set; }
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public bool IsDefault { get; set; }
    public Guid ProjectId { get; set; }
    public string? Value { get; set; }
    public int IdentityTypeId { get; set; }
    public bool IsRequired { get; set; }
}

/// <summary>
/// List response for identity values.
/// </summary>
public class IdentityValueListResponseDto
{
    public IEnumerable<IdentityValueResponseDto> Items { get; set; } = Enumerable.Empty<IdentityValueResponseDto>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

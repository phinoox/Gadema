// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Abilities;

/// <summary>
/// DTO for creating an ability set.
/// </summary>
public class AbilitySetCreateDto
{
    /// <summary>
    /// ID of the content item this ability set belongs to (optional).
    /// </summary>
    public Guid? MetaInfoId { get; set; }

    /// <summary>
    /// ID of the project this ability set belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Name of the ability set.
    /// </summary>
    [Required, MaxLength(128)]
    public string Name { get; set; } = "";

    /// <summary>
    /// URL-friendly slug.
    /// </summary>
    [MaxLength(128)]
    public string? Slug { get; set; }

    /// <summary>
    /// Description of the ability set.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }

    /// <summary>
    /// Type (Combat, Non-Combat, Hybrid).
    /// </summary>
    public int Type { get; set; }

    /// <summary>
    /// Whether the ability set is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Response DTO for an ability set.
/// </summary>
public class AbilitySetResponseDto
{
    public Guid Id { get; set; }
    public Guid? MetaInfoId { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string? Description { get; set; }
    public int Type { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// List response for ability sets.
/// </summary>
public class AbilitySetListResponseDto
{
    public IEnumerable<AbilitySetResponseDto> Items { get; set; } = Enumerable.Empty<AbilitySetResponseDto>();
    public int TotalCount { get; set; }
}

// =============================================================================
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Enums;

namespace Gadema.Core.Dtos.Abilities;

/// <summary>
/// DTO for creating an ability definition.
/// </summary>
public class AbilityDefinitionCreateDto
{
    /// <summary>
    /// ID of the content item this ability belongs to (optional).
    /// </summary>
    public Guid? MetaInfoId { get; set; }

    /// <summary>
    /// Name of the ability.
    /// </summary>
    [Required, MaxLength(128)]
    public string Name { get; set; } = "";

    /// <summary>
    /// URL-friendly slug.
    /// </summary>
    [MaxLength(128)]
    public string? Slug { get; set; }

    /// <summary>
    /// Description of the ability.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }

    /// <summary>
    /// Type (Attack, Defense, Buff, Debuff).
    /// </summary>
    public int AbilityType { get; set; }

    /// <summary>
    /// Cooldown in seconds.
    /// </summary>
    public decimal? CooldownSeconds { get; set; }

    /// <summary>
    /// Resource cost (mana, energy, etc.).
    /// </summary>
    public decimal? ResourceCost { get; set; }

    /// <summary>
    /// Maximum level for the ability.
    /// </summary>
    public int? MaxLevel { get; set; }

    /// <summary>
    /// Scaling formula JSON per level.
    /// </summary>
    [MaxLength(1024)]
    public string? ScalingFormulaJson { get; set; }

    /// <summary>
    /// Required flag condition for the ability.
    /// </summary>
    [MaxLength(512)]
    public string? RequiresFlagCondition { get; set; }
}

/// <summary>
/// Response DTO for an ability definition.
/// </summary>
public class AbilityDefinitionResponseDto
{
    public Guid Id { get; set; }
    public Guid? MetaInfoId { get; set; }
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string? Description { get; set; }
    public int AbilityType { get; set; }
    public decimal? CooldownSeconds { get; set; }
    public decimal? ResourceCost { get; set; }
    public int? MaxLevel { get; set; }
    public string? ScalingFormulaJson { get; set; }
    public string? RequiresFlagCondition { get; set; }
}

/// <summary>
/// List response for ability definitions.
/// </summary>
public class AbilityDefinitionListResponseDto
{
    public IEnumerable<AbilityDefinitionResponseDto> Items { get; set; } = Enumerable.Empty<AbilityDefinitionResponseDto>();
    public int TotalCount { get; set; }
}

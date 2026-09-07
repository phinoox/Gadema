// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Abilities;

/// <summary>
/// DTO for creating a status effect definition.
/// </summary>
public class StatusEffectDefinitionCreateDto
{
    /// <summary>
    /// ID of the content item this status effect belongs to (optional).
    /// </summary>
    public Guid? MetaInfoId { get; set; }

    /// <summary>
    /// Name of the status effect.
    /// </summary>
    [Required, MaxLength(128)]
    public string Name { get; set; } = "";

    /// <summary>
    /// URL-friendly slug.
    /// </summary>
    [MaxLength(128)]
    public string? Slug { get; set; }

    /// <summary>
    /// Description of the status effect.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }

    /// <summary>
    /// Type (Buff, Debuff, Neutral).
    /// </summary>
    public int EffectType { get; set; }

    /// <summary>
    /// Duration in seconds.
    /// </summary>
    public decimal? DurationSeconds { get; set; }

    /// <summary>
    /// Damage per tick (for DoT effects).
    /// </summary>
    public decimal? DamagePerTick { get; set; }

    /// <summary>
    /// Required condition for the effect.
    /// </summary>
    [MaxLength(512)]
    public string? RequiresCondition { get; set; }
}

/// <summary>
/// Response DTO for a status effect definition.
/// </summary>
public class StatusEffectDefinitionResponseDto
{
    public Guid Id { get; set; }
    public Guid? MetaInfoId { get; set; }
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string? Description { get; set; }
    public int EffectType { get; set; }
    public decimal? DurationSeconds { get; set; }
    public decimal? DamagePerTick { get; set; }
    public string? RequiresCondition { get; set; }
}

/// <summary>
/// List response for status effect definitions.
/// </summary>
public class StatusEffectDefinitionListResponseDto
{
    public IEnumerable<StatusEffectDefinitionResponseDto> Items { get; set; } = Enumerable.Empty<StatusEffectDefinitionResponseDto>();
    public int TotalCount { get; set; }
}

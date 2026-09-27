// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Game.Abilities;

/// <summary>
/// Represents a blueprint for a status effect (e.g., "Burning", "Stunned").
/// Defines how the effect behaves, including its type, duration, and potential damage per tick.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo))]
public class StatusEffectDefinition
{
    /// <summary>
    /// Unique identifier for the status effect definition.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The ID of the associated ContentMetaInfo entity, used for identity anchoring and searching.
    /// </summary>
    [Required]
    public Guid? MetaInfoId { get; set; }
    
    /// <summary>
    /// Navigation property for the status effect's identity anchor.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo? ContentMetaInfo { get; set; }
    /// <summary>
    /// The human-readable name of the status effect (e.g., "Poisoned", "Haste").
    /// </summary>
    [Required]
    public string Name { get; set; } = "";
    
    /// <summary>
    /// A unique, URL-friendly slug for the status effect.
    /// </summary>
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    /// <summary>
    /// A detailed description of the status effect and its impact.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    /// <summary>
    /// The functional type of the effect (e.g., Buff, Debuff, or Neutral).
    /// </summary>
    public int EffectType { get; set; }  // Enum: Buff, Debuff, Neutral
    /// <summary>
    /// The intended duration of the status effect in seconds.
    /// </summary>
    public decimal? DurationSeconds { get; set; }
    
    /// <summary>
    /// The amount of damage dealt per tick for Damage-over-Time (DoT) effects.
    /// </summary>
    public decimal? DamagePerTick { get; set; }
    
    /// <summary>
    /// An optional requirement or condition that must be met for the effect to trigger or persist.
    /// </summary>
    [MaxLength(512)]
    public string? RequiresCondition { get; set; }
}
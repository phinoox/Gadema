// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Game.Abilities;

/// <summary>
/// Represents a blueprint for an individual ability (e.g., "Fireball", "Shield Bash").
/// Defines the core mechanics, including type, resource cost, cooldowns, and scaling logic.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo))]
public class AbilityDefinition
{
    /// <summary>
    /// Unique identifier for the ability definition.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The ID of the associated ContentMetaInfo entity, used for identity anchoring and searching.
    /// </summary>
    [Required]
    public Guid? MetaInfoId { get; set; }
    
    /// <summary>
    /// Navigation property for the ability's identity anchor.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo? ContentMetaInfo { get; set; }
    /// <summary>
    /// The human-readable name of the ability.
    /// </summary>
    [Required]
    public string Name { get; set; } = "";
    
    /// <summary>
    /// A unique, URL-friendly slug for the ability.
    /// </summary>
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    /// <summary>
    /// A detailed description of what the ability does and its narrative flavor.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    /// <summary>
    /// The functional type of the ability (e.g., Attack, Defense, Buff, Debuff).
    /// </summary>
    public int AbilityType { get; set; }  // Enum: Attack, Defense, Buff, Debuff
    /// <summary>
    /// The cooldown duration in seconds before the ability can be used again.
    /// </summary>
    public decimal? CooldownSeconds { get; set; }
    
    /// <summary>
    /// The resource cost (e.g., mana, stamina) required to activate the ability.
    /// </summary>
    public decimal? ResourceCost { get; set; }
    /// <summary>
    /// The maximum level this ability can reach through progression.
    /// </summary>
    public int? MaxLevel { get; set; }
    
    /// <summary>
    /// A JSON representation of the scaling formulas used to determine power/effect per level.
    /// </summary>
    [MaxLength(1024)]
    public string? ScalingFormulaJson { get; set; }  // JSON for scaling per level
    
    /// <summary>
    /// An optional flag-based condition that must be met to use this ability (e.g., "HasMagic").
    /// </summary>
    [MaxLength(512)]
    public string? RequiresFlagCondition { get; set; }
}
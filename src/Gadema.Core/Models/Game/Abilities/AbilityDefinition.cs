// =============================================================================
using Gadema.Core.Models.Base.MetaInfo;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Game.Abilities;

/// <summary>
/// Represents an ability definition for character/class templates.
/// Used for defining individual abilities with scaling formulas (GAS-like architecture).
/// </summary>
[ModelDependency(typeof(ContentMetaInfo))]
public class AbilityDefinition
{
    /// <summary>
    /// Unique identifier for the ability definition.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? MetaInfoId { get; set; }  
    
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo? ContentMetaInfo { get; set; }

    
    /// <summary>
    /// Name of the ability (e.g., "Fireball", "Heal").
    /// </summary>
    [Required]
    public string Name { get; set; } = "";
    
    /// <summary>
    /// URL-friendly slug for the ability (unique).
    /// </summary>
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    /// <summary>
    /// Description of the ability.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Type: 0=Attack, 1=Defense, 2=Buff, 3=Debuff.
    /// </summary>
    public int AbilityType { get; set; }  // Enum: Attack, Defense, Buff, Debuff
    
    /// <summary>
    /// Cooldown in seconds.
    /// </summary>
    public decimal? CooldownSeconds { get; set; }
    
    /// <summary>
    /// Resource cost (e.g., mana, energy).
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
    public string? ScalingFormulaJson { get; set; }  // JSON for scaling per level
    
    /// <summary>
    /// Required flag condition.
    /// </summary>
    [MaxLength(512)]
    public string? RequiresFlagCondition { get; set; }
}
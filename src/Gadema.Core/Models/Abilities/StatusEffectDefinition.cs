// =============================================================================
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models;

/// <summary>
/// Represents a status effect definition for character/class templates.
/// Used for defining buffs, debuffs, and other status effects (GAS-like architecture).
/// </summary>
[DependencyResolver.ModelDependency(typeof(ContentItem))]
public class StatusEffectDefinition
{
    /// <summary>
    /// Unique identifier for the status effect definition.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? ContentItemId { get; set; }  
    
    [ForeignKey("ContentItemId")]
    public virtual ContentItem? ContentItem { get; set; }

    
    /// <summary>
    /// Name of the status effect (e.g., "Burning", "Frozen").
    /// </summary>
    [Required]
    public string Name { get; set; } = "";
    
    /// <summary>
    /// URL-friendly slug for the status effect (unique).
    /// </summary>
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    /// <summary>
    /// Description of the status effect.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Type: 0=Buff, 1=Debuff, 2=Neutral.
    /// </summary>
    public int EffectType { get; set; }  // Enum: Buff, Debuff, Neutral
    
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
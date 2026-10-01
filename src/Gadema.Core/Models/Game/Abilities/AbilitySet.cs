namespace Gadema.Core.Models.Game.Abilities;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a collection of abilities grouped together (e.g., a "Mage Ability Set").
/// This allows for defining complete skill sets for character classes or templates.
/// </summary>
[ModelDependency(typeof(Project), typeof(ContentMetaInfo))]
public class AbilitySet
{
    /// <summary>
    /// Unique identifier for the ability set.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the associated ContentMetaInfo entity, used for identity anchoring and searching.
    /// </summary>
    [Required]
    public Guid? MetaInfoId { get; set; }
    
    /// <summary>
    /// Navigation property for the ability set's identity anchor.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo? ContentMetaInfo { get; set; }

    /// <summary>
    /// The human-readable name of the ability set (e.g., "Warrior Skills").
    /// </summary>
    [Required]
    public string Name { get; set; } = "";
    
    /// <summary>
    /// A unique, URL-friendly slug for the ability set.
    /// </summary>
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";

    /// <summary>
    /// A detailed description of what this set contains and its intended use.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    /// <summary>
    /// The ID of the project this ability set belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Navigation property for the parent project.
    /// </summary>
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }
    
    /// <summary>
    /// The functional category of the ability set (e.g., Combat, Non-Combat, Hybrid).
    /// </summary>
    public int Type { get; set; }  // Enum: Combat, Non-Combat, Hybrid
    
    /// <summary>
    /// Indicates whether this ability set is currently active and available for use.
    /// </summary>
    public bool IsActive { get; set; } = true;
}

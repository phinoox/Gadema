// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Game.Attributes;

/// <summary>
/// Defines a template for a character class (e.g., "Warrior", "Mage") within the game's scaling system.
/// This links a class name and description to a specific <see cref="AttributeSet"/>,
/// establishing the baseline stats and level progression for that archetype.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo), typeof(AttributeSet))]
public class ClassTemplate
{
    /// <summary>
    /// Unique identifier for the class template.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The ID of the associated ContentMetaInfo entity, used for identity anchoring and searching.
    /// </summary>
    [Required]
    public Guid? MetaInfoId { get; set; }
    
    /// <summary>
    /// Navigation property for the class template's identity anchor.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo? ContentMetaInfo { get; set; }
    /// <summary>
    /// The human-readable name of the class (e.g., "Warrior", "Mage").
    /// </summary>
    [Required]
    public string Name { get; set; } = "";
    /// <summary>
    /// A detailed description of the class's role, abilities, and flavor.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    /// <summary>
    /// The ID of the attribute set that defines this class's base stats.
    /// </summary>
    [Required, Display(Name = "Attribute Set")]
    public Guid AttributeSetId { get; set; }

    /// <summary>
    /// Navigation property for the associated attribute set.
    /// </summary>
    [ForeignKey("AttributeSetId")]
    public virtual AttributeSet AttributeSet { get; set; }
    
    /// <summary>
    /// The starting level applied to all attributes within this class template.
    /// </summary>
    public int BaseLevel { get; set; } = 1;
    
    /// <summary>
    /// The maximum possible level for characters of this class.
    /// </summary>
    public int? MaxLevel { get; set; }
}
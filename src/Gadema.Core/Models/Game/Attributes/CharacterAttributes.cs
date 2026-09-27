// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Game.Attributes;

/// <summary>
/// Represents the current concrete values for a specific attribute on a character.
/// This entity tracks how an <see cref="AttributeDefinition"/> is manifested in a specific
/// individual, including whether the value is derived from template scaling or manually overridden.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo), typeof(AttributeDefinition))]
public class CharacterAttributes
{
    /// <summary>
    /// The ID of the associated ContentMetaInfo entity (the character's identity anchor).
    /// Used as part of the composite primary key.
    /// </summary>
    public Guid MetaInfoId { get; set; }

    /// <summary>
    /// Navigation property for the character's identity anchor.
    /// </summary>
    // Navigation property for ContentMetaInfo (Many-to-One)
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; }
    
    /// <summary>
    /// The ID of the attribute definition this value pertains to.
    /// </summary>
    [Required, Display(Name = "Attribute Definition")]
    public Guid AttributeDefinitionId { get; set; }

    /// <summary>
    /// Navigation property for the attribute definition.
    /// </summary>
    // Navigation property for AttributeDefinition (Many-to-One)
    [ForeignKey("AttributeDefinitionId")]
    public virtual AttributeDefinition AttributeDefinition { get; set; }
    /// <summary>
    /// The current numeric value of this specific attribute for the character.
    /// </summary>
    public decimal? CurrentValue { get; set; }
    
    /// <summary>
    /// Indicates if this value was automatically derived from the template's scaling formula.
    /// </summary>
    public bool CalculatedFromTemplate { get; set; } = true;
    
    /// <summary>
    /// Indicates if a custom manual override has been applied, bypassing the default scaling logic.
    /// </summary>
    public bool OverridesFormula { get; set; } = false;
}
// =============================================================================
using Gadema.Core.Models.Base.MetaInfo;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Game.Attributes;

/// <summary>
/// Character attributes stored as a child entity.
/// Uses MetaInfoId and AttributeDefinitionId as composite primary key.
/// Stores calculated or manually set attribute values per character.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo), typeof(AttributeDefinition))]
public class CharacterAttributes
{
    /// <summary>
    /// FK to the content item this belongs to (used as Primary Key).
    /// </summary>
    public Guid MetaInfoId { get; set; }

    // Navigation property for ContentMetaInfo (Many-to-One)
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; }
    
    /// <summary>
    /// FK to the attribute definition being stored.
    /// </summary>
    [Required, Display(Name = "Attribute Definition")]
    public Guid AttributeDefinitionId { get; set; }

    // Navigation property for AttributeDefinition (Many-to-One)
    [ForeignKey("AttributeDefinitionId")]
    public virtual AttributeDefinition AttributeDefinition { get; set; }
    
    /// <summary>
    /// Current value of this attribute (can be null).
    /// </summary>
    public decimal? CurrentValue { get; set; }
    
    /// <summary>
    /// Indicates if the value was calculated from the template.
    /// </summary>
    public bool CalculatedFromTemplate { get; set; } = true;
    
    /// <summary>
    /// Indicates if a custom formula overrides the default scaling.
    /// </summary>
    public bool OverridesFormula { get; set; } = false;
}
// =============================================================================
// CharacterIdentity - Entity for character identity assignments (race, faction, guild)
// =============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gadema.Core.Enums;

namespace Gadema.Core.Models;

/// <summary>
/// Identity assignment for a specific character/content item.
/// Links a content item with selected identity definitions and values.
/// </summary>
public class CharacterIdentity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Content Item ID")]
    public Guid ContentItemId { get; set; }

    // Navigation property for ContentItem (Many-to-One)
    [ForeignKey("ContentItemId")]
    public virtual ContentItem ContentItem { get; set; }
    
    public Guid? IdentityDefinitionId { get; set; }  // Nullable FK to IdentityDefinition

    // Navigation property for IdentityDefinition (Many-to-One)
    [ForeignKey("IdentityDefinitionId")]
    public virtual IdentityDefinition? IdentityDefinition { get; set; }
    
    public Guid? IdentityValueId { get; set; }  // Nullable FK to IdentityValue selected

    // Navigation property for IdentityValue (Many-to-One)
    [ForeignKey("IdentityValueId")]
    public virtual IdentityValue? IdentityValue { get; set; }
    
    [MaxLength(1024)]
    public string? DisplayText { get; set; }  // e.g., "Human Male"

    // ⬇️ ADD THESE NEW PROPERTIES ⬇️
    
    [EnumDataType(typeof(IdentityTypeEnum)), Required, Display(Name = "Identity Type")]
    public int IdentityType { get; set; }  // FK to IdentityDefinition.IdentityType
    
    [Display(Name = "Identity Type ID")]
    public Guid? IdentityTypeId { get; set; } 
    
    [Display(Name = "Is Primary?")]
    public bool IsPrimary { get; set; } = false;  // e.g., for race (primary) vs alignment (secondary)
}
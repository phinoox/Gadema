// =============================================================================
// CharacterIdentity - Entity for character identity assignments (race, faction, guild)
// =============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameDev.Core.Models;

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
    
    public Guid? IdentityDefinitionId { get; set; }  // Nullable if project doesn't require identity

    // Navigation property for IdentityDefinition (Many-to-One)
    [ForeignKey("IdentityDefinitionId")]
    public virtual IdentityDefinition? IdentityDefinition { get; set; }
    
    public Guid? IdentityValueId { get; set; }  // Nullable FK to IdentityValue selected for character

    // Navigation property for IdentityValue (Many-to-One)
    [ForeignKey("IdentityValueId")]
    public virtual IdentityValue? IdentityValue { get; set; }
    
    [MaxLength(1024)]
    public string? DisplayText { get; set; }  // e.g., "Human Male" for race+gender combo

}

// =============================================================================
// CharacterIdentity - Entity for character identity assignments (race, faction, guild)
// =============================================================================

using System.ComponentModel.DataAnnotations;

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
    
    public Guid? IdentityDefinitionId { get; set; }  // Nullable if project doesn't require identity
    
    public Guid? IdentityValueId { get; set; }  // Nullable FK to IdentityValue selected for character
    
    [MaxLength(1024)]
    public string? DisplayText { get; set; }  // e.g., "Human Male" for race+gender combo
}

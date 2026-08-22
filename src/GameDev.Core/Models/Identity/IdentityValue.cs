// =============================================================================
// IdentityValue - Entity for selectable identity values (specific races, factions, etc.)
// =============================================================================

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameDev.Core.Models;

/// <summary>
/// Specific identity values that can be selected for character identities.
/// Contains all the available options (e.g., "Human", "Elf", "Orc" for race selection).
/// </summary>
public class IdentityValue
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required, Display(Name = "Identity Definition ID")]
    public Guid IdentityDefinitionId { get; set; }

    // Navigation property: IdentityDefinition (Many-to-One)
    [ForeignKey("IdentityDefinitionId")]
    public virtual IdentityDefinition IdentityDefinition { get; set; }
    
    [MaxLength(128), Required, Display(Name = "Value Name")]
    public string Name { get; set; } = "";
    
    [Column("slug"), MaxLength(128)]
    public string Slug { get; set; } = "";
    
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    public int OrderIndex { get; set; } = 0;
    
    public bool IsDefault { get; set; } = false;

}

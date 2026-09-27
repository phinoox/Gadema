// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

using Gadema.Core.Models.Base.Projects;

namespace Gadema.Core.Models.Game.Inventory;

/// <summary>
/// Represents a collectible, key, achievement, or currency item within a project's inventory system.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo))]
public class InventoryItem
{
    /// <summary>
    /// Unique identifier for the inventory item.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The ID of the associated ContentMetaInfo entity, used for identity anchoring and searching.
    /// </summary>
    [Required]
    public Guid? MetaInfoId { get; set; }

    /// <summary>
    /// Navigation property for the inventory item's identity anchor.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo? ContentMetaInfo { get; set; }
    /// <summary>
    /// The ID of the project this inventory item belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }
    /// <summary>
        /// Navigation property for the parent project.
    /// </summary>
    // Navigation property for Project (Many-to-One)
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; }
    
    /// <summary>
    /// The human-readable name of the item (e.g., "Golden Key", "Dragon Scale").
    /// </summary>
    [MaxLength(128), Required]
    public string ItemName { get; set; } = "";
    
    /// <summary>
    /// The functional category of the item (e.g., Collectible, Key, Achievement, Currency).
    /// </summary>
    public int ItemType { get; set; }  // Enum: Collectible, Key, Achievement, Currency
    
    /// <summary>
    /// The current quantity or amount of this item held/available.
    /// </summary>
    public int Quantity { get; set; }
    
    /// <summary>
    /// Indicates if the inventory item is officially published and available in the game world.
    /// </summary>
    public bool Published { get; set; } = false;

}
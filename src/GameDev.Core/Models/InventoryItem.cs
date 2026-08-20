// =============================================================================
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Models;

/// <summary>
/// Represents an inventory item for game assets (collectibles, keys, achievements).
/// </summary>
public class InventoryItem
{
    /// <summary>
    /// Unique identifier for the inventory item.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// ID of the project this inventory item belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// Name of the inventory item.
    /// </summary>
    [MaxLength(128), Required]
    public string ItemName { get; set; } = "";
    
    /// <summary>
    /// Type: 0=Collectible, 1=Key, 2=Achievement, 3=Currency.
    /// </summary>
    public int ItemType { get; set; }  // Enum: Collectible, Key, Achievement, Currency
    
    /// <summary>
    /// Current value/quantity.
    /// </summary>
    public int CurrentValue { get; set; }
    
    /// <summary>
    /// Indicates if the inventory item is published.
    /// </summary>
    public bool Published { get; set; } = false;
}
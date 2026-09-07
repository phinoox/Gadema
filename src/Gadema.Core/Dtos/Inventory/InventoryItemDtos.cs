// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Inventory;

/// <summary>
/// DTO for creating an inventory item.
/// </summary>
public class InventoryItemCreateDto
{
    /// <summary>
    /// ID of the content item this inventory item belongs to (optional).
    /// </summary>
    public Guid? MetaInfoId { get; set; }

    /// <summary>
    /// ID of the project this inventory item belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Name of the inventory item.
    /// </summary>
    [Required, MaxLength(128)]
    public string ItemName { get; set; } = "";

    /// <summary>
    /// Type (Collectible, Key, Achievement, Currency).
    /// </summary>
    public int ItemType { get; set; }

    /// <summary>
    /// Current value/quantity.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Indicates if the inventory item is published.
    /// </summary>
    public bool Published { get; set; } = false;
}

/// <summary>
/// Response DTO for an inventory item.
/// </summary>
public class InventoryItemResponseDto
{
    public Guid Id { get; set; }
    public Guid? MetaInfoId { get; set; }
    public Guid ProjectId { get; set; }
    public string ItemName { get; set; } = "";
    public int ItemType { get; set; }
    public int Quantity { get; set; }
    public bool Published { get; set; }
}

/// <summary>
/// List response for inventory items.
/// </summary>
public class InventoryItemListResponseDto
{
    public IEnumerable<InventoryItemResponseDto> Items { get; set; } = Enumerable.Empty<InventoryItemResponseDto>();
    public int TotalCount { get; set; }
}

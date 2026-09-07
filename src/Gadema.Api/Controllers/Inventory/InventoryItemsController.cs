// =============================================================================
using Gadema.Api.Services;
using Gadema.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Inventory;

/// <summary>
/// Controller for inventory item management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/inventory-items")]
public class InventoryItemsController : ControllerBase
{
    private readonly IInventoryItemService _inventoryItemService;
    private readonly ILogger<InventoryItemsController> _logger;

    public InventoryItemsController(IInventoryItemService inventoryItemService, ILogger<InventoryItemsController> logger)
    {
        _inventoryItemService = inventoryItemService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetInventoryItemsAsync(Guid projectId)
    {
        return Ok(await _inventoryItemService.GetInventoryItemsAsync(projectId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetInventoryItemAsync(Guid id)
    {
        return Ok(await _inventoryItemService.GetInventoryItemAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateInventoryItemAsync(Guid projectId, [FromBody] InventoryItemCreateDto createDto)
    {
        return Ok(await _inventoryItemService.CreateInventoryItemAsync(projectId, createDto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateInventoryItemAsync(Guid id, [FromBody] InventoryItemUpdateDto updateDto)
    {
        return Ok(await _inventoryItemService.UpdateInventoryItemAsync(id, updateDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInventoryItemAsync(Guid id)
    {
        return Ok(await _inventoryItemService.DeleteInventoryItemAsync(id));
    }
}

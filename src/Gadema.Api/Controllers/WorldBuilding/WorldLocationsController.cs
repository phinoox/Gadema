// =============================================================================
using Gadema.Api.Services.WorldBuilding;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.WorldBuilding;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.WorldBuilding;

/// <summary>
/// Controller for world location management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId:guid}/world-locations")]
public class WorldLocationsController : ControllerBase
{
    private readonly WorldLocationService _worldLocationService;
    private readonly ILogger<WorldLocationsController> _logger;

    public WorldLocationsController(WorldLocationService worldLocationService, ILogger<WorldLocationsController> logger)
    {
        _worldLocationService = worldLocationService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetWorldLocationsAsync(Guid projectId)
    {
        return Ok(await _worldLocationService.GetWorldLocationsAsync(projectId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetWorldLocationAsync(Guid id)
    {
        return Ok(await _worldLocationService.GetWorldLocationAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateWorldLocationAsync(Guid projectId, [FromBody] WorldLocationCreateDto createDto)
    {
        return Ok(await _worldLocationService.CreateWorldLocationAsync(projectId, createDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateWorldLocationAsync(Guid id, [FromBody] WorldLocationUpdateDto updateDto)
    {
        return Ok(await _worldLocationService.UpdateWorldLocationAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteWorldLocationAsync(Guid id)
    {
        return Ok(await _worldLocationService.DeleteWorldLocationAsync(id));
    }
}
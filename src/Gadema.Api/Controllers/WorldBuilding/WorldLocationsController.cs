using Gadema.Api.Services.WorldBuilding;
using Gadema.Core.Dtos.MetaInfos;
using Gadema.Core.Dtos.WorldBuilding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gadema.Api.Controllers.WorldBuilding;

[ApiController]
[Route("api/v1/projects/{projectId:guid}/locations")]
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
    public async Task<IActionResult> GetLocationsAsync(
        Guid projectId,
        [FromQuery] int? locationType = null,
        [FromQuery] Guid? parentId = null)
    {
        return Ok(await _worldLocationService.GetLocationsAsync(projectId, locationType, parentId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetLocationAsync(Guid id)
    {
        return Ok(await _worldLocationService.GetLocationAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateLocationAsync(
        Guid projectId,
        [FromBody] WorldLocationCreateDto createDto)
    {
        return Ok(await _worldLocationService.CreateLocationAsync(projectId, createDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateLocationAsync(Guid id, [FromBody] WorldLocationUpdateDto updateDto)
    {
        return Ok(await _worldLocationService.UpdateLocationAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteLocationAsync(Guid id)
    {
        return Ok(await _worldLocationService.DeleteLocationAsync(id));
    }
}

public record WorldLocationCreateDto(CreateMetaInfoDto CreateData, int LocationType, Guid? ParentId = null, string? Description = null);
public record WorldLocationUpdateDto(UpdateMetaInfoDto MetaInfo, int? LocationType = null, Guid? ParentId = null, string? Description = null);
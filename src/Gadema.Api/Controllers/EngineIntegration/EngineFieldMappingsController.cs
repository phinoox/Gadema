// =============================================================================
using Gadema.Api.Services;
using Gadema.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.EngineIntegration;

/// <summary>
/// Controller for engine field mapping management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/field-mappings")]
public class EngineFieldMappingsController : ControllerBase
{
    private readonly IEngineFieldMappingService _engineFieldMappingService;
    private readonly ILogger<EngineFieldMappingsController> _logger;

    public EngineFieldMappingsController(IEngineFieldMappingService engineFieldMappingService, ILogger<EngineFieldMappingsController> logger)
    {
        _engineFieldMappingService = engineFieldMappingService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetEngineFieldMappingsAsync(Guid projectId)
    {
        return Ok(await _engineFieldMappingService.GetEngineFieldMappingsAsync(projectId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEngineFieldMappingAsync(Guid id)
    {
        return Ok(await _engineFieldMappingService.GetEngineFieldMappingAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateEngineFieldMappingAsync(Guid projectId, [FromBody] EngineFieldMappingCreateDto createDto)
    {
        return Ok(await _engineFieldMappingService.CreateEngineFieldMappingAsync(projectId, createDto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEngineFieldMappingAsync(Guid id, [FromBody] EngineFieldMappingUpdateDto updateDto)
    {
        return Ok(await _engineFieldMappingService.UpdateEngineFieldMappingAsync(id, updateDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEngineFieldMappingAsync(Guid id)
    {
        return Ok(await _engineFieldMappingService.DeleteEngineFieldMappingAsync(id));
    }
}

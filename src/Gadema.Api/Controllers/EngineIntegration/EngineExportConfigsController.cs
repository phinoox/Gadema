// =============================================================================
using Gadema.Api.Services;
using Gadema.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.EngineIntegration;

/// <summary>
/// Controller for engine export configuration management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/engine-configs")]
public class EngineExportConfigsController : ControllerBase
{
    private readonly IEngineExportConfigService _engineExportConfigService;
    private readonly ILogger<EngineExportConfigsController> _logger;

    public EngineExportConfigsController(IEngineExportConfigService engineExportConfigService, ILogger<EngineExportConfigsController> logger)
    {
        _engineExportConfigService = engineExportConfigService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetEngineExportConfigsAsync(Guid projectId)
    {
        return Ok(await _engineExportConfigService.GetEngineExportConfigsAsync(projectId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEngineExportConfigAsync(Guid id)
    {
        return Ok(await _engineExportConfigService.GetEngineExportConfigAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateEngineExportConfigAsync(Guid projectId, [FromBody] EngineExportConfigCreateDto createDto)
    {
        return Ok(await _engineExportConfigService.CreateEngineExportConfigAsync(projectId, createDto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEngineExportConfigAsync(Guid id, [FromBody] EngineExportConfigUpdateDto updateDto)
    {
        return Ok(await _engineExportConfigService.UpdateEngineExportConfigAsync(id, updateDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEngineExportConfigAsync(Guid id)
    {
        return Ok(await _engineExportConfigService.DeleteEngineExportConfigAsync(id));
    }
}

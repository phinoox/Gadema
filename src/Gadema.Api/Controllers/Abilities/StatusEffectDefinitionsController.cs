// =============================================================================
using Gadema.Api.Services;
using Gadema.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Abilities;

/// <summary>
/// Controller for status effect definition management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/status-effects")]
public class StatusEffectDefinitionsController : ControllerBase
{
    private readonly IStatusEffectDefinitionService _statusEffectDefinitionService;
    private readonly ILogger<StatusEffectDefinitionsController> _logger;

    public StatusEffectDefinitionsController(IStatusEffectDefinitionService statusEffectDefinitionService, ILogger<StatusEffectDefinitionsController> logger)
    {
        _statusEffectDefinitionService = statusEffectDefinitionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetStatusEffectDefinitionsAsync(Guid projectId)
    {
        return Ok(await _statusEffectDefinitionService.GetStatusEffectDefinitionsAsync(projectId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStatusEffectDefinitionAsync(Guid id)
    {
        return Ok(await _statusEffectDefinitionService.GetStatusEffectDefinitionAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateStatusEffectDefinitionAsync(Guid projectId, [FromBody] StatusEffectDefinitionCreateDto createDto)
    {
        return Ok(await _statusEffectDefinitionService.CreateStatusEffectDefinitionAsync(projectId, createDto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateStatusEffectDefinitionAsync(Guid id, [FromBody] StatusEffectDefinitionUpdateDto updateDto)
    {
        return Ok(await _statusEffectDefinitionService.UpdateStatusEffectDefinitionAsync(id, updateDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStatusEffectDefinitionAsync(Guid id)
    {
        return Ok(await _statusEffectDefinitionService.DeleteStatusEffectDefinitionAsync(id));
    }
}

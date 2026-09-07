// =============================================================================
using Gadema.Api.Services;
using Gadema.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Abilities;

/// <summary>
/// Controller for ability definition management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/ability-definitions")]
public class AbilityDefinitionsController : ControllerBase
{
    private readonly IAbilityDefinitionService _abilityDefinitionService;
    private readonly ILogger<AbilityDefinitionsController> _logger;

    public AbilityDefinitionsController(IAbilityDefinitionService abilityDefinitionService, ILogger<AbilityDefinitionsController> logger)
    {
        _abilityDefinitionService = abilityDefinitionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAbilityDefinitionsAsync(Guid projectId)
    {
        return Ok(await _abilityDefinitionService.GetAbilityDefinitionsAsync(projectId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAbilityDefinitionAsync(Guid id)
    {
        return Ok(await _abilityDefinitionService.GetAbilityDefinitionAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAbilityDefinitionAsync(Guid projectId, [FromBody] AbilityDefinitionCreateDto createDto)
    {
        return Ok(await _abilityDefinitionService.CreateAbilityDefinitionAsync(projectId, createDto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAbilityDefinitionAsync(Guid id, [FromBody] AbilityDefinitionUpdateDto updateDto)
    {
        return Ok(await _abilityDefinitionService.UpdateAbilityDefinitionAsync(id, updateDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAbilityDefinitionAsync(Guid id)
    {
        return Ok(await _abilityDefinitionService.DeleteAbilityDefinitionAsync(id));
    }
}

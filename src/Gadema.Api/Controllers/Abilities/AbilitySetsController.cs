// =============================================================================
using Gadema.Api.Services;
using Gadema.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Abilities;

/// <summary>
/// Controller for ability set management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/ability-sets")]
public class AbilitySetsController : ControllerBase
{
    private readonly IAbilitySetService _abilitySetService;
    private readonly ILogger<AbilitySetsController> _logger;

    public AbilitySetsController(IAbilitySetService abilitySetService, ILogger<AbilitySetsController> logger)
    {
        _abilitySetService = abilitySetService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAbilitySetsAsync(Guid projectId)
    {
        return Ok(await _abilitySetService.GetAbilitySetsAsync(projectId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAbilitySetAsync(Guid id)
    {
        return Ok(await _abilitySetService.GetAbilitySetAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAbilitySetAsync(Guid projectId, [FromBody] AbilitySetCreateDto createDto)
    {
        return Ok(await _abilitySetService.CreateAbilitySetAsync(projectId, createDto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAbilitySetAsync(Guid id, [FromBody] AbilitySetUpdateDto updateDto)
    {
        return Ok(await _abilitySetService.UpdateAbilitySetAsync(id, updateDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAbilitySetAsync(Guid id)
    {
        return Ok(await _abilitySetService.DeleteAbilitySetAsync(id));
    }
}

// =============================================================================
using Gadema.Api.Services;
using Gadema.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Characters;

/// <summary>
/// Controller for character background management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/character-backgrounds")]
public class CharacterBackgroundsController : ControllerBase
{
    private readonly ICharacterBackgroundService _characterBackgroundService;
    private readonly ILogger<CharacterBackgroundsController> _logger;

    public CharacterBackgroundsController(ICharacterBackgroundService characterBackgroundService, ILogger<CharacterBackgroundsController> logger)
    {
        _characterBackgroundService = characterBackgroundService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetCharacterBackgroundsAsync(Guid projectId)
    {
        return Ok(await _characterBackgroundService.GetCharacterBackgroundsAsync(projectId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCharacterBackgroundAsync(Guid id)
    {
        return Ok(await _characterBackgroundService.GetCharacterBackgroundAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCharacterBackgroundAsync(Guid projectId, [FromBody] CharacterBackgroundCreateDto createDto)
    {
        return Ok(await _characterBackgroundService.CreateCharacterBackgroundAsync(projectId, createDto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCharacterBackgroundAsync(Guid id, [FromBody] CharacterBackgroundUpdateDto updateDto)
    {
        return Ok(await _characterBackgroundService.UpdateCharacterBackgroundAsync(id, updateDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCharacterBackgroundAsync(Guid id)
    {
        return Ok(await _characterBackgroundService.DeleteCharacterBackgroundAsync(id));
    }
}

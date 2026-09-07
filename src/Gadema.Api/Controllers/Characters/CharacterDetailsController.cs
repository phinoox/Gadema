// =============================================================================
using Gadema.Api.Services;
using Gadema.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Characters;

/// <summary>
/// Controller for character details management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/characters")]
public class CharacterDetailsController : ControllerBase
{
    private readonly ICharacterDetailsService _characterDetailsService;
    private readonly ILogger<CharacterDetailsController> _logger;

    public CharacterDetailsController(ICharacterDetailsService characterDetailsService, ILogger<CharacterDetailsController> logger)
    {
        _characterDetailsService = characterDetailsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetCharacterDetailsAsync(Guid projectId)
    {
        return Ok(await _characterDetailsService.GetCharacterDetailsAsync(projectId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCharacterDetailAsync(Guid id)
    {
        return Ok(await _characterDetailsService.GetCharacterDetailAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCharacterDetailAsync(Guid projectId, [FromBody] CharacterDetailsDto createDto)
    {
        return Ok(await _characterDetailsService.CreateCharacterDetailAsync(projectId, createDto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCharacterDetailAsync(Guid id, [FromBody] CharacterDetailsUpdateDto updateDto)
    {
        return Ok(await _characterDetailsService.UpdateCharacterDetailAsync(id, updateDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCharacterDetailAsync(Guid id)
    {
        return Ok(await _characterDetailsService.DeleteCharacterDetailAsync(id));
    }
}

// =============================================================================
using Gadema.Api.Services;
using Gadema.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Characters;

/// <summary>
/// Controller for character relation management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/character-relations")]
public class CharacterRelationsController : ControllerBase
{
    private readonly ICharacterRelationService _characterRelationService;
    private readonly ILogger<CharacterRelationsController> _logger;

    public CharacterRelationsController(ICharacterRelationService characterRelationService, ILogger<CharacterRelationsController> logger)
    {
        _characterRelationService = characterRelationService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetCharacterRelationsAsync(Guid projectId)
    {
        return Ok(await _characterRelationService.GetCharacterRelationsAsync(projectId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCharacterRelationAsync(Guid id)
    {
        return Ok(await _characterRelationService.GetCharacterRelationAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCharacterRelationAsync(Guid projectId, [FromBody] CharacterRelationCreateDto createDto)
    {
        return Ok(await _characterRelationService.CreateCharacterRelationAsync(projectId, createDto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCharacterRelationAsync(Guid id, [FromBody] CharacterRelationUpdateDto updateDto)
    {
        return Ok(await _characterRelationService.UpdateCharacterRelationAsync(id, updateDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCharacterRelationAsync(Guid id)
    {
        return Ok(await _characterRelationService.DeleteCharacterRelationAsync(id));
    }
}

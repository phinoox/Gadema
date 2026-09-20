// =============================================================================
using Gadema.Api.Services.Writing.Characters;
using Gadema.Core.Dtos.Writing.Characters;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Writing.Characters;

/// <summary>
/// Controller for character relation management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/character-relations")]
public class CharacterRelationsController : ControllerBase
{
    private readonly CharacterRelationService _characterRelationService;
    private readonly ILogger<CharacterRelationsController> _logger;

    public CharacterRelationsController(CharacterRelationService characterRelationService, ILogger<CharacterRelationsController> logger)
    {
        _characterRelationService = characterRelationService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetCharacterRelationsAsync(Guid projectId)
    {
        return Ok(await _characterRelationService.GetRelationsAsync(projectId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCharacterRelationAsync(Guid id)
    {
        return Ok(await _characterRelationService.GetRelationByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCharacterRelationAsync(Guid projectId, [FromBody] CharacterRelationCreateDto createDto)
    {
        return Ok(await _characterRelationService.CreateCharacterRelationAsync(projectId, createDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCharacterRelationAsync(Guid id, [FromBody] CharacterRelationUpdateDto updateDto)
    {
        return Ok(await _characterRelationService.UpdateCharacterRelationAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCharacterRelationAsync(Guid id)
    {
        return Ok(await _characterRelationService.DeleteCharacterRelationAsync(id));
    }
}

// src/Gadema.Api/Controllers/Characters/CharacterStatesController.cs
using Gadema.Api.Services.Characters;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Characters;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Characters;

[ApiController]
[Route("api/v1/projects/{projectId:guid}/character-states")]
public class CharacterStatesController : ControllerBase
{
    private readonly CharacterStateService _characterStateService;
    private readonly ILogger<CharacterStatesController> _logger;

    public CharacterStatesController(CharacterStateService characterStateService, ILogger<CharacterStatesController> logger)
    {
        _characterStateService = characterStateService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetCharacterStatesAsync(Guid projectId)
    {
        return Ok(await _characterStateService.GetCharacterStatesAsync(projectId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCharacterStateAsync(Guid id)
    {
        return Ok(await _characterStateService.GetCharacterStateAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCharacterStateAsync(Guid projectId, [FromBody] CharacterStateCreateDto createDto)
    {
        return Ok(await _characterStateService.CreateCharacterStateAsync(projectId, createDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCharacterStateAsync(Guid id, [FromBody] CharacterStateUpdateDto updateDto)
    {
        return Ok(await _characterStateService.UpdateCharacterStateAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCharacterStateAsync(Guid id)
    {
        return Ok(await _characterStateService.DeleteCharacterStateAsync(id));
    }
}
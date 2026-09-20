// src/Gadema.Api/Controllers/Characters/CharacterStatesController.cs
using Gadema.Api.Services.Writing.Characters;
using Gadema.Core.Dtos.Writing.Characters;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Writing.Characters;

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
        return Ok(await _characterStateService.GetStatesAsync(projectId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCharacterStateAsync(Guid id)
    {
        return Ok(await _characterStateService.GetStateAsync(id));
    }

    [HttpPost("{id:guid}")]
    public async Task<IActionResult> CreateCharacterStateAsync(Guid projectId,Guid id, [FromBody] CharacterStateCreateDto createDto)
    {
        return Ok(await _characterStateService.CreateCharacterStateAsync(projectId,id, createDto));
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
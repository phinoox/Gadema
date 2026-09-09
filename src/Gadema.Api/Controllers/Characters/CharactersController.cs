// src/Gadema.Api/Controllers/Characters/CharactersController.cs
using Gadema.Api.Services.Characters;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Characters;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Characters;

[ApiController]
[Route("api/v1/projects/{projectId:guid}/characters")]
public class CharactersController : ControllerBase
{
    private readonly CharacterService _characterService;
    private readonly ILogger<CharactersController> _logger;

    public CharactersController(CharacterService characterService, ILogger<CharactersController> logger)
    {
        _characterService = characterService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetCharactersAsync(Guid projectId)
    {
        return Ok(await _characterService.GetCharactersAsync(projectId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCharacterAsync(Guid id)
    {
        return Ok(await _characterService.GetCharacterAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCharacterAsync(Guid projectId, [FromBody] CharacterCreateDto createDto)
    {
        return Ok(await _characterService.CreateCharacterAsync(projectId, createDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCharacterAsync(Guid id, [FromBody] CharacterUpdateDto updateDto)
    {
        return Ok(await _characterService.UpdateCharacterAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCharacterAsync(Guid id)
    {
        return Ok(await _characterService.DeleteCharacterAsync(id));
    }
}

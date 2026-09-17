// src/Gadema.Api/Controllers/Characters/CharacterStoryProfilesController.cs
using Gadema.Api.Services.Characters;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Characters;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Writing.Characters;

[ApiController]
[Route("api/v1/projects/{projectId:guid}/characters/{characterId:guid}/story-profile")]
public class CharacterStoryProfilesController : ControllerBase
{
    private readonly CharacterStoryProfileService _storyProfileService;
    private readonly ILogger<CharacterStoryProfilesController> _logger;

    public CharacterStoryProfilesController(CharacterStoryProfileService storyProfileService, ILogger<CharacterStoryProfilesController> logger)
    {
        _storyProfileService = storyProfileService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetStoryProfileAsync(Guid characterId)
    {
        return Ok(await _storyProfileService.GetProfileAsync(characterId));
    }

    [HttpPost]
    public async Task<IActionResult> CreateStoryProfileAsync(Guid projectId, Guid characterId, [FromBody] CharacterStoryProfileCreateDto createDto)
    {
        return Ok(await _storyProfileService.CreateProfileAsync(projectId, characterId, createDto));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateStoryProfileAsync(Guid characterId, [FromBody] CharacterStoryProfileUpdateDto updateDto)
    {
        return Ok(await _storyProfileService.UpdateProfileAsync(characterId, updateDto));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteStoryProfileAsync(Guid characterId)
    {
        return Ok(await _storyProfileService.DeleteProfileAsync(characterId));
    }
}

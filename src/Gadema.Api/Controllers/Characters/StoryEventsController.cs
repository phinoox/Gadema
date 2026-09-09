// src/Gadema.Api/Controllers/Characters/StoryEventsController.cs
using Gadema.Api.Services.Characters;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Characters;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Characters;

[ApiController]
[Route("api/v1/projects/{projectId:guid}/scenes/{sceneId:guid}/story-events")]
public class StoryEventsController : ControllerBase
{
    private readonly StoryEventService _storyEventService;
    private readonly ILogger<StoryEventsController> _logger;

    public StoryEventsController(StoryEventService storyEventService, ILogger<StoryEventsController> logger)
    {
        _storyEventService = storyEventService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetStoryEventsAsync(Guid sceneId)
    {
        return Ok(await _storyEventService.GetStoryEventsAsync(sceneId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetStoryEventAsync(Guid id)
    {
        return Ok(await _storyEventService.GetStoryEventAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateStoryEventAsync(Guid projectId, Guid sceneId, [FromBody] StoryEventCreateDto createDto)
    {
        return Ok(await _storyEventService.CreateStoryEventAsync(projectId, sceneId, createDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateStoryEventAsync(Guid id, [FromBody] StoryEventUpdateDto updateDto)
    {
        return Ok(await _storyEventService.UpdateStoryEventAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteStoryEventAsync(Guid id)
    {
        return Ok(await _storyEventService.DeleteStoryEventAsync(id));
    }
}

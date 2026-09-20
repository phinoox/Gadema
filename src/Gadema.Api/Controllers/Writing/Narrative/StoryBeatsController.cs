// =============================================================================
using Gadema.Api.Services.Writing.Narrative;
using Gadema.Core.Dtos.Writing.Narrative;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Writing.Narrative;

/// <summary>
/// Controller for story beat management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId:guid}/story-beats")]
public class StoryBeatsController : ControllerBase
{
    private readonly StoryBeatService _storyBeatService;
    private readonly ILogger<StoryBeatsController> _logger;

    public StoryBeatsController(StoryBeatService storyBeatService, ILogger<StoryBeatsController> logger)
    {
        _storyBeatService = storyBeatService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetStoryBeatsAsync(Guid projectId)
    {
        return Ok(await _storyBeatService.GetStoryBeatsAsync(projectId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetStoryBeatAsync(Guid id)
    {
        return Ok(await _storyBeatService.GetStoryBeatAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateStoryBeatAsync(Guid projectId, [FromBody] StoryBeatCreateDto createDto)
    {
        return Ok(await _storyBeatService.CreateStoryBeatAsync(projectId, createDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateStoryBeatAsync(Guid id, [FromBody] StoryBeatUpdateDto updateDto)
    {
        return Ok(await _storyBeatService.UpdateStoryBeatAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteStoryBeatAsync(Guid id)
    {
        return Ok(await _storyBeatService.DeleteStoryBeatAsync(id));
    }
}
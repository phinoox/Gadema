// =============================================================================
using Gadema.Api.Services.Narrative;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Narrative;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Narrative;

/// <summary>
/// Controller for story management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId:guid}/stories")]
public class StoryController : ControllerBase
{
    private readonly StoryService _storyService;
    private readonly ILogger<StoryController> _logger;

    public StoryController(StoryService storyService, ILogger<StoryController> logger)
    {
        _storyService = storyService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetStoriesAsync(Guid projectId)
    {
        return Ok(await _storyService.GetStoriesAsync(projectId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetStoryAsync(Guid id)
    {
        return Ok(await _storyService.GetStoryAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateStoryAsync(Guid projectId, [FromBody] StoryCreateDto createDto)
    {
        return Ok(await _storyService.CreateStoryAsync(projectId, createDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateStoryAsync(Guid id, [FromBody] StoryUpdateDto updateDto)
    {
        return Ok(await _storyService.UpdateStoryAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteStoryAsync(Guid id)
    {
        return Ok(await _storyService.DeleteStoryAsync(id));
    }
}

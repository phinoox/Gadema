// =============================================================================
using Gadema.Api.Services.Writing.Narrative;
using Gadema.Core.Dtos.Writing.Narrative;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Writing.Narrative;

/// <summary>
/// Controller for story chapter management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId:guid}/story-chapters")]
public class StoryChapterController : ControllerBase
{
    private readonly StoryChapterService _storyChapterService;
    private readonly ILogger<StoryChapterController> _logger;

    public StoryChapterController(StoryChapterService storyChapterService, ILogger<StoryChapterController> logger)
    {
        _storyChapterService = storyChapterService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetStoryChaptersAsync(Guid projectId)
    {
        return Ok(await _storyChapterService.GetStoryChaptersAsync(projectId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetStoryChapterAsync(Guid id)
    {
        return Ok(await _storyChapterService.GetStoryChapterAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateStoryChapterAsync(Guid projectId, [FromBody] StoryChapterCreateDto createDto)
    {
        return Ok(await _storyChapterService.CreateStoryChapterAsync(projectId, createDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateStoryChapterAsync(Guid id, [FromBody] StoryChapterUpdateDto updateDto)
    {
        return Ok(await _storyChapterService.UpdateStoryChapterAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteStoryChapterAsync(Guid id)
    {
        return Ok(await _storyChapterService.DeleteStoryChapterAsync(id));
    }
}

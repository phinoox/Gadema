// =============================================================================
using Gadema.Api.Services;
using Gadema.Api.Services.Narrative;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Narrative;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Writing.Narrative;

/// <summary>
/// Controller for story outline management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId:guid}/story-outlines")]
public class StoryOutlinesController : ControllerBase
{
    private readonly StoryOutlineService _storyOutlineService;
    private readonly ILogger<StoryOutlinesController> _logger;

    public StoryOutlinesController(StoryOutlineService storyOutlineService, ILogger<StoryOutlinesController> logger)
    {
        _storyOutlineService = storyOutlineService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetStoryOutlinesAsync(Guid projectId)
    {
        return Ok(await _storyOutlineService.GetStoryOutlinesAsync(projectId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetStoryOutlineAsync(Guid id)
    {
        return Ok(await _storyOutlineService.GetStoryOutlineAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateStoryOutlineAsync(Guid projectId, [FromBody] StoryOutlineCreateDto createDto)
    {
        return Ok(await _storyOutlineService.CreateStoryOutlineAsync(projectId, createDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateStoryOutlineAsync(Guid id, [FromBody] StoryOutlineUpdateDto updateDto)
    {
        return Ok(await _storyOutlineService.UpdateStoryOutlineAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteStoryOutlineAsync(Guid id)
    {
        return Ok(await _storyOutlineService.DeleteStoryOutlineAsync(id));
    }
}

// =============================================================================
using Gadema.Api.Services;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.DialogueTrees;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.SceneSegments;

/// <summary>
/// Controller for content segment management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/content-segments")]
public class SceneSegmentsController : ControllerBase
{
    private readonly ISceneSegmentService _SceneSegmentService;
    private readonly ILogger<SceneSegmentsController> _logger;

    public SceneSegmentsController(ISceneSegmentService SceneSegmentService, ILogger<SceneSegmentsController> logger)
    {
        _SceneSegmentService = SceneSegmentService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetSceneSegmentsAsync(Guid projectId)
    {
        return Ok(await _SceneSegmentService.GetSceneSegmentsAsync(projectId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSceneSegmentAsync(Guid id)
    {
        return Ok(await _SceneSegmentService.GetSceneSegmentAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateSceneSegmentAsync(Guid projectId, [FromBody] SceneSegmentCreateDto createDto)
    {
        return Ok(await _SceneSegmentService.CreateSceneSegmentAsync(projectId, createDto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSceneSegmentAsync(Guid id, [FromBody] SceneSegmentUpdateDto updateDto)
    {
        return Ok(await _SceneSegmentService.UpdateSceneSegmentAsync(id, updateDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSceneSegmentAsync(Guid id)
    {
        return Ok(await _SceneSegmentService.DeleteSceneSegmentAsync(id));
    }
}

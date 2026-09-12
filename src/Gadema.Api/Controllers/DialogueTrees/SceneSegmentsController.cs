// =============================================================================
using Gadema.Api.Services.DialogueTrees;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.DialogueTrees;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.DialogueTrees;

/// <summary>
/// Controller for scene segment management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId:guid}/scene-segments")]
public class SceneSegmentsController : ControllerBase
{
    private readonly SceneSegmentService _sceneSegmentService;
    private readonly ILogger<SceneSegmentsController> _logger;

    public SceneSegmentsController(SceneSegmentService sceneSegmentService, ILogger<SceneSegmentsController> logger)
    {
        _sceneSegmentService = sceneSegmentService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetSceneSegmentsAsync(Guid projectId)
    {
        return Ok(await _sceneSegmentService.GetSceneSegmentsAsync(projectId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSceneSegmentAsync(Guid id)
    {
        return Ok(await _sceneSegmentService.GetSceneSegmentAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateSceneSegmentAsync(Guid projectId, [FromBody] SceneSegmentCreateDto createDto)
    {
        return Ok(await _sceneSegmentService.CreateSceneSegmentAsync(projectId, createDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateSceneSegmentAsync(Guid id, [FromBody] SceneSegmentUpdateDto updateDto)
    {
        return Ok(await _sceneSegmentService.UpdateSceneSegmentAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteSceneSegmentAsync(Guid id)
    {
        return Ok(await _sceneSegmentService.DeleteSceneSegmentAsync(id));
    }
}

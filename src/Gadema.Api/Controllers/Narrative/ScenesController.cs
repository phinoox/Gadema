// =============================================================================
using Gadema.Api.Services;
using Gadema.Api.Services.Narrative;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.MetaInfos;
using Gadema.Core.Dtos.Narrative;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Narrative;

/// <summary>
/// Controller for scene management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId:guid}/scenes")]
public class ScenesController : ControllerBase
{
    private readonly SceneService _sceneService;
    private readonly ILogger<ScenesController> _logger;

    public ScenesController(SceneService sceneService, ILogger<ScenesController> logger)
    {
        _sceneService = sceneService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetScenesAsync(Guid projectId)
    {
        return Ok(await _sceneService.GetScenesAsync(projectId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSceneAsync(Guid id)
    {
        return Ok(await _sceneService.GetSceneAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateSceneAsync(Guid projectId, [FromBody] SceneCreateDto createDto)
    {
        return Ok(await _sceneService.CreateSceneAsync(projectId, createDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateSceneAsync(Guid id, [FromBody] SceneUpdateDto updateDto)
    {
        return Ok(await _sceneService.UpdateSceneAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteSceneAsync(Guid id)
    {
        return Ok(await _sceneService.DeleteSceneAsync(id));
    }
}

// =============================================================================
using Microsoft.AspNetCore.Http;
// GameDev.Api - ASP.NET Core Web API Controllers
// =============================================================================

using System;
using System.Threading.Tasks;
using GameDev.Api.Services;
using GameDev.Core.Dtos.StoryOutlining;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameDev.Api.Controllers;

/// <summary>
/// Controller for story outlining endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{id}/sequences")]
public class StorySequencesController : ControllerBase
{
    private readonly IStoryOutlineService _outlineService;
    private readonly ILogger<StorySequencesController> _logger;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public StorySequencesController(IStoryOutlineService outlineService, ILogger<StorySequencesController> logger)
    {
        _outlineService = outlineService;
        _logger = logger;
    }

    /// <summary>
    /// List story sequences for project.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetSequencesAsync(Guid id)
    {
        return Ok(await _outlineService.GetSequencesAsync(id));
    }

    /// <summary>
    /// Create new sequence (chapter).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateSequenceAsync(Guid id, [FromBody] CreateSequenceDto createDto)
    {
        return Ok(await _outlineService.CreateSequenceAsync(id, createDto));
    }
}
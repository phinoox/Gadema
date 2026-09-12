// =============================================================================
using Gadema.Api.Services;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Projects;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Projects;

/// <summary>
/// Controller for project series management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/series")]
public class ProjectSeriesController : ControllerBase
{
    private readonly IProjectSeriesService _projectSeriesService;
    private readonly ILogger<ProjectSeriesController> _logger;

    public ProjectSeriesController(IProjectSeriesService projectSeriesService, ILogger<ProjectSeriesController> logger)
    {
        _projectSeriesService = projectSeriesService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjectSeriesAsync()
    {
        return Ok(await _projectSeriesService.GetProjectSeriesAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectSeriesAsync(Guid id)
    {
        return Ok(await _projectSeriesService.GetProjectSeriesAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateProjectSeriesAsync([FromBody] ProjectSeriesCreateDto createDto)
    {
        return Ok(await _projectSeriesService.CreateProjectSeriesAsync(createDto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProjectSeriesAsync(Guid id, [FromBody] ProjectSeriesUpdateDto updateDto)
    {
        return Ok(await _projectSeriesService.UpdateProjectSeriesAsync(id, updateDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProjectSeriesAsync(Guid id)
    {
        return Ok(await _projectSeriesService.DeleteProjectSeriesAsync(id));
    }
}

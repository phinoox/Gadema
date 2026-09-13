using Gadema.Api.Services.Projects;
using Gadema.Core.Dtos.Base.Projects;
using Gadema.Core.Dtos.Projects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gadema.Api.Controllers.Projects;

[ApiController]
[Route("api/v1/projects/{projectId:guid}/series")]
public class ProjectSeriesController : ControllerBase
{
    private readonly ProjectSeriesService _projectSeriesService;
    private readonly ILogger<ProjectSeriesController> _logger;

    public ProjectSeriesController(ProjectSeriesService projectSeriesService, ILogger<ProjectSeriesController> logger)
    {
        _projectSeriesService = projectSeriesService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjectSeriesAsync(Guid projectId)
    {
        return Ok(await _projectSeriesService.GetSeriesAsync(projectId));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProjectSeriesByIdAsync(Guid id, Guid projectId)
    {
        return Ok(await _projectSeriesService.GetSeriesAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateProjectSeriesAsync(Guid projectId, [FromBody] ProjectSeriesCreateDto createDto)
    {
        return Ok(await _projectSeriesService.CreateSeriesAsync(projectId, createDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProjectSeriesAsync(Guid id, Guid projectId, [FromBody] ProjectSeriesUpdateDto updateDto)
    {
        return Ok(await _projectSeriesService.UpdateSeriesAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProjectSeriesAsync(Guid id, Guid projectId)
    {
        return Ok(await _projectSeriesService.DeleteSeriesAsync(id));
    }
}
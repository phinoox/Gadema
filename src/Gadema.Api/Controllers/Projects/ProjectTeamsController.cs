// =============================================================================
using Gadema.Api.Services;
using Gadema.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Projects;

/// <summary>
/// Controller for project team management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/teams")]
public class ProjectTeamsController : ControllerBase
{
    private readonly IProjectTeamService _projectTeamService;
    private readonly ILogger<ProjectTeamsController> _logger;

    public ProjectTeamsController(IProjectTeamService projectTeamService, ILogger<ProjectTeamsController> logger)
    {
        _projectTeamService = projectTeamService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjectTeamsAsync(Guid projectId)
    {
        return Ok(await _projectTeamService.GetProjectTeamsAsync(projectId));
    }

    [HttpPost]
    public async Task<IActionResult> CreateProjectTeamAsync(Guid projectId, [FromBody] ProjectTeamCreateDto createDto)
    {
        return Ok(await _projectTeamService.CreateProjectTeamAsync(projectId, createDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProjectTeamAsync(Guid id)
    {
        return Ok(await _projectTeamService.DeleteProjectTeamAsync(id));
    }
}

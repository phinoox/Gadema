// =============================================================================
using Microsoft.AspNetCore.Http;
// Gadema.Api - ASP.NET Core Web API Controllers
// =============================================================================

using System;
using System.Threading.Tasks;
using Gadema.Api.Services;
using Gadema.Core.Dtos.Projects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gadema.Api.Controllers;

/// <summary>
/// Controller for project management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectsController> _logger;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger)
    {
        _projectService = projectService;
        _logger = logger;
    }

    /// <summary>
    /// List all projects (paginated).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetProjectsAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? status = null,
        [FromQuery] int? visibility = null,
        [FromQuery] string? search = null)
    {
        return Ok(await _projectService.GetProjectsAsync(page, pageSize, status, visibility, search));
    }

    /// <summary>
    /// Create a new project.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateProjectAsync([FromBody] CreateProjectDto createDto)
    {
        return Ok(await _projectService.CreateProjectAsync(createDto));
    }

    /// <summary>
    /// Update a project.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProjectAsync(Guid id, [FromBody] UpdateProjectDto updateDto)
    {
        return Ok(await _projectService.UpdateProjectAsync(id, updateDto));
    }

    /// <summary>
    /// Transfer or delete a project.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> TransferOrDeleteProjectAsync(Guid id)
    {
        return Ok(await _projectService.TransferOrDeleteProjectAsync(id));
    }

    /// <summary>
    /// Create API token for project.
    /// </summary>
    [HttpPost("{id}/tokens")]
    public async Task<IActionResult> CreateApiTokenAsync(Guid id, [FromBody] ProjectTokenDto tokenDto)
    {
        return Ok(await _projectService.CreateApiTokenAsync(id, tokenDto));
    }

    /// <summary>
    /// List API tokens for project.
    /// </summary>
    [HttpGet("{id}/tokens")]
    public async Task<IActionResult> GetProjectTokensAsync(Guid id)
    {
        return Ok(await _projectService.GetProjectTokensAsync(id));
    }

    /// <summary>
    /// Revoke API token for project.
    /// </summary>
    [HttpDelete("{id}/tokens/{tokenId}")]
    public async Task<IActionResult> RevokeApiTokenAsync(Guid id, Guid tokenId)
    {
        return Ok(await _projectService.RevokeApiTokenAsync(id, tokenId));
    }
}
using Gadema.Api.Services.Projects;
using Gadema.Core.Dtos.Base.Projects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gadema.Api.Controllers.Projects;

[ApiController]
[Route("api/v1/projects")]
public class ProjectsController : ControllerBase
{
    private readonly ProjectService _projectService;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(ProjectService projectService, ILogger<ProjectsController> logger)
    {
        _projectService = projectService;
        _logger = logger;
    }

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

    [HttpPost]
    public async Task<IActionResult> CreateProjectAsync([FromBody] ProjectCreateDto createDto)
    {
        return Ok(await _projectService.CreateProjectAsync(createDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateProjectAsync(Guid id, [FromBody] ProjectUpdateDto updateDto)
    {
        return Ok(await _projectService.UpdateProjectAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> TransferOrDeleteProjectAsync(Guid id)
    {
        return Ok(await _projectService.TransferOrDeleteProjectAsync(id));
    }

    [HttpPost("{id:guid}/tokens")]
    public async Task<IActionResult> CreateApiTokenAsync(Guid id, [FromBody] ProjectTokenCreateDto tokenDto)
    {
        return Ok(await _projectService.CreateApiTokenAsync(id, tokenDto));
    }

    [HttpGet("{id:guid}/tokens")]
    public async Task<IActionResult> GetProjectTokensAsync(Guid id)
    {
        return Ok(await _projectService.GetProjectTokensAsync(id));
    }

    [HttpDelete("{id:guid}/tokens/{tokenId:guid}")]
    public async Task<IActionResult> RevokeApiTokenAsync(Guid id, Guid tokenId)
    {
        return Ok(await _projectService.RevokeApiTokenAsync(id, tokenId));
    }
}
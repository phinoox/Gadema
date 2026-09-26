using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gadema.Core.Dtos.Base.Projects;
using Gadema.Core.Dtos.Search;
using Gadema.Api.Services.Base.Projects;
using Gadema.Core.Dtos;

namespace Gadema.Api.Controllers.Projects;

[ApiController]
[Route("api/v1/projects")]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly ProjectService _projectService;

    public ProjectController(ProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] Guid? projectId) 
        => Ok(await _projectService.GetMatchesAsync(query, projectId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProjectCreateDto createDto)
    {
        var result = await _projectService.CreateAsync(createDto);
        return result.Successful ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, [FromQuery] Guid contextProjectId) 
        => await HandleResultAsync(await _projectService.GetAsync(id, contextProjectId));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProjectUpdateDto updateDto, [FromQuery] Guid contextProjectId) 
        => await HandleResultAsync(await _projectService.UpdateAsync(id, contextProjectId, updateDto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] Guid contextProjectId) 
        => await HandleResultAsync(await _projectService.DeleteAsync(id, contextProjectId));

    /// <summary>
    /// Helper to map ApiResponseDto to IActionResult with correct status codes.
    /// </summary>
    private async Task<IActionResult> HandleResultAsync<T>(ApiResponseDto<T> result) where T : class
    {
        return result.Successful ? Ok(result) : StatusCode((int)result.StatusCode, result);
    }
}
using Gadema.Api.Services.Projects;
using Gadema.Core.Dtos.Projects;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Projects;

[ApiController]
[Route("api/v1/projects")]
public class ProjectsController : ControllerBase
{
    private readonly ProjectService _projectService;

    public ProjectsController(ProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpPost("{projectId:guid}")]
    public async Task<IActionResult> Create(Guid projectId, [FromBody] ProjectCreateDto dto) 
        => Ok(await _projectService.CreateAsync(projectId, dto));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, [FromQuery] Guid contextProjectId) 
        => Ok(await _projectService.GetAsync(id, contextProjectId));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromQuery] Guid contextProjectId, [FromBody] ProjectUpdateDto dto) 
        => Ok(await _projectService.UpdateAsync(id, contextProjectId, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] Guid contextProjectId) 
        => Ok(await _projectService.DeleteAsync(id, contextProjectId));
}
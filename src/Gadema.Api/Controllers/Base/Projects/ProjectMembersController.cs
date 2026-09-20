using Gadema.Api.Services.Base.Projects;
using Gadema.Core.Dtos.Base.Projects;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Base.Projects;

[ApiController]
[Route("api/v1/projects/{projectId:guid}/members")]
public class ProjectMembersController : ControllerBase
{
    private readonly ProjectMemberService _service;

    public ProjectMembersController(ProjectMemberService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetMembers(Guid projectId)
    {
        return Ok(await _service.GetMembersAsync(projectId));
    }

    [HttpPost]
    public async Task<IActionResult> AddMember(Guid projectId, [FromBody] AddProjectMemberDto dto)
    {
        return Ok(await _service.AddMemberAsync(projectId, dto));
    }

    [HttpPut("{memberId:guid}/role")]
    public async Task<IActionResult> UpdateRole(Guid projectId, Guid memberId, [FromBody] UpdateProjectMemberRoleDto dto)
    {
        return Ok(await _service.UpdateRoleAsync(projectId, memberId, dto));
    }

    [HttpDelete("{memberId:guid}")]
    public async Task<IActionResult> RemoveMember(Guid projectId, Guid memberId)
    {
        return Ok(await _service.RemoveMemberAsync(projectId, memberId));
    }
}
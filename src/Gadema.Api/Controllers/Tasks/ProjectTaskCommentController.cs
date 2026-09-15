using Microsoft.AspNetCore.Mvc;
using Gadema.Api.Services.Tasks;
using Gadema.Core.Dtos.Tasks;

namespace Gadema.Api.Controllers.Tasks;

[ApiController]
[Route("api/v1/projects/{projectId:guid}/tasks/{taskId:guid}/comments")]
public class ProjectTaskCommentController : ControllerBase
{
    private readonly ProjectTaskCommentService _service;

    public ProjectTaskCommentController(ProjectTaskCommentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] Guid projectId, [FromRoute] Guid taskId) 
        => Ok(await _service.GetCommentsAsync(taskId));

    [HttpPost]
    public async Task<IActionResult> Create([FromRoute] Guid projectId, [FromRoute] Guid taskId, [FromBody] ProjectTaskCommentCreateDto dto)
    {
        var result = await _service.AddCommentAsync(taskId, dto);
        return result.Successful ? CreatedAtAction(nameof(GetById), new { projectId = projectId, taskId = taskId, id = result.Data.EntityId }, result) : BadRequest(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) 
        => Ok(await _service.GetCommentsAsync(id)); // Assuming you add this to service

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid projectId, Guid taskId, [FromBody] ProjectTaskCommentUpdateDto dto) 
        => Ok(await _service.UpdateCommentAsync(taskId, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid projectId, Guid taskId, [FromRoute] Guid id) 
        => Ok(await _service.DeleteCommentAsync(id));
}
using Microsoft.AspNetCore.Mvc;
using Gadema.Api.Services.Tasks;
using Gadema.Core.Dtos.Tasks;

namespace Gadema.Api.Controllers.Tasks;

[ApiController]
[Route("api/v1/projects/{projectId:guid}/tasks")]
public class ProjectTaskController : ControllerBase
{
    private readonly ProjectTaskService _service;

    public ProjectTaskController(ProjectTaskService service)
    {
        _service = service;
    }

    /// <summary>
    /// List all tasks for a specific project.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromRoute] Guid projectId) 
        => Ok(await _service.GetTasksAsync(projectId));

    /// <summary>
    /// Get a single task by its ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) 
        => Ok(await _service.GetTaskAsync(id));

    /// <summary>
    /// Create a new task within the specified project.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromRoute] Guid projectId, [FromBody] ProjectTaskCreateDto dto)
    {
        var result = await _service.CreateTaskAsync(projectId, dto);
        return result.Successful ? CreatedAtAction(nameof(GetById), new { id = result.Data.EntityId }, result) : BadRequest(result);
    }

    /// <summary>
    /// Update an existing task.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProjectTaskUpdateDto dto) 
        => Ok(await _service.UpdateTaskAsync(id, dto));

    /// <summary>
    /// Delete a task.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) 
        => Ok(await _service.DeleteTaskAsync(id));
}
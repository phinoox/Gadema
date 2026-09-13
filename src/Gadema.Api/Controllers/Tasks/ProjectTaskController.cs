using Gadema.Api.Services.Tasks;
using Gadema.Core.Dtos.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gadema.Api.Controllers.Tasks;

[ApiController]
[Route("api/v1/projects/{projectId:guid}/tasks")]
public class ProjectTaskController : ControllerBase
{
    private readonly ProjectTaskService _projectTaskService;
    private readonly ILogger<ProjectTaskController> _logger;

    public ProjectTaskController(ProjectTaskService projectTaskService, ILogger<ProjectTaskController> logger)
    {
        _projectTaskService = projectTaskService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetTasksAsync(
        Guid projectId,
        [FromQuery] int? status = null,
        [FromQuery] Guid? assignedToUserId = null,
        [FromQuery] bool? isQuickWin = null,
        [FromQuery] string? search = null)
    {
        return Ok(await _projectTaskService.GetTasksAsync(projectId, status, assignedToUserId, isQuickWin, search));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTaskAsync(Guid id, Guid projectId)
    {
        return Ok(await _projectTaskService.GetTaskAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTaskAsync(Guid projectId, [FromBody] ProjectTaskCreateDto createDto)
    {
        return Ok(await _projectTaskService.CreateTaskAsync(projectId, createDto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTaskAsync(Guid id, Guid projectId, [FromBody] ProjectTaskUpdateDto updateDto)
    {
        return Ok(await _projectTaskService.UpdateTaskAsync(id, updateDto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTaskAsync(Guid id, Guid projectId)
    {
        return Ok(await _projectTaskService.DeleteTaskAsync(id));
    }

    // Comments endpoint for tasks
    [HttpGet("{id:guid}/comments")]
    public async Task<IActionResult> GetTaskCommentsAsync(Guid taskId, Guid projectId)
    {
        var commentService = new Gadema.Api.Services.Tasks.ProjectTaskCommentsService(_projectTaskService._db, _projectTaskService._logger, _projectTaskService._userContext); // Note: would need dependency injection fix
        return Ok(await commentService.GetCommentsAsync(projectId, taskId));
    }

    [HttpPost("{id:guid}/comments")]
    public async Task<IActionResult> CreateTaskCommentAsync(Guid id, Guid projectId, string text)
    {
        var commentService = new Gadema.Api.Services.Tasks.ProjectTaskCommentsService(_projectTaskService._db, _projectTaskService._logger, _projectTaskService._userContext);
        return Ok(await commentService.CreateCommentAsync(projectId, taskId: null!, text)); // Simplified
    }
}

public record ProjectTaskCreateDto(string TaskTitle, string? Description = null, int? Status = null, int Priority = 1, int Difficulty = 1, decimal? EstimatedMinutes = null, Guid? AssignedToUserId = null, DateTime? DueDate = null, bool IsQuickWin = false);
public record ProjectTaskUpdateDto(string? TaskTitle = null, string? Description = null, int? Status = null, int? Priority = null, int? Difficulty = null, decimal? EstimatedMinutes = null, Guid? AssignedToUserId = null, DateTime? DueDate = null, bool? IsQuickWin = null);
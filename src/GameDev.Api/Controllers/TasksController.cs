// =============================================================================
// GameDev.Api - ASP.NET Core Web API Controllers
// =============================================================================

namespace GameDev.Api.Controllers;

/// <summary>
/// Controller for task management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TasksController> _logger;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public TasksController(ITaskService taskService, ILogger<TasksController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    /// <summary>
    /// List all tasks (paginated).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetTasksAsync(
        [FromQuery] Guid? projectId = null,
        [FromQuery] int? status = null,
        [FromQuery] int? difficulty = null,
        [FromQuery] bool isQuickWin = false)
    {
        return Ok(await _taskService.GetTasksAsync(projectId, status, difficulty, isQuickWin));
    }

    /// <summary>
    /// Create new task.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateTaskAsync([FromBody] CreateTaskDto createDto)
    {
        return Ok(await _taskService.CreateTaskAsync(createDto));
    }

    /// <summary>
    /// Update task.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTaskAsync(Guid id, [FromBody] UpdateTaskDto updateDto)
    {
        return Ok(await _taskService.UpdateTaskAsync(id, updateDto));
    }

    /// <summary>
    /// Delete task (admin only).
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTaskAsync(Guid id)
    {
        return Ok(await _taskService.DeleteTaskAsync(id));
    }
}
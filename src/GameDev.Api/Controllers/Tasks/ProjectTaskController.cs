// =============================================================================
using Microsoft.AspNetCore.Http;
// GameDev.Api - ASP.NET Core Web API Controllers
// =============================================================================

using System;
using System.Threading.Tasks;
using GameDev.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GameDev.Core.Dtos;
using GameDev.Core.Dtos.Tasks;

namespace GameDev.Api.Controllers.Tasks;

/// <summary>
/// Controller for project task management endpoints (renamed to ProjectTaskController).
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/tasks")]
public class ProjectTaskController : ControllerBase
{
    private readonly IProjectTaskService _taskService;
    private readonly ILogger<ProjectTaskController> _logger;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public ProjectTaskController(IProjectTaskService taskService, ILogger<ProjectTaskController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    /// <summary>
    /// List all tasks (paginated).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetTasksAsync(
        Guid projectId,  // Moved from query param to path param for cleaner routing
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
    public async Task<IActionResult> CreateTaskAsync([FromBody] ProjectTaskCreateDto createDto)
    {
        return Ok(await _taskService.CreateTaskAsync(createDto));
    }

    /// <summary>
    /// Update task.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTaskAsync(Guid id, [FromBody] ProjectTaskUpdateDto updateDto)
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
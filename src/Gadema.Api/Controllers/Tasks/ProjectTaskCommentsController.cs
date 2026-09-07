// =============================================================================
using Gadema.Api.Services;
using Gadema.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Tasks;

/// <summary>
/// Controller for project task comments management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/tasks/{taskId}/comments")]
public class ProjectTaskCommentsController : ControllerBase
{
    private readonly IProjectTaskCommentService _taskCommentService;
    private readonly ILogger<ProjectTaskCommentsController> _logger;

    public ProjectTaskCommentsController(IProjectTaskCommentService taskCommentService, ILogger<ProjectTaskCommentsController> logger)
    {
        _taskCommentService = taskCommentService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetTaskCommentsAsync(Guid projectId, Guid taskId)
    {
        return Ok(await _taskCommentService.GetTaskCommentsAsync(taskId));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTaskCommentAsync(Guid projectId, Guid taskId, [FromBody] ProjectTaskCommentCreateDto createDto)
    {
        return Ok(await _taskCommentService.CreateTaskCommentAsync(taskId, createDto));
    }
}

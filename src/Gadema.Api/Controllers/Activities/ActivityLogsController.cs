using Gadema.Api.Services.Content;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Activities;

/// <summary>
/// Controller for viewing project activity logs.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId:guid}/activity-logs")]
public class ActivityLogsController : ControllerBase
{
    private readonly ActivityLogService _activityLogService;
    private readonly ILogger<ActivityLogsController> _logger;

    public ActivityLogsController(ActivityLogService activityLogService, ILogger<ActivityLogsController> logger)
    {
        _activityLogService = activityLogService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a paginated list of activity logs for the specified project.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetActivityLogsAsync(Guid projectId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(await _activityLogService.GetLogsAsync(projectId, page, pageSize));
    }
}
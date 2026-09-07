// =============================================================================
using Gadema.Api.Services;
using Gadema.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Activities;

/// <summary>
/// Controller for activity log management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/activity-logs")]
public class ActivityLogsController : ControllerBase
{
    private readonly IActivityLogService _activityLogService;
    private readonly ILogger<ActivityLogsController> _logger;

    public ActivityLogsController(IActivityLogService activityLogService, ILogger<ActivityLogsController> logger)
    {
        _activityLogService = activityLogService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetActivityLogsAsync(Guid projectId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(await _activityLogService.GetActivityLogsAsync(projectId, page, pageSize));
    }
}

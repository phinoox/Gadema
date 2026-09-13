// =============================================================================
using Gadema.Api.Services.Activities;
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
    private readonly ActivityLogService _activityLogService;
    private readonly ILogger<ActivityLogsController> _logger;

    public ActivityLogsController(ActivityLogService activityLogService, ILogger<ActivityLogsController> logger)
    {
        _activityLogService = activityLogService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetActivityLogsAsync(
        Guid projectId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20,
        [FromQuery] int? actionId = null, 
        [FromQuery] string? entityId = null, 
        [FromQuery] DateTime? startDate = null, 
        [FromQuery] DateTime? endDate = null)
    {
        // Delegate to the existing GetLogsAsync method which has the full functionality
        return Ok(await _activityLogService.GetLogsAsync(
            projectId, 
            actionId, 
            entityId, 
            startDate, 
            endDate, 
            page, 
            pageSize));
    }
}
// =============================================================================
using Gadema.Api.Services;
using Gadema.Api.Services.Activities;
using Gadema.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Activities;

/// <summary>
/// Controller for token usage log management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/token-usage-logs")]
public class TokenUsageLogsController : ControllerBase
{
    private readonly TokenUsageLogService _tokenUsageLogService;
    private readonly ILogger<TokenUsageLogsController> _logger;

    public TokenUsageLogsController(TokenUsageLogService tokenUsageLogService, ILogger<TokenUsageLogsController> logger)
    {
        _tokenUsageLogService = tokenUsageLogService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetTokenUsageLogsAsync(
        Guid projectId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        // Delegate to the existing GetLogsAsync method which has the full functionality
        return Ok(await _tokenUsageLogService.GetLogsAsync(projectId, page, pageSize));
    }
}

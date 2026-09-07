// =============================================================================
using Gadema.Api.Services;
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
    private readonly ITokenUsageLogService _tokenUsageLogService;
    private readonly ILogger<TokenUsageLogsController> _logger;

    public TokenUsageLogsController(ITokenUsageLogService tokenUsageLogService, ILogger<TokenUsageLogsController> logger)
    {
        _tokenUsageLogService = tokenUsageLogService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetTokenUsageLogsAsync(Guid projectId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(await _tokenUsageLogService.GetTokenUsageLogsAsync(projectId, page, pageSize));
    }
}

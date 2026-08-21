// =============================================================================
using Microsoft.AspNetCore.Http;
// GameDev.Api - ASP.NET Core Web API Controllers
// =============================================================================

using System;
using System.Threading.Tasks;
using GameDev.Api.Services;
using GameDev.Core.Dtos.Reviews;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameDev.Api.Controllers;

/// <summary>
/// Controller for review status endpoints.
/// </summary>
[ApiController]
[Route("api/v1/content-items/{id}/review")]
public class ReviewStatusController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly ILogger<ReviewStatusController> _logger;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public ReviewStatusController(IReviewService reviewService, ILogger<ReviewStatusController> logger)
    {
        _reviewService = reviewService;
        _logger = logger;
    }

    /// <summary>
    /// Get review status for content item.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetReviewStatusAsync(Guid id)
    {
        return Ok(await _reviewService.GetReviewStatusAsync(id));
    }

    /// <summary>
    /// Approve/reject content item.
    /// </summary>
    [HttpPut]
    public async Task<IActionResult> ApproveContentAsync(Guid id, [FromBody] ApproveContentDto approveDto)
    {
        return Ok(await _reviewService.ApproveContentAsync(id, approveDto));
    }
}
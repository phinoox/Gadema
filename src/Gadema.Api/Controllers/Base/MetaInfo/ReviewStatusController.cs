using Gadema.Api.Services.Content;
using Gadema.Core.Dtos.Reviews; // Ensure this matches your actual DTO namespace
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gadema.Api.Controllers.Base.MetaInfo;

/// <summary>
/// Controller for review status endpoints.
/// </summary>
[ApiController]
// Updated to follow the project-scoped routing pattern: api/v1/projects/{projectId}/...
[Route("api/v1/projects/{projectId:guid}/review-status")]
public class ReviewStatusController : ControllerBase
{
    private readonly ReviewStatusService _reviewService;
    private readonly ILogger<ReviewStatusController> _logger;

    public ReviewStatusController(ReviewStatusService reviewService, ILogger<ReviewStatusController> logger)
    {
        _reviewService = reviewService;
        _logger = logger;
    }

    /// <summary>
    /// Get review status for a specific content item.
    /// </summary>
    [HttpGet("{id:guid}")] // The id is the target (the component/anchor)
    public async Task<IActionResult> GetReviewStatusAsync(Guid projectId, Guid id)
    {
        // Service now receives both IDs to perform project access validation
        return Ok(await _reviewService.GetReviewStatusAsync(projectId, id));
    }

    /// <summary>
    /// Approve/reject content item.
    /// </summary>
    [HttpPut("{id:guid}")] // The id is the target (the component/anchor)
    public async Task<IActionResult> ApproveContentAsync(Guid projectId, Guid id, [FromBody] ApproveContentDto approveDto)
    {
        // Service now receives both IDs to perform project access validation
        return Ok(await _reviewService.ApproveContentAsync(projectId, id, approveDto));
    }
}
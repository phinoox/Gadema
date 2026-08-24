// =============================================================================
using GameDev.Core.Dtos;
using Microsoft.EntityFrameworkCore;
// GameDev.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using GameDev.Core.Dtos.Reviews;
using GameDev.Data;
using Microsoft.Extensions.Logging;
using GameDev.Core.Services;


namespace GameDev.Api.Services;

/// <summary>
/// Implementation of review status service.
/// </summary>
public class ReviewStatusService : IGademaService,  IReviewStatusService
{
    private readonly GameDbContext _context;
    private readonly ILogger<ReviewStatusService> _logger;

    public ServiceTypeEnum ServiceType => ServiceTypeEnum.ReviewService;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public ReviewStatusService(GameDbContext context, ILogger<ReviewStatusService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get review status for content item.
    /// </summary>
    public async Task<ApiResponseDto<ReviewStatusResponseDto>> GetReviewStatusAsync(Guid contentItemId)
    {
        var reviewStatus = await _context.ReviewStatuses.FindAsync(contentItemId);

        return ApiResponseDto<ReviewStatusResponseDto>.Success(new ReviewStatusResponseDto());
    }

    /// <summary>
    /// Approve/reject content item.
    /// </summary>
    public async Task<ApiResponseDto<ReviewStatusResponseDto>> ApproveContentAsync(Guid contentItemId, ApproveContentDto approveDto)
    {
        var reviewStatus = await _context.ReviewStatuses.FindAsync(contentItemId);

        if (reviewStatus == null)
        {
            return ApiResponseDto<ReviewStatusResponseDto>.NotFound($"Review status for ContentItem {contentItemId} not found");
        }

        reviewStatus.Status = approveDto.Status;
        
        if (!string.IsNullOrWhiteSpace(approveDto.ReviewComments))
        {
            reviewStatus.ReviewComments = approveDto.ReviewComments;
        }

        reviewStatus.ReviewedByUserId = UserHelper.GetUserId();
        reviewStatus.ReviewedAt = DateTime.UtcNow;

        _context.Entry(reviewStatus).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        await _context.SaveChangesAsync();

        return ApiResponseDto<ReviewStatusResponseDto>.Success(new ReviewStatusResponseDto());
    }
}
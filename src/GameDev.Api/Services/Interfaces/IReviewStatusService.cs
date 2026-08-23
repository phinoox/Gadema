// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

using System.Collections.Generic;
using System.Threading.Tasks;
using GameDev.Core.Dtos.Response;
using GameDev.Core.Dtos.Reviews;
using GameDev.Api.Services;
using System;


namespace GameDev.Api.Services;

/// <summary>
/// Interface for review status operations.
/// </summary>
public interface IReviewStatusService
{
    /// <summary>
    /// Get review status for content item.
    /// </summary>
    Task<ApiResponseDto<ReviewStatusResponseDto>> GetReviewStatusAsync(Guid contentItemId);

    /// <summary>
    /// Approve/reject content item.
    /// </summary>
    Task<ApiResponseDto<ReviewStatusResponseDto>> ApproveContentAsync(Guid contentItemId, ApproveContentDto approveDto);
}
// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

using System.Collections.Generic;
using System.Threading.Tasks;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Dtos.Reviews;
using Gadema.Api.Services;
using System;
using Gadema.Core.Dtos;


namespace Gadema.Api.Services;

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
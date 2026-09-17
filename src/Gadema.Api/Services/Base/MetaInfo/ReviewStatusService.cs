using System.Linq;
using Microsoft.EntityFrameworkCore;

using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Reviews;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;

namespace Gadema.Api.Services.Content;

/// <summary>
/// Service for managing content review workflow.
/// </summary>
public class ReviewStatusService : CoreService
{
    public ReviewStatusService(GameDbContext db, ILogger<ReviewStatusService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    private ReviewStatusResponseDto MapToResponseDto(ReviewStatus status)
        => new()
        {
            Id = status.Id,
            TargetId = status.TargetId, // Replaced MetaInfoId
            Status = status.Status,
            ReviewedByUserId = status.ReviewedByUserId,
            ReviewComments = status.ReviewComment,
            ReviewedAt = status.ReviewedAt
        };

    // ========================================================================
    // GET /api/v1/content-items/{id}/review - Get review status for a content item
    // ========================================================================

    public async Task<ApiResponseDto<ReviewStatusResponseDto>> GetReviewStatusAsync(Guid projectId, Guid targetId)
    {
        // 1. Verify the target exists in this project first
        if (!await IsTargetInProjectAsync(projectId, targetId))
            return ApiResponseDto<ReviewStatusResponseDto>.BadRequest("Target content not found or access denied.");

        var status = await _db.ReviewStatuses
            .FirstOrDefaultAsync(rs => rs.TargetId == targetId);

        if (status is null)
            return ApiResponseDto<ReviewStatusResponseDto>.NotFound($"No review status found for target {targetId}.");

        // 2. Validate user has access to the project
        var error = await ValidateProjectAccessAsync<ReviewStatusResponseDto>(projectId);
        if (error != null) return error;

        return ApiResponseDto<ReviewStatusResponseDto>.Success(MapToResponseDto(status));
    }

    // ========================================================================
    // PUT /api/v1/content-items/{id}/review - Approve/Reject content
    // ========================================================================

    public async Task<ApiResponseDto<string>> ApproveContentAsync(Guid projectId, Guid targetId, ApproveContentDto approveDto)
    {
        // 1. Verify the target exists and belongs to the project
        if (!await IsTargetInProjectAsync(projectId, targetId))
            return ApiResponseDto<string>.BadRequest("Target content not found or access denied.");

        var status = await _db.ReviewStatuses
            .FirstOrDefaultAsync(rs => rs.TargetId == targetId);

        // If no active review exists, we create a new one (or you could require existence)
        if (status == null)
        {
            status = new ReviewStatus 
            { 
                Id = Guid.NewGuid(), 
                TargetId = targetId, 
                CreatedAt = DateTime.UtcNow 
            };
            _db.ReviewStatuses.Add(status);
        }

        // 2. Validate user permissions
        var user = _userContext.CurrentUser;
        if (user == null) return ApiResponseDto<string>.Unauthorized("Not authenticated.");

        var isOwner = await _db.Projects.AnyAsync(p => p.Id == projectId && p.UserId == user.Id);
        var isMember = await _db.ProjectMembers.AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == user.Id && pm.Role >= ProjectMemberRoleEnum.Reviewer);

        if (!isOwner && !isMember)
            return ApiResponseDto<string>.Forbidden("You do not have permission to approve this content.");

        // 3. Update the status and metadata
        status.Status = approveDto.Status;
        status.ReviewedByUserId = user.Id;
        status.ReviewComment = approveDto.ReviewComment;
        status.ReviewedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return ApiResponseDto<string>.Success($"Content status updated to {approveDto.Status}.");
    }
}

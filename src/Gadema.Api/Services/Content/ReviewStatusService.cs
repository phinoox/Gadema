// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.MetaInfos;
using Gadema.Core.Dtos.Reviews;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Content;

/// <summary>
/// Service for managing content review workflow.
/// </summary>
public class ReviewStatusService : CoreService
{
    public ReviewStatusService(GameDbContext db, ILogger<ReviewStatusService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    private ReviewStatusResponseDto CreateResponseDto(ReviewStatus status)
        => new()
        {
            Id = status.Id,
            MetaInfoId = status.MetaInfoId,
            ContentType = status.ContentType,
            Status = status.Status,
            ReviewedByUserId = status.ReviewedByUserId,
            ReviewNotes = status.ReviewNotes,
            ReviewedAt = status.ReviewedAt,
            CreatedAt = status.CreatedAt,
        };

    // ========================================================================
    // GET /api/v1/content-items/{id}/review - Get review status for a content item
    // ========================================================================

    public async Task<ApiResponseDto<ReviewStatusResponseDto>> GetReviewStatusAsync(Guid id)
    {
        var status = await _db.ReviewStatuses
            .Include(rs => rs.MetaInfo)
            .FirstOrDefaultAsync(rs => rs.MetaInfoId == id);

        if (status is null)
            return ApiResponseDto<ReviewStatusResponseDto>.NotFound($"No review status found for content item {id}.");

        var error = await ValidateProjectAccessAsync<ReviewStatusResponseDto>(status.MetaInfo!.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<ReviewStatusResponseDto>.Success(CreateResponseDto(status));
    }

    // ========================================================================
    // PUT /api/v1/content-items/{id}/review - Approve/Reject content
    // ========================================================================

    public async Task<ApiResponseDto<string>> ApproveContentAsync(Guid id, ApproveContentDto approveDto)
    {
        var status = await _db.ReviewStatuses
            .Include(rs => rs.MetaInfo)
            .FirstOrDefaultAsync(rs => rs.MetaInfoId == id);

        if (status is null)
            return ApiResponseDto<string>.NotFound($"No review status found for content item {id}.");

        var error = await ValidateProjectAccessAsync<ReviewStatusResponseDto>(status.MetaInfo!.ProjectId);
        if (error != null) return error;

        var user = _userContext.CurrentUser;
        if (user == null)
            return ApiResponseDto<string>.Unauthorized("Not authenticated.");

        // Check role: Owner, Admin, or Editor (Reviewer is the minimum for approval)
        var isOwner = await _db.Projects.AnyAsync(p => p.Id == status.MetaInfo.ProjectId && p.UserId == user.Id);
        var isMember = await _db.ProjectMembers.AnyAsync(pm => pm.ProjectId == status.MetaInfo.ProjectId && pm.UserId == user.Id && pm.Role >= ProjectMemberRoleEnum.Reviewer);

        if (!isOwner && !isMember)
            return ApiResponseDto<string>.Forbidden("You do not have permission to approve this content.");

        // Update MetaInfo status
        ApplyMetaInfoUpdates(status.MetaInfo, new UpdateMetaInfoDto { Status = (int)approveDto.Status });

        status.Status = (int)approveDto.Status;
        status.ReviewedByUserId = user.Id;
        status.ReviewNotes = approveDto.Notes;
        status.ReviewedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return ApiResponseDto<string>.Success($"Content status updated to {approveDto.Status}.");
    }

    // Helper to map ContentTypeEnum to int for the model
    private ContentTypeEnum GetContentTypeFromMetaInfo(MetaInfo metaInfo) => metaInfo.ContentType;
}

// Helper DTO for the controller
public class ApproveContentDto
{
    public ReviewStatusEnum Status { get; set; }
    public string? Notes { get; set; }
}
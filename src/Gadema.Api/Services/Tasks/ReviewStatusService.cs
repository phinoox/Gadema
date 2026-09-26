using Gadema.Api.CoreServices;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Reviews;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Enums;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Data.Database.Core;
using Gadema.Data.Database.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Tasks;

/// <summary>
/// Service for managing content review workflow.
/// </summary>
[ServiceLifetime(ServiceLifetime.Scoped)] public class ReviewStatusService : DomainService
{
    private TaskDbContext _db;

    public ReviewStatusService( TaskDbContext db,
        ILogger<ReviewStatusService> logger,  
        CoreServicesProvider coreServices) // Injected via CoreService constructor
        : base(coreServices,logger) { _db = db; }

    private ReviewStatusResponseDto MapToResponseDto(ReviewStatus status)
        => new()
        {
            Id = status.Id,
            TargetId = status.TargetId, 
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
        // 1. Check permission to view the project scope
        var error = await CheckAccessAsync<ReviewStatusResponseDto>(projectId, Permission.CanView);
        if (error != null) return error;

        // 2. Verify target existence and ownership via IsTargetInProjectAsync
        if (!await IsTargetInProjectAsync(projectId, targetId))
            return ApiResponseDto<ReviewStatusResponseDto>.BadRequest("Target content not found or access denied.");

        var status = await _db.ReviewStatuses
            .FirstOrDefaultAsync(rs => rs.TargetId == targetId);

        if (status is null)
            return ApiResponseDto<ReviewStatusResponseDto>.NotFound($"No review status found for target {targetId}.");

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

        // If no active review exists, we create a new one
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

        // 2. Validate user permissions via the PermissionEngine (using CanEdit or a specialized permission)
        // For this implementation, we use CanEdit as the authority to change content status.
        var error = await CheckAccessAsync<string>(projectId, Permission.CanEdit);
        if (error != null) return error;

        // 3. Update the status and metadata
        status.Status = approveDto.Status;
        status.ReviewedByUserId = _userId;
        status.ReviewComment = approveDto.ReviewComment;
        status.ReviewedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return ApiResponseDto<string>.Success($"Content status updated to {approveDto.Status}.");
    }
}
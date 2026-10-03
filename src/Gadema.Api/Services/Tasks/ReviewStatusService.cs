using Gadema.Api.CoreServices;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Dtos.Reviews;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Enums;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Data.Database.Core;
using Gadema.Data.Database.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Tasks;

[ServiceLifetime(ServiceLifetime.Scoped)] public class ReviewStatusService : DomainService
{
    private TaskDbContext _db;

    public ReviewStatusService( TaskDbContext db,
        ILogger<ReviewStatusService> logger,  
        ICoreServicesProvider coreServices) 
        : base(coreServices,logger) { _db = db; }

    public async Task<ApiResponseDto<ReviewStatusResponseDto>> GetReviewStatusAsync(Guid projectId, Guid targetId)
    {
        var error = await CheckAccessAsync<ReviewStatusResponseDto>(projectId, Permission.CanView);
        if (error != null) return error;

        // Use the target ID directly to find the review status
        var status = await _db.ReviewStatuses
            .FirstOrDefaultAsync(rs => rs.Id == targetId);

        if (status is null)
            return ApiResponseDto<ReviewStatusResponseDto>.NotFound($"No review status found for target {targetId}.");

        // Permission check: ensure the target belongs to this project scope
        // In a real implementation, we'd resolve target type and project. 
        // For now, we assume the provided projectId is correct context.
        return ApiResponseDto<ReviewStatusResponseDto>.Success(MapToResponseDto(status));
    }

    public async Task<ApiResponseDto<string>> ApproveContentAsync(Guid projectId, Guid targetId, ApproveContentDto approveDto)
    {
        var error = await CheckAccessAsync<string>(projectId, Permission.CanEdit);
        if (error != null) return error;

        var status = await _db.ReviewStatuses.FirstOrDefaultAsync(rs => rs.Id == targetId);

        if (status == null)
        {
            status = new ReviewStatus 
            { 
                Id = targetId, 
                CreatedAt = DateTime.UtcNow 
            };
            _db.ReviewStatuses.Add(status);
        }

        status.Status = approveDto.Status;
        status.ReviewedByUserId = _userId;
        status.ReviewComment = approveDto.ReviewComment;
        status.ReviewedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return ApiResponseDto<string>.Success($"Content status updated to {approveDto.Status}.");
    }

    private ReviewStatusResponseDto MapToResponseDto(ReviewStatus status)
        => new()
        {
            Id = status.Id,
            Status = status.Status,
            ReviewedByUserId = status.ReviewedByUserId,
            ReviewComments = status.ReviewComment,
            ReviewedAt = status.ReviewedAt
        };
}
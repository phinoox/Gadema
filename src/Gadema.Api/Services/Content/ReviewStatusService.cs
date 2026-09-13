// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.MetaInfos;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Dtos.Reviews;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Content;

/// <summary>
/// Service for managing content review workflow: Draft → In Review → Published → Archived.
/// Handles approval/rejection with role-based permissions and audit trail.
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
            ContentType = (int)status.ContentType,
            Status = (int)status.Status,
            ReviewedByUserId = status.ReviewedByUserId,
            ReviewNotes = status.ReviewNotes,
            ReviewedAt = status.ReviewedAt,
            CreatedAt = status.CreatedAt,
        };

    private ListResponseDto<ReviewStatusResponseDto> CreateListResponseDto(IEnumerable<ReviewStatus> statuses)
        => new() { Items = statuses.Select(CreateResponseDto).ToList(), TotalCount = statuses.Count() };

    // ========================================================================
    // GET /api/v1/content/reviews - List reviews with optional status filter
    // ========================================================================

    public async Task<ApiResponseDto<ListResponseDto<ReviewStatusResponseDto>>> GetReviewsAsync(
        Guid projectId,
        ContentStatusEnum? status = null)
    {
        var error = await ValidateProjectAccessAsync<ListResponseDto<ReviewStatusResponseDto>>(projectId);
        if (error != null) return error;

        IQueryable<ReviewStatus> query = _db.ReviewStatuses
            .Include(rs => rs.MetaInfo)
            .Where(rs => rs.MetaInfo.ProjectId == projectId)
            .OrderByDescending(rs => rs.CreatedAt);

        if (status.HasValue) query = query.Where(rs => rs.Status == status.Value);

        var statuses = await query.ToListAsync();
        return ApiResponseDto<ListResponseDto<ReviewStatusResponseDto>>.Success(CreateListResponseDto(statuses));
    }

    // ========================================================================
    // POST /api/v1/content/reviews - Create a new review request
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateReviewAsync(
        Guid projectId,
        ReviewStatusCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        // Find existing MetaInfo or create new one
        var metaInfo = _db.MetaInfos.FirstOrDefault(m => m.Id == createDto.MetaInfoId && m.ProjectId == projectId);

        if (metaInfo is null)
            metaInfo = CreateMetaInfo(projectId, ContentTypeEnum.ContentReview, createDto.CreateData);

        // Auto-apply MetaInfo status change from Draft → InReview/Reviewed
        if (createDto.Status.HasValue && createDto.Status.Value != ContentStatusEnum.Draft && metaInfo.Status == ContentStatusEnum.Draft)
            ApplyMetaInfoUpdates(metaInfo, new UpdateMetaInfoDto { Status = (int)createDto.Status.Value });

        var status = new ReviewStatus
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id,
            ContentType = (int)metaInfo.ContentType,
            Status = createDto.Status ?? (int)ContentStatusEnum.Draft,
            ReviewedByUserId = _userContext.CurrentUser!.Id,
            ReviewNotes = createDto.ReviewNotes,
            CreatedAt = DateTime.UtcNow,
        };

        _db.ReviewStatuses.Add(status);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto { EntityId = status.Id, MetaInfoId = metaInfo.Id, ProjectId = projectId });
    }

    // ========================================================================
    // PUT /api/v1/content/reviews/{id}/approve - Approve content (moves to Published)
    // ========================================================================

    public async Task<ApiResponseDto<string>> ApproveAsync(Guid id)
    {
        var status = await _db.ReviewStatuses
            .Include(rs => rs.MetaInfo).FirstOrDefaultAsync(rs => rs.Id == id);

        if (status is null) return ApiResponseDto<string>.NotFound($"Review with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<ReviewStatusResponseDto>(status.MetaInfo.ProjectId);
        if (error != null) return error;

        // Verify reviewer has appropriate role (Reviewer, Editor, or Admin)
        var user = _userContext.CurrentUser;
        if (!HasReviewerRole(user, status.MetaInfo.ProjectId))
            return ApiResponseDto<string>.Forbidden("You are not authorized to approve reviews.");

        // Update MetaInfo status in parallel with ReviewStatus
        ApplyMetaInfoUpdates(status.MetaInfo, new UpdateMetaInfoDto { Status = (int)ContentStatusEnum.Published });
        _db.ChangeTracker.NotifyChanged(status.MetaInfo);

        status.Status = (int)ContentStatusEnum.Published;
        status.ReviewedByUserId = user.Id;
        status.ReviewNotes = "Approved by " + user.Name;
        status.ReviewedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return ApiResponseDto<string>.Success("Review approved. Content has been published.");
    }

    // ========================================================================
    // PUT /api/v1/content/reviews/{id}/reject - Reject content (moves back to Draft)
    // ========================================================================

    public async Task<ApiResponseDto<string>> RejectAsync(Guid id, string notes)
    {
        var status = await _db.ReviewStatuses
            .Include(rs => rs.MetaInfo).FirstOrDefaultAsync(rs => rs.Id == id);

        if (status is null) return ApiResponseDto<string>.NotFound($"Review with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<ReviewStatusResponseDto>(status.MetaInfo.ProjectId);
        if (error != null) return error;

        ApplyMetaInfoUpdates(status.MetaInfo, new UpdateMetaInfoDto { Status = (int)ContentStatusEnum.Draft });
        _db.ChangeTracker.NotifyChanged(status.MetaInfo);

        status.Status = (int)ContentStatusEnum.Draft;
        status.ReviewedByUserId = _userContext.CurrentUser!.Id;
        status.ReviewNotes = $"Rejected: {notes}";
        status.ReviewedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return ApiResponseDto<string>.Success("Review rejected. Content has been returned to Draft.");
    }

    // ========================================================================
    // HELPER: Check if user has reviewer-level role in project team
    // ========================================================================

    private async Task<bool> HasReviewerRole(User user, Guid projectId)
    {
        var projectTeam = await _db.ProjectMembers.FirstOrDefaultAsync(pt => pt.ProjectId == projectId);
        if (projectTeam is null) return false;

        var reviewerRoles = Enum.GetValues(typeof(TeamMemberRoleEnum))
            .Cast<TeamMemberRoleEnum>()
            .Where(r => r == TeamMemberRoleEnum.Reviewer || r == TeamMemberRoleEnum.Editor || r == TeamMemberRoleEnum.Admin);

        return projectTeam.TeamMembers.Any(tm => tm.UserId == user.Id && reviewerRoles.Contains(tm.Role));
    }
}
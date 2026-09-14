// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Enums;

using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services;

/// <summary>
/// Base class for all domain services. Provides shared infrastructure:
/// - Database context access
/// - User authentication via IUserContext
/// - Project access validation (ownership + team membership)
/// - Project ID consistency validation (route vs body)
/// - Slug generation utility
/// - MetaInfo creation helper
/// - MetaInfo update application helper
/// </summary>
public abstract class CoreService
{
    protected readonly GameDbContext _db;
    protected readonly ILogger _logger;
    protected readonly IUserContext _userContext;

    protected CoreService(GameDbContext db, ILogger logger, IUserContext userContext)
    {
        _db = db;
        _logger = logger;
        _userContext = userContext;
    }

    // ========================================================================
    // AUTHORIZATION HELPERS
    // ========================================================================

    /// <summary>
    /// Validates that the current user has access to the specified project.
    /// Checks both direct ownership and team membership.
    /// Returns null if access is granted, or an error ApiResponseDto if denied.
    /// </summary>
    protected async Task<ApiResponseDto<T>?> ValidateProjectAccessAsync<T>(Guid projectId, ProjectMemberRoleEnum minimumRole = ProjectMemberRoleEnum.Viewer) where T : class
    {
        var user = _userContext.CurrentUser;
        if (user == null)
            return ApiResponseDto<T>.Unauthorized("Not authenticated.");

        // Owner always has access
        var isOwner = await _db.Projects
            .AnyAsync(p => p.Id == projectId && p.UserId == user.Id);

        if (isOwner)
            return null;

        // Check membership with minimum role
        var member = await _db.ProjectMembers
            .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == user.Id);

        if (member == null || member.Role > minimumRole)
            return ApiResponseDto<T>.Forbidden("You do not have sufficient access to this project.");

        return null;
    }

    /// <summary>
    /// Verifies that the target ID provided actually belongs to a MetaInfo entry within the specified project.
    /// This is crucial for ancillary data like Comments or Tags.
    /// </summary>
    protected async Task<bool> IsTargetInProjectAsync(Guid projectId, Guid targetId)
    {
        return await _db.MetaInfos.AnyAsync(m => m.Id == targetId && m.ProjectId == projectId);
    }

    /// <summary>
    /// Records a business-level audit event into the database.
    /// This is for accountability (e.g., "User X updated Character Y").
    /// </summary>
    protected async Task LogAsync(
        Guid projectId, 
        string action, 
        string relatedEntityType, 
        Guid? relatedEntityId = null, 
        string? description = null)
    {
        var log = new ActivityLog
        {
            ProjectId = projectId,
            UserId = _userContext.CurrentUser?.Id ?? Guid.Empty, // Fallback if user is system/anonymous
            Action = action,
            RelatedEntityType = relatedEntityType,
            RelatedEntityId = relatedEntityId,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };

        _db.ActivityLogs.Add(log);
        // We await here to ensure the audit trail is persisted alongside the business transaction
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Validates that the ProjectId in the request body matches the route parameter.
    /// Ensures consistency between client intent and server routing.
    /// Returns null if IDs match, or a BadRequest error if they don't.
    /// </summary>
    protected Task<ApiResponseDto<T>?> ValidateProjectIdMatch<T>(Guid routeId, Guid bodyId) where T : class
    {
        if (routeId != bodyId)
            return Task.FromResult(ApiResponseDto<T>.BadRequest("Project ID in request body does not match route."));
        return Task.FromResult<ApiResponseDto<T>?>(null);
    }

    // ========================================================================
    // METAINFO HELPERS
    // ========================================================================

    /// <summary>
    /// Creates a new MetaInfo entity from creation data.
    /// Sets default ContentType, ViewMode, and timestamps.
    /// Generates a slug if none provided in the create data.
    /// </summary>
    protected MetaInfo CreateMetaInfo(Guid projectId, ContentTypeEnum contentType, MetaInfoCreateData createData)
    {
        var user = _userContext.CurrentUser;
        if (user == null)
            throw new UnauthorizedAccessException("Not authenticated.");

        return new MetaInfo
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            ContentType = contentType,
            Title = createData.Title,
            Slug = string.IsNullOrWhiteSpace(createData.Slug)
                ? GenerateSlug(createData.Title)
                : createData.Slug,
            ShortDesc = createData.ShortDesc,
            Status = createData.Status,
            IsPublic = createData.IsPublic,
            ViewMode = ViewModeEnum.PrivateWriting,
            CreatedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Applies MetaInfoUpdateData fields to an existing MetaInfo entity.
    /// Only non-null/non-empty fields are applied (partial update pattern).
    /// Returns true if any field was actually updated.
    /// </summary>
    protected bool ApplyMetaInfoUpdates(MetaInfo metaInfo, MetaInfoUpdateData? updateData)
    {
        if (updateData == null) return false;

        if (!string.IsNullOrWhiteSpace(updateData.Title))
            metaInfo.Title = updateData.Title;

        if (!string.IsNullOrWhiteSpace(updateData.Slug))
            metaInfo.Slug = updateData.Slug;

        if (updateData.ShortDesc != null)
            metaInfo.ShortDesc = updateData.ShortDesc;

        if (updateData.Status.HasValue)
            metaInfo.Status = updateData.Status.Value;

        if (updateData.IsPublic.HasValue)
            metaInfo.IsPublic = updateData.IsPublic.Value;

        return true;
    }

    // ========================================================================
    // UTILITY HELPERS
    // ========================================================================

    /// <summary>
    /// Generates a URL-friendly slug from a title string.
    /// Lowercases, replaces spaces/underscores with hyphens, removes special characters.
    /// </summary>
    protected static string GenerateSlug(string title)
    {
        var slug = title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-");

        foreach (var c in new[] { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')' })
            slug = slug.Replace(c.ToString(), string.Empty);

        return slug;
    }
}

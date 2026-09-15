// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Enums;
using Gadema.Core.Interfaces.Identity;
using Gadema.Core.Models;
using Gadema.Core.Models.Base.MetaInfo;
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
/// - ContentMetaInfo creation helper
/// - ContentMetaInfo update application helper
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
/// </summary>
protected async Task<bool> IsTargetInProjectAsync(Guid projectId, Guid targetId)
{
    // We use OfType<T> because it is compatible with EF Core's SQL translation.
    // This checks if the ID exists in any of our known subtype tables and matches the ProjectId.
    return await _db.Set<BaseMetaInfo>().OfType<ContentMetaInfo>().AnyAsync(m => m.Id == targetId && m.ProjectId == projectId) ||
           await _db.Set<BaseMetaInfo>().OfType<ProjectMetaInfo>().AnyAsync(m => m.Id == targetId && m.ProjectId == projectId);
}

    /// <summary>
    /// Checks if the current user has a specific role within the given project.
    /// </summary>
    protected async Task<bool> HasRoleAsync(Guid projectId, ProjectMemberRoleEnum role)
    {
        if (_userContext.CurrentUser == null) return false;

        return await _db.ProjectMembers.AnyAsync(pm => 
            pm.ProjectId == projectId && 
            pm.UserId == _userContext.CurrentUser.Id && 
            pm.Role >= role); // Using >= allows roles to inherit permissions (e.s. Admin > Editor)
    }

    /// <summary>
    /// Convenience wrapper to check if the current user is an Administrator for a project.
    /// </summary>
    protected async Task<bool> IsAdminAsync(Guid projectId)
    {
        return await HasRoleAsync(projectId, ProjectMemberRoleEnum.Admin);
    }

    /// <summary>
    /// Records a business-level audit event into the database.
    /// This is for accountability (e.g., "User X updated Character Y").
    /// </summary>
    protected async Task LogDbAsync(
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
    // ContentMetaInfo HELPERS
    // ========================================================================

     /// <summary>
    /// [Obsolete] Use the generic CreateMetaInfo<T> instead.
    /// </summary>
    protected ContentMetaInfo CreateMetaInfo(Guid projectId, ContentTypeEnum contentType, BaseMetaInfoCreateData createData)
    {
        var user = _userContext.CurrentUser;
        if (user == null)
            throw new UnauthorizedAccessException("Not authenticated.");

        return new ContentMetaInfo
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            ContentType = contentType,
            Title = createData.Title,
            Slug = string.IsNullOrWhiteSpace(createData.Slug)
                ? GenerateSlug(createData.Title)
                : createData.Slug,
            ShortDesc = createData.ShortDesc,
           // Status = createData.Status,
            IsPublic = createData.IsPublic,
            ViewMode = ViewModeEnum.PrivateWriting,
            CreatedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// [Obsolete] Use ApplyIdentitySyncAsync with an IIdentitySyncStrategy instead.
    /// </summary>
    [Obsolete("Use ApplyIdentitySyncAsync with a specialized strategy to ensure consistent sync logic.")]
    protected bool ApplyMetaInfoUpdates(ContentMetaInfo ContentMetaInfo, BaseMetaInfoUpdateData? updateData)
    {
        if (updateData == null) return false;

        if (!string.IsNullOrWhiteSpace(updateData.Title))
            ContentMetaInfo.Title = updateData.Title;

        if (!string.IsNullOrWhiteSpace(updateData.Slug))
            ContentMetaInfo.Slug = updateData.Slug;

        if (updateData.ShortDesc != null)
            ContentMetaInfo.ShortDesc = updateData.ShortDesc;

        if (updateData.Status.HasValue)
            ContentMetaInfo.Status = updateData.Status.Value;

        if (updateData.IsPublic.HasValue)
            ContentMetaInfo.IsPublic = updateData.IsPublic.Value;

        return true;
    }

    /// <summary>
    /// Creates a new MetaInfo anchor of type T.
    /// Handles universal properties like Title, Slug, and IsPublic.
    /// </summary>
    /// <typeparam name="T">The specific MetaInfo implementation (e.g., ProjectMetaInfo).</typeparam>
    /// <param name="createData">Initial data from the request body.</param>
    /// <param name="initialize">A delegate to handle domain-specific initialization (like assigning ProjectId or User).</param>
    protected T CreateMetaInfo<T>(BaseMetaInfoCreateData createData, Action<T> initialize) where T : BaseMetaInfo
    {
        var user = _userContext.CurrentUser;
        if (user == null)
            throw new UnauthorizedAccessException("Not authenticated.");

        // 1. Instantiate the specific MetaInfo type
        T meta = Activator.CreateInstance<T>();

        // 2. Apply universal properties from createData
        meta.Title = createData.Title;
        meta.Slug = string.IsNullOrWhiteSpace(createData.Slug)
            ? GenerateSlug(createData.Title)
            : createData.Slug;
        meta.IsPublic = createData.IsPublic;
        meta.ShortDesc = createData.ShortDesc;
        meta.CreatedAt = DateTime.UtcNow;
        meta.LastModifiedAt = DateTime.UtcNow;

        // 3. Let the caller handle domain-specific initialization (e.g., setting ProjectId)
        initialize(meta);

        return meta;
    }

     /// <summary>
    /// Executes a specialized identity update using the provided strategy.
    /// This allows for polymorphic updates of Title, Slug, Status, and Tags.
    /// </summary>
    protected async Task<bool> ApplyIdentitySyncAsync(
        Guid identityId, 
        BaseMetaInfoUpdateData updateData, 
        IIdentitySyncStrategy strategy)
    {
        // The transaction is managed by the calling service to ensure 
        // atomicity across both domain and identity updates.
        await strategy.SyncAsync(identityId, updateData);
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

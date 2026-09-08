// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services;

/// <summary>
/// Base class for all domain services. Provides shared infrastructure:
/// - Database context access
/// - User authentication via IUserContext
/// - Project access validation (ownership + team membership)
/// - Slug generation utility
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

    /// <summary>
    /// Validates that the current user has access to the specified project.
    /// Checks both direct ownership and team membership.
    /// Returns null if access is granted, or an error ApiResponseDto if denied.
    /// </summary>
    protected async Task<ApiResponseDto<T>?> ValidateProjectAccessAsync<T>(Guid projectId) where T : class
    {
        var user = _userContext.CurrentUser;
        if (user == null)
            return ApiResponseDto<T>.Unauthorized("Not authenticated.");

        // Check 1: Direct project ownership
        var isOwner = await _db.Projects
            .AnyAsync(p => p.Id == projectId && p.UserId == user.Id);

        if (isOwner)
            return null; // ✅ Owner has access

        // Check 2: Team-based access
        // User → TeamMember → ProjectTeam → Project
        var isInTeam = await _db.ProjectTeams
            .Where(pt => pt.ProjectId == projectId)
            .Join(
                _db.TeamMembers,
                pt => pt.TeamId,
                tm => tm.TeamId,
                (pt, tm) => tm.UserId
            )
            .AnyAsync(userId => userId == user.Id);

        if (!isInTeam)
            return ApiResponseDto<T>.Forbidden("You do not have access to this project.");

        return null; // ✅ Team member has access
    }

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

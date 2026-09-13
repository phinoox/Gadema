// =============================================================================
using System.Text.Json;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Activities;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Activities;

/// <summary>
/// Service for managing ActivityLogs - audit trail of all project operations.
/// Logs every meaningful action taken by users in the application.
/// </summary>
public class ActivityLogService : CoreService
{
    public ActivityLogService(GameDbContext db, ILogger<ActivityLogService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    /// <summary>Creates a response DTO from an ActivityLog entity.</summary>
    private ActivityLogResponseDto CreateResponseDto(ActivityLog log)
        => new()
        {
            Id = log.Id,
            UserId = log.UserId,
            Action = (int)log.Action,
            EntityId = log.EntityId,
            EntityType = (int)log.EntityType,
            ProjectId = log.ProjectId,
            Metadata = log.Metadata,
            CreatedAt = log.CreatedAt,
        };

    // ========================================================================
    // GET - List activity logs for a project (paginated, filterable)
    // ========================================================================

    public async Task<ApiResponseDto<PagedResponseDto<ActivityLogResponseDto>>> GetLogsAsync(Guid projectId, int? actionId = null, string? entityId = null, DateTime? startDate = null, DateTime? endDate = null, int page = 1, int pageSize = 50)
    {
        var error = await ValidateProjectAccessAsync<PagedResponseDto<ActivityLogResponseDto>>(projectId);
        if (error != null) return error;

        var query = _db.ActivityLogs
            .Where(l => l.ProjectId == projectId)
            .OrderByDescending(l => l.CreatedAt);

        if (actionId.HasValue)
            query = query.Where(l => l.Action == actionId.Value);

        if (!string.IsNullOrWhiteSpace(entityId))
            query = query.Where(l => l.EntityId == entityId);

        if (startDate.HasValue)
            query = query.Where(l => l.CreatedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(l => l.CreatedAt <= endDate.Value);

        var total = await query.CountAsync();
        var logs = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return ApiResponseDto<PagedResponseDto<ActivityLogResponseDto>>.Success(new PagedResponseDto<ActivityLogResponseDto>
        {
            Items = logs.Select(CreateResponseDto).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
    }

    // ========================================================================
    // POST - Create an activity log entry (used internally by other services)
    // ========================================================================

    public async Task<ApiResponseDto<string>> LogActivityAsync(Guid projectId, ActivityTypeEnum action, Guid? entityId = null, string? entityType = null, Dictionary<string, object>? metadata = null)
    {
        var user = _userContext.CurrentUser;
        if (user == null)
            return ApiResponseDto<string>.Unauthorized("Not authenticated.");

        var log = new ActivityLog
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            UserId = user.Id,
            Action = (int)action,
            EntityId = entityId,
            EntityType = entityType?.ToLowerInvariant() ?? "",
            Metadata = metadata != null ? JsonSerializer.Serialize(metadata) : null,
            CreatedAt = DateTime.UtcNow,
        };

        _db.ActivityLogs.Add(log);
        await _db.SaveChangesAsync();

        return ApiResponseDto<string>.Success("Activity logged successfully.");
    }
}
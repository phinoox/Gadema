using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Infrastructure;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Content;

/// <summary>
/// Service for querying the project's business audit trail.
/// </summary>
[ServiceLifetime(ServiceLifetime.Scoped)] public class ActivityLogService : CoreService
{
    public ActivityLogService(GameDbContext db, ILogger<ActivityLogService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    public async Task<ApiResponseDto<IEnumerable<ActivityLogResponseDto>>> GetLogsAsync(Guid projectId, int page = 1, int pageSize = 20)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<ActivityLogResponseDto>>(projectId);
        if (error != null) return error;

        var logs = await _db.ActivityLogs
            .Where(l => l.ProjectId == projectId)
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return ApiResponseDto<IEnumerable<ActivityLogResponseDto>>.Success(logs.Select(MapToResponseDto));
    }

    private ActivityLogResponseDto MapToResponseDto(ActivityLog log)
        => new()
        {
            Id = log.Id,
            Action = log.Action,
            RelatedEntityType = log.RelatedEntityType,
            RelatedEntityId = log.RelatedEntityId,
            Description = log.Description,
            CreatedAt = log.CreatedAt,
            ProjectId = log.ProjectId
        };
}
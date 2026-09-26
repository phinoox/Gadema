using Gadema.Api.CoreServices.Interfaces;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Access;
using Gadema.Core.Models.Base.Infrastructure;
using Gadema.Data.Database.Core;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.CoreServices.Services;

public class AuditService : IAuditService
{
    private readonly CoreDbContext _db;
    private readonly IUserContext _userContext;
    private readonly ILogger<AuditService> _logger;

    public AuditService(CoreDbContext db, IUserContext userContext, ILogger<AuditService> logger)
    {
        _db = db;
        _userContext = userContext;
        _logger = logger;
    }

    public async Task LogDbAsync(Guid? projectId, string action, string relatedEntityType, Guid? relatedEntityId = null, string? description = null)
    {
        var actualProjectId = projectId ?? throw new ArgumentException("ProjectId is required for activity logging.");
        var actualUser = _userContext.CurrentUser?.Id ?? throw new ArgumentException("User is required for activity logging.");

        var log = new ActivityLog
        {
            ProjectId = actualProjectId,
            UserId = actualUser,
            Action = action,
            RelatedEntityType = relatedEntityType,
            RelatedEntityId = relatedEntityId,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };

        _db.ActivityLogs.Add(log);
        await _db.SaveChangesAsync();
    }
}
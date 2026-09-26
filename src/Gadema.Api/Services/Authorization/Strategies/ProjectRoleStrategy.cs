using Gadema.Core.Models.Base.Permissions;
using Gadema.Core.Models.Base.Projects;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;
using Gadema.Api.Services.Authorization;
using Gadema.Core.Models.Access;
using Gadema.Data.Database.Core;

namespace Gadema.Api.Services.Authorization.Strategies;

/// <summary>
/// Strategy that evaluates permissions based on the user's role within a specific project scope.
/// </summary>
public class ProjectRoleStrategy : IPermissionStrategy
{
    private readonly CoreDbContext _db;

    public ProjectRoleStrategy(CoreDbContext db)
    {
        _db = db;
    }

    public async Task<bool> EvaluateAsync(Guid userId, Guid scopeId, Permission permission)
    {
        // 1. Fetch the user's role within this project scope
        var member = await _db.Set<ProjectMember>()
            .FirstOrDefaultAsync(pm => pm.UserId == userId && pm.ProjectId == scopeId);

        if (member == null) return false;

        // 2. Map Roles to Permissions
        return permission switch
        {
            Permission.CanView => true, // All members can view
            Permission.CanEdit => member.Role >= ProjectMemberRoleEnum.Editor,
            Permission.CanDelete => member.Role >= ProjectMemberRoleEnum.Admin,
            Permission.CanCreate => member.Role >= ProjectMemberRoleEnum.Editor,
            Permission.CanManageTags => member.Role >= ProjectMemberRoleEnum.Admin,
            _ => false
        };
    }
}

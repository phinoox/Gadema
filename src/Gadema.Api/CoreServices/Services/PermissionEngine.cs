using Gadema.Api.CoreServices.Interfaces;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Access;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Data.Database.Core;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Authorization;

/// <summary>
/// The central authority for evaluating authorization requests across the system.
/// </summary>
public class PermissionEngine : IPermissionEngine
{

    private readonly CoreDbContext _db;

    public PermissionEngine(CoreDbContext db)
    {
        _db = db;
    }

    public async Task<AccessResult> CheckAccessAsync(Guid userId, Guid projectId, ProjectMemberRoleEnum minRole)
    {
        if(userId == Guid.Empty)
            return new AccessResult(AccessResultStatus.Unauthorized,"You need to be logged in access this Project");
        // 1. Get the project regardless of deletion status
        var project = await _db.Projects
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
            return new AccessResult(AccessResultStatus.NotFound,"Project not found.");

        // 2. Determine User's Relationship/Role
        bool isOwner = project.UserId == userId;

        var member = await _db.ProjectMembers
            .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

        // 3. Apply Visibility Rule (The "Who can see a deleted project" rule)
        if (project.IsDeleted)
        {
            // Only Owners or Admins are allowed to 'see' a deleted project
            bool isAdmin = member != null && member.Role == ProjectMemberRoleEnum.Admin;
            if (!isOwner && !isAdmin)
            {
                return new AccessResult(AccessResultStatus.NotFound,"Project not found.");
            }
        }

        // 4. Apply Permission Rule (The "Can they perform this action" rule)
        if (isOwner) return null; // Owners always have access (even if deleted, per step 3)

        if (member == null || member.Role > minRole)
            return new AccessResult(AccessResultStatus.Forbidden,"You do not have sufficient access to this project");
            

         return new AccessResult(AccessResultStatus.Allowed,"");;
    }
    

}

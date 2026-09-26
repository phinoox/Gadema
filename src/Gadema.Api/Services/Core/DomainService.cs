using Gadema.Api.CoreServices;
using Gadema.Api.CoreServices.Interfaces;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Access;
using Gadema.Core.Models.Base.Permissions;

namespace Gadema.Api.Services.Core;

public abstract class DomainService
{
    protected CoreServicesProvider _core;
    protected readonly ILogger _logger;
    protected Guid _userId;

    public DomainService(CoreServicesProvider coreServices,ILogger logger)
    {
        _core = coreServices;
        _userId = _core.UserContext == null ? Guid.Empty : _core.UserContext.UserId ?? Guid.Empty;
        _logger = logger;
    }

    protected async Task LogDbAsync(Guid projectId, string action, string relatedEntityType, Guid relatedEntityId, string description)
    {
        await _core.AuditService.LogDbAsync(projectId,action,relatedEntityType,relatedEntityId,description);
    }


    /// <summary>
    /// checks if the user is logged in. later could also check if the user is deleted or banned etc
    /// </summary>
    /// <returns></returns>
    public async Task<bool> CheckIsLoggedIn()
    {
        return _userId != Guid.Empty;
    }

    protected async Task<bool> IsTargetInProjectAsync(Guid projectId, Guid targetId)
    {
        return true;//ToDo: actual implementation. basemetainfo doesnt have it. maybe better to implement a seperate metainfo/contentitem validation service
        // Check if the target is a piece of MetaInfo that belongs to the project
        /*var exists = await _db.Set<BaseMetaInfo>()
            .AnyAsync(m => m.Id == targetId && m.ProjectId == projectId);
        return exists;*/
    }


    public async Task<ApiResponseDto<T>> CheckAccessAsync<T>(Guid? projectId,Permission permission, ProjectMemberRoleEnum minRole = ProjectMemberRoleEnum.Default) where T : class
    {
        
        return await CheckAccessAsync<T>(projectId.Value,permission,minRole);
    }



    /// <summary>
    /// Checks if the current user has permission to perform a specific action on a resource.
    /// This is the preferred method for all authorization checks.
    /// wrapper for permissionengine.checkpermissionasync for convinience
    /// </summary>
    public async Task<ApiResponseDto<T>> CheckAccessAsync<T>(Guid projectId,Permission permission, ProjectMemberRoleEnum minRole = ProjectMemberRoleEnum.Default) where T : class
    {
        //ToDo: refine this once roles are more defined
        if(minRole == ProjectMemberRoleEnum.Default)
        {
            minRole = permission switch
            {
                Permission.CanDelete =>  ProjectMemberRoleEnum.Admin,
                Permission.CanView => ProjectMemberRoleEnum.Viewer,
                Permission.CanEdit => ProjectMemberRoleEnum.Editor,
                Permission.CanCreate => ProjectMemberRoleEnum.Admin,
                _ => ProjectMemberRoleEnum.Owner
            };
        }

        var result = await _core.PermissionEngine.CheckAccessAsync(_userId, projectId, minRole);

        return result.Status switch
        {
            AccessResultStatus.Allowed => ApiResponseDto<T>.Success(default!),
            AccessResultStatus.NotFound => ApiResponseDto<T>.NotFound(result.Message),
            AccessResultStatus.Forbidden => ApiResponseDto<T>.Forbidden(result.Message),
            AccessResultStatus.Unauthorized => ApiResponseDto<T>.Unauthorized(result.Message),
            _ => ApiResponseDto<T>.Unauthorized(result.Message)
        };
    }

    /// <summary>
    /// Checks if the current user has Admin rights for the project.
    /// Useful for administrative overrides or permanent deletions.
    /// </summary>
    public async Task<bool> IsAdminAsync(Guid projectId)
    {
        if (_userId == Guid.Empty) return false;

        // We use the PermissionEngine directly via the gateway to check specifically for Admin role
        var result = await _core.PermissionEngine.CheckAccessAsync(_userId, projectId, ProjectMemberRoleEnum.Admin);
        return result?.Status == AccessResultStatus.Allowed;
    }

    public async Task<bool> SyncIdentityAsync<T>(Guid identityId, BaseMetaInfoUpdateData updateData) where T : IIdentitySyncStrategy
    {
       return  await _core.MetadataService.SyncAsync<T>(identityId,updateData);
    }

}
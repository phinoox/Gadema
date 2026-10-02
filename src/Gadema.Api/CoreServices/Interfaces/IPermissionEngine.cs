

using Gadema.Core.Models.Access;
using Gadema.Core.Models.Base.Permissions;

namespace Gadema.Api.CoreServices.Interfaces;
public enum AccessResultStatus { Allowed, Forbidden, NotFound, Unauthorized }

public record AccessResult(AccessResultStatus Status, string Message = "");

public interface IPermissionEngine
{
    // Purely logical: No knowledge of ApiResponseDto or Web/HTTP context
    Task<AccessResult> CheckAccessAsync(Guid userId, Guid projectId, ProjectMemberRoleEnum minRole);
    Task<bool> IsTargetInProjectAsync(Guid projectId, Guid targetId);
}
namespace Gadema.Api.Services.Authorization;

using Gadema.Core.Models.Base.Permissions;

/// <summary>
/// Defines the contract for specialized permission evaluation strategies.
/// </summary>
public interface IPermissionStrategy
{
    /// <summary>
    /// Evaluates if the user has a specific permission within a given scope.
    /// </summary>
    Task<bool> EvaluateAsync(Guid userId, Guid scopeId, Permission permission);
}

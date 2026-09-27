using Gadema.Core.Dtos.Base.Infrastructure;

namespace Gadema.Core.Interfaces;

/// <summary>
/// Defines a strategy for synchronizing identity information across different systems or components.
/// </summary>
public interface IIdentitySyncStrategy
{
    /// <summary>
    /// Asynchronously synchronizes the identity with the provided update data.
    /// </summary>
    /// <param name="identityId">The unique identifier of the identity to sync.</param>
    /// <param name="updateData">The data used for updating the identity.</param>
    /// <returns>A task that represents the asynchronous sync operation.</returns>
    Task SyncAsync(Guid identityId, BaseMetaInfoUpdateData updateData);
}
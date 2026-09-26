using System;
using System.Threading.Tasks;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Infrastructure;

namespace Gadema.Api.CoreServices.Interfaces;

/// <summary>
/// Service for managing the creation and updates of MetaInfo anchors (the "Soul" of entities).
/// </summary>
public interface IMetadataService
{
    /// <summary>
    /// Creates a new MetaInfo anchor of type T.
    /// </summary>
    /// <typeparam name="T">The specific MetaInfo implementation.</typeparam>
    /// <param name="projectId">The project scope for this metadata.</param>
    /// <param name="createData">Initial data from the request body.</param>
    /// <param name="initialize">A delegate to handle domain-specific initialization (e.g., setting ProjectId).</param>
    Task<T> CreateAsync<T>(BaseMetaInfoCreateData createData, Action<T> initialize) where T : BaseMetaInfo;

    /// <summary>
    /// Applies updates from a DTO to an existing MetaInfo anchor.
    /// </summary>
    /// <param name="metaInfoId">The unique identifier of the metadata.</param>
    /// <param name="updateData">The update data containing new values.</param>
    Task ApplyUpdatesAsync(Guid metaInfoId, BaseMetaInfoUpdateData updateData);

    Task<bool> SyncAsync<T>(Guid identityId, BaseMetaInfoUpdateData updateData) where T : IIdentitySyncStrategy;

    Task<bool> DeleteAsync<T>(Guid identityId) where T : BaseMetaInfo;
}
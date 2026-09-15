using Gadema.Core.Dtos;

namespace Gadema.Core.Interfaces;

public interface IIdentitySyncStrategy
{
    Task SyncAsync(Guid identityId, BaseMetaInfoUpdateData updateData);
}
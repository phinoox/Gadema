using Gadema.Core.Dtos.Base.Infrastructure;

namespace Gadema.Core.Interfaces;

public interface IIdentitySyncStrategy
{
    Task SyncAsync(Guid identityId, BaseMetaInfoUpdateData updateData);
}
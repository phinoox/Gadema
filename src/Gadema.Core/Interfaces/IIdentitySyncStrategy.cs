using Gadema.Core.Dtos;

namespace Gadema.Core.Interfaces.Identity;

public interface IIdentitySyncStrategy
{
    Task SyncAsync(Guid identityId, MetaInfoUpdateData updateData);
}
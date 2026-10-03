using Gadema.Api.CoreServices.Strategies;
using Gadema.Core.Dtos.Access;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Models.Access;
using Gadema.Data.Database;
using Gadema.Data.Database.Core;

namespace Gadema.Api.CoreServices.Strategies;

/// <summary>
/// Strategy for synchronizing user profile information.
/// Handles mapping of UserProfileUpdateDto to UserMetaInfo and manages the AvatarUrl property.
/// </summary>
public class UserIdentityStrategy : BaseIdentityStrategy<UserProfileUpdateDto, UserMetaInfo>
{
    public UserIdentityStrategy(CoreDbContext db) : base(db) { }

    protected override async Task<UserMetaInfo?> FetchMetaInfo(Guid id, UserProfileUpdateDto updateData)
    {
        return await _db.Set<UserMetaInfo>().FindAsync(id);
    }

    protected override async Task ApplyDomainPropertiesAsync(Guid id, UserProfileUpdateDto updateData)
    {
        // AvatarUrl is not part of BaseMetaInfo, so we handle it here in the domain-specific step.
        if (MetaInfo != null && updateData.AvatarUrl != null)
        {
            MetaInfo.AvatarUrl = updateData.AvatarUrl;
        }
    }
}

using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Interfaces;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Tags.Strategies;

/// <summary>
/// The universal engine for identity synchronization.
/// Handles universal properties (Title, Slug, etc.) and delegates 
/// domain-specific updates and tag syncing to subclasses.
/// </summary>
public abstract class BaseIdentityStrategy<TUpdate, TMetaInfo> : IIdentitySyncStrategy 
    where TUpdate : BaseMetaInfoUpdateData
    where TMetaInfo : BaseMetaInfo
{
    protected readonly GameDbContext _db;

    protected BaseIdentityStrategy(GameDbContext db) => _db = db;

    // The shared anchor instance for the current operation
    protected TMetaInfo? MetaInfo;

    public async Task SyncAsync(Guid identityId, BaseMetaInfoUpdateData updateData)
    {
        if (updateData is not TUpdate specificUpdate) throw new ArgumentException("...");

        // 1. Fetch the MetaInfo anchor
        MetaInfo = await FetchMetaInfo(identityId, specificUpdate);
        if (MetaInfo == null) throw new Exception("MetaInfo not found.");

        // 2. Apply Universal Properties
        await ApplyUniversalPropertiesAsync(updateData);

        // 3. Perform the Tag Sync (Generic version)
        if (updateData.TagIds != null )
        {
            // We call a method that uses the generic relation class
            await SyncTagsInternalAsync<TMetaInfo>(identityId, updateData.TagIds);
        }

        // 4. Apply Domain Properties (Status, etc.)
        await ApplyDomainPropertiesAsync(identityId, specificUpdate);
    }

   

    // This is the "Magic" method that handles the generic database interaction
    protected async Task SyncTagsInternalAsync<T>(Guid id, List<Guid> tagIds) where T : BaseMetaInfo
    {
        // We use the generic TagRelation<T> class here
        var currentTags = await _db.Set<TagRelation<T>>()
            .Where(r => r.MetaInfoId == id)
            .Select(r => r.TagId)
            .ToListAsync();

        var toAdd = tagIds.Except(currentTags).ToList();
        var toRemove = currentTags.Except(tagIds).ToList();

        foreach (var tagId in toRemove)
        {
            var relation = await _db.Set<TagRelation<T>>()
                .FirstOrDefaultAsync(r => r.MetaInfoId == id && r.TagId == tagId);
            if (relation != null) _db.Set<TagRelation<T>>().Remove(relation);
        }

        foreach (var tagId in toAdd)
        {
            _db.Set<TagRelation<T>>().Add(new TagRelation<T> { MetaInfoId = id, TagId = tagId });
        }
    }
    private async Task ApplyUniversalPropertiesAsync(BaseMetaInfoUpdateData data)
    {
        if (data.Title != null) MetaInfo!.Title = data.Title;
        if (data.Slug != null) MetaInfo!.Slug = data.Slug;
        if (data.IsPublic.HasValue) MetaInfo!.IsPublic = data.IsPublic.Value;
        if (data.ShortDesc != null) MetaInfo!.ShortDesc = data.ShortDesc;

        MetaInfo!.LastModifiedAt = DateTime.UtcNow;
    }

    protected abstract Task<TMetaInfo?> FetchMetaInfo(Guid id, TUpdate updateData);
    protected abstract Task ApplyDomainPropertiesAsync(Guid id, TUpdate updateData);
}
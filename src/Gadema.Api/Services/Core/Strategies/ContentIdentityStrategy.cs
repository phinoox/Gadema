using Gadema.Core.Dtos;
using Gadema.Core.Interfaces.Identity;
using Gadema.Core.Models;
using Gadema.Core.Models.Base;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Tags.Strategies;

public class ContentIdentityStrategy : IIdentitySyncStrategy
{
    private readonly GameDbContext _db;

    public ContentIdentityStrategy(GameDbContext db) => _db = db;

    public async Task SyncAsync(Guid metaInfoId, BaseMetaInfoUpdateData updateData)
    {
        var ContentMetaInfo = await _db.MetaInfos.FindAsync(metaInfoId);
        if (ContentMetaInfo == null) throw new Exception("ContentMetaInfo not found.");

        // 1. Update Identity Properties
        if (updateData.Title != null) ContentMetaInfo.Title = updateData.Title;
        if (updateData.Slug != null) ContentMetaInfo.Slug = updateData.Slug;
        if (updateData.IsPublic.HasValue) ContentMetaInfo.IsPublic = updateData.IsPublic.Value;

        if (updateData is ContentMetaInfoUpdateData contentUpdate)
        {
            if (contentUpdate.Status.HasValue) 
                ContentMetaInfo.Status = contentUpdate.Status.Value;
            
            // If there were other properties unique to ContentMetaInfo, they would go here.
        }

        // 2. Sync Tags
        if (updateData.TagIds != null)
        {
            var currentTags = await _db.ContentTagRelations
                .Where(r => r.MetaInfoId == metaInfoId)
                .Select(r => r.TagId)
                .ToListAsync();

            var toAdd = updateData.TagIds.Except(currentTags);
            var toRemove = currentTags.Except(updateData.TagIds);

            foreach (var id in toRemove)
            {
                var relation = await _db.ContentTagRelations
                    .FirstOrDefaultAsync(r => r.MetaInfoId == metaInfoId && r.TagId == id);
                if (relation != null) _db.ContentTagRelations.Remove(relation);
            }

            foreach (var id in toAdd)
            {
                _db.ContentTagRelations.Add(new ContentTagRelation { MetaInfoId = metaInfoId, TagId = id });
            }
        }

        ContentMetaInfo.LastModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}
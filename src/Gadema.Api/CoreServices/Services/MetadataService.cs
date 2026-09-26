using Gadema.Api.CoreServices.Interfaces;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Infrastructure;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Data.Database.Core;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.CoreServices.Services;

public class MetadataService : IMetadataService
{
    private readonly CoreDbContext _db;

    public MetadataService(CoreDbContext db)
    {
        _db = db;
    }

    public async Task<T> CreateAsync<T>(BaseMetaInfoCreateData createData, Action<T> initialize) where T : BaseMetaInfo
    {
        // 1. Instantiate the specific MetaInfo type
        T meta = Activator.CreateInstance<T>();

        // 2. Apply universal properties from createData
        meta.Title = createData.Title;
        meta.Slug = string.IsNullOrWhiteSpace(createData.Slug)
            ? GenerateSlug(createData.Title)
            : createData.Slug;
        meta.IsPublic = createData.IsPublic;
        meta.ShortDesc = createData.ShortDesc;
        meta.CreatedAt = DateTime.UtcNow;
        meta.LastModifiedAt = DateTime.UtcNow;

        // 3. Let the caller handle domain-specific initialization (e.g., setting ProjectId)
        initialize(meta);

        _db.Set<T>().Add(meta);
        await _db.SaveChangesAsync();

        return meta;
    }

    public async Task<bool> SyncAsync<T>(Guid identityId, BaseMetaInfoUpdateData updateData) where T : IIdentitySyncStrategy
    {
        object instance = Activator.CreateInstance(typeof(T),[_db]);
        if(instance ==null)
            return false;
        IIdentitySyncStrategy strategy = (IIdentitySyncStrategy)(instance);

        if(strategy == null )
            return false;

        // 2. Delegate the actual property mapping to the strategy
        await strategy.SyncAsync(identityId, updateData); 
        return true;
    }

    public async Task ApplyUpdatesAsync(Guid metaInfoId, BaseMetaInfoUpdateData updateData)
    {
        var meta = await _db.Set<BaseMetaInfo>().FindAsync(metaInfoId);
        if (meta == null) throw new ArgumentException($"Metadata with ID {metaInfoId} not found.");

        if (!string.IsNullOrWhiteSpace(updateData.Title))
            meta.Title = updateData.Title;

        if (!string.IsNullOrWhiteSpace(updateData.Slug))
            meta.Slug = updateData.Slug;

        if (updateData.ShortDesc != null)
            meta.ShortDesc = updateData.ShortDesc;

        if (updateData.IsPublic.HasValue)
            meta.IsPublic = updateData.IsPublic.Value;

        meta.LastModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    private static string GenerateSlug(string title)
    {
        var slug = title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-");

        foreach (var c in new[] { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')' })
            slug = slug.Replace(c.ToString(), string.Empty);

        return slug;
    }

    public async Task<bool> DeleteAsync<T>(Guid identityId) where T : BaseMetaInfo
    {
        var meta = _db.Set<T>().FirstOrDefault(mi => mi.Id == identityId);
        if(meta == null)
            return false;
         _db.Set<T>().Remove(meta);
        await _db.SaveChangesAsync();
        return true;
    }
}
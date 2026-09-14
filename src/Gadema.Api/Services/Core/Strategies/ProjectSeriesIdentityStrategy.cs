using Gadema.Core.Dtos;
using Gadema.Core.Interfaces.Identity;
using Gadema.Core.Models.Projects;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Projects;

public class ProjectSeriesIdentityStrategy : IIdentitySyncStrategy
{
    private readonly GameDbContext _db;

    public ProjectSeriesIdentityStrategy(GameDbContext db) => _db = db;

    public async Task SyncAsync(Guid identityId, MetaInfoUpdateData updateData)
    {
        var meta = await _db.Set<ProjectSeriesMetaInfo>()
            .FirstOrDefaultAsync(m => m.ProjectSeriesId == identityId);

        if (meta == null) throw new Exception("ProjectSeriesMetaInfo not found.");

        // 1. Update Identity Properties from the updateData
        if (!string.IsNullOrWhiteSpace(updateData.Title)) meta.Title = updateData.Title;
        if (!string.IsNullOrWhiteSpace(updateData.Slug)) meta.Slug = updateData.Slug;
        if (updateData.ShortDesc != null) meta.ShortDesc = updateData.ShortDesc;

        // Note: Tag sync is omitted for this initial implementation to keep it simple.

        await _db.SaveChangesAsync();
    }
}
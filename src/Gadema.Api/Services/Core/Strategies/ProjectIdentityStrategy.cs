using Gadema.Core.Dtos;
using Gadema.Core.Enums;
using Gadema.Core.Interfaces.Identity;
using Gadema.Core.Models.Projects;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Tags.Strategies;

public class ProjectIdentityStrategy : IIdentitySyncStrategy
{
    private readonly GameDbContext _db;

    public ProjectIdentityStrategy(GameDbContext db) => _db = db;

    public async Task SyncAsync(Guid projectMetaInfoId, MetaInfoUpdateData basicUpdateData)
    {
        var updateData = (ProjectMetaInfoUpdateData)basicUpdateData;
        if(updateData == null)
            return;
        // We target the ProjectMetaInfo associated with this project
        var ContentMetaInfo = await _db.Set<ProjectMetaInfo>()
            .FirstOrDefaultAsync(mi => mi.ProjectId == projectMetaInfoId);

        if (ContentMetaInfo == null) throw new Exception("Project ContentMetaInfo not found.");

        // 1. Update Identity Properties from the updateData
        if (updateData.Title != null) ContentMetaInfo.Title = updateData.Title;
        if (updateData.Slug != null) ContentMetaInfo.Slug = updateData.Slug;
        
        // Correcting the Enum type mismatch:
        if (updateData.ProjectStatus.HasValue) 
            ContentMetaInfo.Status = (ProjectStatusEnum)updateData.ProjectStatus.Value;

        if (updateData.ViewMode.HasValue) ContentMetaInfo.ViewMode = updateData.ViewMode.Value;

        // 2. Sync Tags
        if (updateData.TagIds != null)
        {
            var currentTags = await _db.ProjectTagRelations
                .Where(r => r.ProjectMetaInfoId == projectMetaInfoId)
                .Select(r => r.TagId)
                .ToListAsync();

            var toAdd = updateData.TagIds.Except(currentTags).ToList();
            var toRemove = currentTags.Except(updateData.TagIds).ToList();

            foreach (var id in toRemove)
            {
                var relation = await _db.ProjectTagRelations
                    .FirstOrDefaultAsync(r => r.ProjectMetaInfoId == projectMetaInfoId && r.TagId == id);
                if (relation != null) _db.ProjectTagRelations.Remove(relation);
            }

            foreach (var id in toAdd)
            {
                _db.ProjectTagRelations.Add(new ProjectTagRelation 
                { 
                    ProjectMetaInfoId = projectMetaInfoId, 
                    TagId = id 
                });
            }
        }

        await _db.SaveChangesAsync();
    }
}
using Gadema.Core.Dtos;
using Gadema.Core.Enums;
using Gadema.Core.Interfaces.Identity;
using Gadema.Core.Models.Projects;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Tags.Strategies;

public class ProjectIdentityStrategy : BaseIdentityStrategy<ProjectMetaInfoUpdateData,ProjectMetaInfo>,IIdentitySyncStrategy
{
    
    public ProjectIdentityStrategy(GameDbContext db) :base(db) {}

    
    protected override async Task ApplyDomainPropertiesAsync(Guid id, ProjectMetaInfoUpdateData updateData)
    {
       // Correcting the Enum type mismatch:
        if (updateData.ProjectStatus.HasValue) 
            MetaInfo!.Status = (ProjectStatusEnum)updateData.ProjectStatus.Value;

        if (updateData.ViewMode.HasValue) MetaInfo!.ViewMode = updateData.ViewMode.Value;
    }

    protected override async Task<ProjectMetaInfo?> FetchMetaInfo(Guid id, ProjectMetaInfoUpdateData updateData)
    {
        return await _db.Set<ProjectMetaInfo>().FindAsync(id);
    }
}
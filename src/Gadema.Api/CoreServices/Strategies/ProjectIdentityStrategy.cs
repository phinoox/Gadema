using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Projects.Enums;
using Gadema.Data.Database;
using Gadema.Data.Database.Core;
using Gadema.Data.Database.Game;

namespace Gadema.Api.CoreServices.Strategies;

public class ProjectIdentityStrategy : BaseIdentityStrategy<ProjectMetaInfoUpdateData,ProjectMetaInfo>,IIdentitySyncStrategy
{
    
    public ProjectIdentityStrategy(CoreDbContext db) :base(db) {}

    
    protected override async Task ApplyDomainPropertiesAsync(Guid id, ProjectMetaInfoUpdateData updateData)
    {
       // Correcting the Enum type mismatch:
        if (updateData.ProjectStatus.HasValue) 
            MetaInfo!.Status = (ProjectStatusEnum)updateData.ProjectStatus.Value;

        if (updateData.ViewMode.HasValue) MetaInfo!.ViewMode = updateData.ViewMode.Value;
    }

    protected override async Task<ProjectMetaInfo?> FetchMetaInfo(Guid id, ProjectMetaInfoUpdateData updateData)
    {
        return await _db.ProjectMetaInfos.FindAsync(id);
    }
}
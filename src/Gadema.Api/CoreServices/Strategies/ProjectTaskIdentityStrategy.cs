using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Tasks;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Tasks;
using Gadema.Data.Database;
using Gadema.Data.Database.Core;
using Gadema.Data.Database.Game;

namespace Gadema.Api.CoreServices.Strategies;

//ToDo implement real version
public class ProjectTaskIdentityStrategy : BaseIdentityStrategy<BaseMetaInfoUpdateData,ProjectTaskMetaInfo>,IIdentitySyncStrategy
{
    
    public ProjectTaskIdentityStrategy(CoreDbContext db) :base(db) {}

    
    protected override async Task ApplyDomainPropertiesAsync(Guid id, BaseMetaInfoUpdateData updateData)
    {
       // Correcting the Enum type mismatch:
       //ToDo: update dtos, or rather create proper projecttaskmetainfoupdatedto .. meh, too long
    }

    protected override async Task<ProjectTaskMetaInfo?> FetchMetaInfo(Guid id, BaseMetaInfoUpdateData updateData)
    {
        return await _db.ProjectTaskMetaInfos.FindAsync(id);
    }
}
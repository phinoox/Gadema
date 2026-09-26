using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Data.Database;
using Gadema.Data.Database.Core;

namespace Gadema.Api.CoreServices.Strategies;

public class ContentIdentityStrategy : BaseIdentityStrategy<ContentMetaInfoUpdateData,ContentMetaInfo>
{
    public ContentIdentityStrategy(CoreDbContext db) : base(db) { }

    protected override async Task ApplyDomainPropertiesAsync(Guid id, ContentMetaInfoUpdateData updateData)
    {
      
        // Now we have access to both the specific entity AND the casted update data!
        if (updateData.Status.HasValue) 
            MetaInfo!.Status = updateData.Status.Value;
       
    }

    protected async override Task<ContentMetaInfo?> FetchMetaInfo(Guid id, ContentMetaInfoUpdateData updateData)
    {
       return await _db.Set<ContentMetaInfo>().FindAsync(id);
    }
}
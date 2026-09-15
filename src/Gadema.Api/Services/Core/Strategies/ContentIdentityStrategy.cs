using Gadema.Core.Dtos;
using Gadema.Core.Interfaces.Identity;
using Gadema.Core.Models;
using Gadema.Core.Models.Base;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Tags.Strategies;

public class ContentIdentityStrategy : BaseIdentityStrategy<ContentMetaInfoUpdateData,ContentMetaInfo>
{
    public ContentIdentityStrategy(GameDbContext db) : base(db) { }

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
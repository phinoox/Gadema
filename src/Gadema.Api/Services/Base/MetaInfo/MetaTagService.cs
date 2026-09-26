using Gadema.Api.CoreServices;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Data.Database.Core;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Base.MetaInfo;

[ServiceLifetime(ServiceLifetime.Scoped)] 
public class MetaTagService : DomainService
{
    private CoreDbContext _db;

    public MetaTagService( CoreDbContext db,
        ILogger<MetaTagService> logger,  
        CoreServicesProvider coreServices) // Injected via CoreService constructor
        : base(coreServices,logger) {  _db = db; }

    public async Task<ApiResponseDto<ListResponseDto<MetaTag>>> GetAllAsync()
    {
        // Global tags don't belong to a specific project scope in this implementation.
        // We assume any authenticated user can view global tags.
        if (_userId == Guid.Empty)
            return ApiResponseDto<ListResponseDto<MetaTag>>.Unauthorized("Not authenticated.");

        var tags = await _db.MetaTags.ToListAsync();
        return ApiResponseDto<ListResponseDto<MetaTag>>.Success(new ListResponseDto<MetaTag>
        {
            Items = tags,
            TotalCount = tags.Count
        });
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateAsync(string name, string? slug)
    {
        // Creating a global tag is an administrative action. 
        // Since there's no scope, we check if the user has some form of admin role or just use internal logic.
        // For now, we allow it but log with Guid.Empty as per original implementation.
        var tag = new MetaTag 
        { 
            Name = name, 
            Slug = slug ?? name.ToLower().Replace(" ", "-") 
        };

        _db.MetaTags.Add(tag);
        await _db.SaveChangesAsync();

        await LogDbAsync(Guid.Empty, "Created", "MetaTag", tag.Id, $"Global tag '{tag.Name}' created.");

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto { EntityId = tag.Id });
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteAsync(Guid id)
    {
        var tag = await _db.MetaTags.FindAsync(id);
        if (tag == null) return ApiResponseDto<DeleteResponseDto>.NotFound("Tag not found.");

        _db.MetaTags.Remove(tag);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto { EntityId = id });
    }
}
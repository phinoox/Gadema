using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Models.Tags;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Tags;

public class MetaTagService : CoreService
{
    public MetaTagService(GameDbContext db, ILogger<MetaTagService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    public async Task<ApiResponseDto<ListResponseDto<MetaTag>>> GetAllAsync()
    {
        var tags = await _db.MetaTags.ToListAsync();
        return ApiResponseDto<ListResponseDto<MetaTag>>.Success(new ListResponseDto<MetaTag>
        {
            Items = tags,
            TotalCount = tags.Count
        });
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateAsync(string name, string? slug)
    {
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
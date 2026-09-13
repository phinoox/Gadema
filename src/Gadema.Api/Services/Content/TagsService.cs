using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Dtos.Tags;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Content;

public class TagsService : CoreService
{
    public TagsService(GameDbContext db, ILogger<TagsService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    public async Task<ApiResponseDto<ListResponseDto<TagResponseDto>>> GetAllTagsAsync(
        int page = 1,
        int pageSize = 20)
    {
        var query = _db.Tags
            .OrderBy(t => t.Name)
            .Select(t => new TagResponseDto
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                UsageCount = _db.ProjectTagRelations.Count(r => r.TagId == t.Id)
            });

        var totalCount = await query.CountAsync();
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return ApiResponseDto<ListResponseDto<TagResponseDto>>.Success(new ListResponseDto<TagResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }
}
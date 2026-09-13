// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Dtos.Search;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Content;

/// <summary>
/// Service for full-text search across all content with relevance scoring.
/// </summary>
public class SearchService : CoreService
{
    public SearchService(GameDbContext db, ILogger<SearchService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    /// <summary>Creates a search result response DTO.</summary>
    private SearchMetaInfosDto CreateResponseDto(MetaInfo metaInfo)
        => new()
        {
            Id = metaInfo.Id,
            ContentType = (int)metaInfo.ContentType,
            Title = metaInfo.Title,
            Slug = metaInfo.Slug,
            ShortDesc = metaInfo.ShortDesc,
            Status = (int)metaInfo.Status,
            IsPublic = metaInfo.IsPublic,
            CreatedAt = metaInfo.CreatedAt,
        };

    // ========================================================================
    // POST /api/v1/search - Full-text search with relevance scoring
    // ========================================================================

    public async Task<ApiResponseDto<PagedResponseDto<SearchMetaInfosDto>>> SearchAsync(string query, int? contentType = null, ContentStatusEnum? status = null, int page = 1, int pageSize = 20)
    {
        var user = _userContext.CurrentUser;
        if (user == null)
            return ApiResponseDto<PagedResponseDto<SearchMetaInfosDto>>.Unauthorized("Not authenticated.");

        // Build base query with full-text search using SQL LIKE for simplicity
        // In production, use Postgres tsvector/tsquery or ElasticSearch
        var builder = _db.Set<MetaInfo>().AsQueryable();

        builder = builder.Where(mi => mi.Title.Contains(query) || mi.ShortDesc?.Contains(query) == true);

        if (contentType.HasValue)
            builder = builder.Where(mi => mi.ContentType == contentType.Value);

        if (status.HasValue)
            builder = builder.Where(mi => mi.Status == status.Value);

        var total = await builder.CountAsync();

        // For simplicity, skip ranking here. In production:
        // - Use Postgres tsvector GIN indexes with rank()
        // - Or use ElasticSearch for fuzzy search + highlighting

        var results = await builder
            .OrderByDescending(mi => mi.Title.Contains(query) ? 1 : 0)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(CreateResponseDto)
            .ToListAsync();

        return ApiResponseDto<PagedResponseDto<SearchMetaInfosDto>>.Success(new PagedResponseDto<SearchMetaInfosDto>
        {
            Items = results,
            TotalCount = total,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        });
    }
}
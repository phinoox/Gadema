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
/// Combines keyword + tag filtering, pagination, and status/content-type filters.
/// </summary>
public class ContentSearchService : CoreService
{
    public ContentSearchService(GameDbContext db, ILogger<ContentSearchService> logger, IUserContext userContext)
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

    /// <summary>Creates a list response DTO from collection.</summary>
    private ListResponseDto<SearchMetaInfosDto> CreateListResponseDto(IEnumerable<MetaInfo> results)
        => new() { Items = results.Select(CreateResponseDto).ToList(), TotalCount = results.Count() };

    // ========================================================================
    // POST /api/v1/search - Full-text search with keyword + tag filtering
    // ========================================================================

    public async Task<ApiResponseDto<ListResponseDto<SearchMetaInfosDto>>> SearchAsync(
        string query,
        int? contentType = null,
        ContentStatusEnum? status = null,
        string[]? tags = null,
        int page = 1,
        int pageSize = 20)
    {
        var user = _userContext.CurrentUser;
        if (user == null) return ApiResponseDto<ListResponseDto<SearchMetaInfosDto>>.Unauthorized("Not authenticated.");

        // Build base query with full-text search
        IQueryable<MetaInfo> query = _db.MetaInfos.AsQueryable();

        // Full-text search: matches title, slug, or short description
        query = query.Where(mi => mi.Title.Contains(query) || (mi.ShortDesc != null && mi.ShortDesc.Contains(query)));

        // Filter by content type
        if (contentType.HasValue) query = query.Where(mi => mi.ContentType == contentType.Value);

        // Filter by status
        if (status.HasValue) query = query.Where(mi => mi.Status == status.Value);

        // Tag-based filtering (requires MetaInfoTagRelation junction table in production)
        if (tags != null && tags.Length > 0)
        {
            var tagIds = await _db.MetaInfoTags
                .Where(mt => mt.TagName.Any(t => t.Contains(query, StringComparison.OrdinalIgnoreCase)))
                .Select(mt => mt.Id)
                .ToListAsync();

            if (tagIds.Any())
                query = query.Where(mi => mi.Tags?.Intersect(tagIds).Any() == true);
        }

        // Get total count for pagination
        var total = await query.CountAsync();

        // Order: exact matches first, then alphabetical
        var results = await query
            .OrderByDescending(mi => mi.Title.Contains(query))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return ApiResponseDto<ListResponseDto<SearchMetaInfosDto>>.Success(
            new ListResponseDto<SearchMetaInfosDto> { Items = CreateListResponseDto(results), TotalCount = total });
    }
}
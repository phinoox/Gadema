// =============================================================================
using Gadema.Core.Dtos;
using Microsoft.EntityFrameworkCore;
// Gadema.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Gadema.Core.Dtos.ContentItems;
using Gadema.Core.Dtos.Search;
using Gadema.Core.Dtos.Tags;
using Gadema.Core.Models;
using Gadema.Data.Database;
using Microsoft.Extensions.Logging;
using Gadema.Core.Services;

namespace Gadema.Api.Services;

/// <summary>
/// Implementation of tag service.
/// </summary>
public class TagService : IGademaService,  ITagService
{
    private readonly GameDbContext _context;
    private readonly ILogger<TagService> _logger;

    public ServiceTypeEnum ServiceType => ServiceTypeEnum.TagService;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public TagService(GameDbContext context, ILogger<TagService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// List tags for content item.
    /// </summary>
    public async Task<ApiResponseDto<TagListResponseDto>> GetTagsAsync(Guid contentItemId)
    {
        var junctionRecords = await _context.ContentTags
            .Where(ct => ct.ContentItemId == contentItemId && ct.Tag.IsActive)
            .Select(ct => new { ct.Id, TagId = ct.TagId })
            .ToListAsync();

        return ApiResponseDto<TagListResponseDto>.Success(new TagListResponseDto());
    }

    /// <summary>
    /// Add tags to content item.
    /// </summary>
    public async Task<ApiResponseDto<TagListResponseDto>> AddTagsAsync(Guid contentItemId, AddTagsDto addDto)
    {
        var now = DateTime.UtcNow;

        foreach (var tagId in addDto.TagIds)
        {
            var junctionRecord = new ContentTags
            {
                Id = Guid.NewGuid(),
                ContentItemId = contentItemId,
                TagId = tagId,
                OrderIndex = null!,
                CreatedAt = now
            };

            _context.ContentTags.Add(junctionRecord);
        }

        await _context.SaveChangesAsync();

        return ApiResponseDto<TagListResponseDto>.Success(new TagListResponseDto());
    }
}

/// <summary>
/// Implementation of search service.
/// </summary>
public class SearchService : IGademaService,  ISearchService
{
    private readonly GameDbContext _context;
    private readonly ILogger<SearchService> _logger;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public SearchService(GameDbContext context, ILogger<SearchService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public ServiceTypeEnum ServiceType => ServiceTypeEnum.SearchService;

    /// <summary>
    /// Search content items.
    /// </summary>
    public async Task<ApiResponseDto<PaginationResponse<ContentItemResponseDto>>> SearchContentItemsAsync(SearchContentItemsDto searchDto)
    {
        var query = _context.ContentItems.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchDto.Query))
        {
            var searchQuery = $"*%{searchDto.Query}%*";
            query = query.Where(c => c.Title.Contains(searchDto.Query) || 
                                      (c.Description != null && c.Description.Contains(searchDto.Query) == true )||
                                      c.Slug.Contains(searchDto.Query));
        }

        if (searchDto.ContentType.HasValue)
        {
            query = query.Where(c => c.ContentType == searchDto.ContentType.Value);
        }

        if (searchDto.PublishedOnly)
        {
            query = query.Where(c => c.Published);
        }

        var contentItems = await query.OrderBy(c => c.Title).Skip(0).Take(20)
            .Select(c => new ContentItemResponseDto
            {
                Id = c.Id,
                ProjectId = c.ProjectId,
                ContentType = (int)c.ContentType,
                Title = c.Title,
                Slug = c.Slug,
                ShortDesc = c.ShortDesc,
                Description = null,
                Published = c.Published,
                Status = c.Status,
                ViewMode = c.ViewMode.ToString(),
                Version = c.Version
            }).ToListAsync();

        return ApiResponseDto<PaginationResponse<ContentItemResponseDto>>.Success(new PaginationResponse<ContentItemResponseDto>());
    }
}
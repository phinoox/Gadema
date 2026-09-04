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
using Gadema.Core.Models.Content; // NEW: Import new tag models
using Gadema.Data.Database;
using Microsoft.Extensions.Logging;
using Gadema.Core.Services;

namespace Gadema.Api.Services;

/// <summary>
/// Implementation of tag service for ContentItems.
/// </summary>
public class TagService : IGademaService, ITagService
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
    /// List tags for a specific content item.
    /// </summary>
    public async Task<ApiResponseDto<TagListResponseDto>> GetTagsAsync(Guid contentItemId)
    {
        // Query the new junction table: ContentItemTag
        var contentItemTags = await _context.ContentItemTags
            .Where(cit => cit.ContentItemId == contentItemId)
            .Include(cit => cit.ContentTag) // Include the actual ContentTag details
            .ToListAsync();

        // Map to DTO
        var tags = contentItemTags.Select(cit => new TagResponseDto // Assuming TagDto exists in Gadema.Core.Dtos.Tags
        {
            Id = cit.ContentTag.Id,
            Name = cit.ContentTag.Name,
            Slug = cit.ContentTag.Slug,
            ColorHex = cit.ContentTag.ColorHex
        }).ToList();

        return ApiResponseDto<TagListResponseDto>.Success(new TagListResponseDto 
        { 
            Tags = tags // Adjust based on your actual DTO structure
        });
    }

    /// <summary>
    /// Add tags to a content item.
    /// </summary>
    public async Task<ApiResponseDto<TagListResponseDto>> AddTagsAsync(Guid contentItemId, AddTagsDto addDto)
    {
        // Verify the content item exists
        var contentItem = await _context.ContentItems.FindAsync(contentItemId);
        if (contentItem == null)
        {
            return ApiResponseDto<TagListResponseDto>.NotFound("Content item not found.");
        }

        var now = DateTime.UtcNow;
        var addedTags = new List<TagResponseDto>();

        foreach (var tagId in addDto.TagIds)
        {
            // Ensure the tag exists and is a ContentTag (not a ProjectTag)
            var contentTag = await _context.ContentTags.FindAsync(tagId);
            if (contentTag == null)
            {
                _logger.LogWarning("Attempted to add non-existent ContentTag {TagId} to ContentItem {ContentItemId}", tagId, contentItemId);
                continue;
            }

            // Check for duplicates
            var exists = await _context.ContentItemTags
                .AnyAsync(cit => cit.ContentItemId == contentItemId && cit.ContentTagId == tagId);

            if (!exists)
            {
                var junction = new ContentItemTag // Use the new junction entity
                {
                    ContentItemId = contentItemId,
                    ContentTagId = tagId,
                    CreatedAt = now
                };

                _context.ContentItemTags.Add(junction);
                
                addedTags.Add(new TagResponseDto 
                { 
                    Id = contentTag.Id, 
                    Name = contentTag.Name 
                });
            }
        }

        await _context.SaveChangesAsync();

        // Return the list of tags actually added (or all tags for that item, depending on your UI needs)
        return ApiResponseDto<TagListResponseDto>.Success(new TagListResponseDto 
        { 
            Tags = addedTags 
        });
    }
}

/// <summary>
/// Implementation of search service.
/// </summary>
public class SearchService : IGademaService, ISearchService
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
// =============================================================================
using Gadema.Core.Dtos;
using Microsoft.EntityFrameworkCore;
// Gadema.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Gadema.Core.Dtos.MetaInfos;
using Gadema.Core.Dtos.Search;
using Gadema.Core.Dtos.Tags;
using Gadema.Core.Models;
using Gadema.Core.Models.Content; // NEW: Import new tag models
using Gadema.Data.Database;
using Microsoft.Extensions.Logging;
using Gadema.Core.Services;

namespace Gadema.Api.Services;

/// <summary>
/// Implementation of tag service for MetaInfos.
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
    public async Task<ApiResponseDto<TagListResponseDto>> GetTagsAsync(Guid MetaInfoId)
    {
        // Query the new junction table: MetaInfoTagRelation
        var MetaInfoTagRelations = await _context.MetaInfoTagRelations
            .Where(cit => cit.MetaInfoId == MetaInfoId)
            .Include(cit => cit.MetaInfoTag) // Include the actual MetaInfoTag details
            .ToListAsync();

        // Map to DTO
        var tags = MetaInfoTagRelations.Select(cit => new TagResponseDto // Assuming TagDto exists in Gadema.Core.Dtos.Tags
        {
            Id = cit.MetaInfoTag.Id,
            Name = cit.MetaInfoTag.Name,
            Slug = cit.MetaInfoTag.Slug,
            ColorHex = cit.MetaInfoTag.ColorHex
        }).ToList();

        return ApiResponseDto<TagListResponseDto>.Success(new TagListResponseDto 
        { 
            Tags = tags // Adjust based on your actual DTO structure
        });
    }

    /// <summary>
    /// Add tags to a content item.
    /// </summary>
    public async Task<ApiResponseDto<TagListResponseDto>> AddTagsAsync(Guid MetaInfoId, AddTagsDto addDto)
    {
        // Verify the content item exists
        var MetaInfo = await _context.MetaInfos.FindAsync(MetaInfoId);
        if (MetaInfo == null)
        {
            return ApiResponseDto<TagListResponseDto>.NotFound("Content item not found.");
        }

        var now = DateTime.UtcNow;
        var addedTags = new List<TagResponseDto>();

        foreach (var tagId in addDto.TagIds)
        {
            // Ensure the tag exists and is a MetaInfoTag (not a ProjectTag)
            var MetaInfoTag = await _context.MetaInfoTags.FindAsync(tagId);
            if (MetaInfoTag == null)
            {
                _logger.LogWarning("Attempted to add non-existent MetaInfoTag {TagId} to MetaInfo {MetaInfoId}", tagId, MetaInfoId);
                continue;
            }

            // Check for duplicates
            var exists = await _context.MetaInfoTagRelations
                .AnyAsync(cit => cit.MetaInfoId == MetaInfoId && cit.MetaInfoTagId == tagId);

            if (!exists)
            {
                var junction = new MetaInfoTagRelation // Use the new junction entity
                {
                    MetaInfoId = MetaInfoId,
                    MetaInfoTagId = tagId,
                    CreatedAt = now
                };

                _context.MetaInfoTagRelations.Add(junction);
                
                addedTags.Add(new TagResponseDto 
                { 
                    Id = MetaInfoTag.Id, 
                    Name = MetaInfoTag.Name 
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
    public async Task<ApiResponseDto<PaginationResponse<MetaInfoResponseDto>>> SearchMetaInfosAsync(SearchMetaInfosDto searchDto)
    {
        var query = _context.MetaInfos.AsQueryable();

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

        var MetaInfos = await query.OrderBy(c => c.Title).Skip(0).Take(20)
            .Select(c => new MetaInfoResponseDto
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

        return ApiResponseDto<PaginationResponse<MetaInfoResponseDto>>.Success(new PaginationResponse<MetaInfoResponseDto>());
    }
}
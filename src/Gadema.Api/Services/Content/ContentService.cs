// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Content;
using Gadema.Core.Dtos.Content.ExternalReferences;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Dtos.Search;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Content;

/// <summary>
/// Service for content search and external reference management.
/// Handles full-text search across all MetaInfo entities and reference tracking.
/// </summary>
public class ContentService : CoreService
{
    public ContentService(GameDbContext db, ILogger<ContentService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    /// <summary>Creates a search result response DTO.</summary>
    private SearchMetaInfosDto CreateSearchResponseDto(MetaInfo metaInfo)
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
    // POST /api/v1/content/search - Full-text search across all content
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<SearchMetaInfosDto>>> SearchAsync(string query)
    {
        var results = await _db.MetaInfos
            .Where(mi => mi.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        mi.ShortDesc?.Contains(query, StringComparison.OrdinalIgnoreCase) == true ||
                        mi.Slug.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Select(mi => CreateSearchResponseDto(mi))
            .ToListAsync();

        return ApiResponseDto<IEnumerable<SearchMetaInfosDto>>.Success(results);
    }

    // ========================================================================
    // POST /api/v1/content/external-references - List external references for a project
    // ========================================================================

    public async Task<ApiResponseDto<ListResponseDto<ExternalReferenceCreateDto>>> GetReferencesAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<ListResponseDto<ExternalReferenceCreateDto>>(projectId);
        if (error != null) return error;

        var references = await _db.ExternalReferences
            .Include(er => er.MetaInfo)
            .Where(er => er.MetaInfo.ProjectId == projectId)
            .OrderBy(er => er.MetaInfo.Title)
            .Select(er => new ExternalReferenceCreateDto
            {
                Id = er.Id,
                MetaInfoId = er.MetaInfoId,
                MetaInfoTitle = er.MetaInfo.Title,
                ReferenceType = (int)er.ReferenceType,
                Url = er.Url,
                Author = er.Author,
                Title = er.Title,
                Notes = er.Notes,
            })
            .ToListAsync();

        return ApiResponseDto<ListResponseDto<ExternalReferenceCreateDto>>.Success(new ListResponseDto<ExternalReferenceCreateDto>
        {
            Items = references,
            TotalCount = references.Count
        });
    }

    // ========================================================================
    // POST /api/v1/content/external-references - Create a new external reference
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateReferenceAsync(Guid projectId, ExternalReferenceCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        var metaInfo = CreateMetaInfo(projectId, ContentTypeEnum.ExternalReference, createDto.CreateData);

        _db.MetaInfos.Add(metaInfo);
        await _db.SaveChangesAsync();

        var reference = new ExternalReference
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id,
            ReferenceType = (ExternalReferenceTypeEnum)createDto.ReferenceType,
            Url = createDto.Url,
            Author = createDto.Author,
            Title = createDto.Title,
            Notes = createDto.Notes,
        };

        _db.ExternalReferences.Add(reference);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = reference.Id,
            MetaInfoId = metaInfo.Id,
            ProjectId = projectId
        });
    }
}
using Gadema.Api.Services.Search;
using Gadema.Api.Services.Tags.Strategies;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.ExternalReferences;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Dtos.Search;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Enums;
using Gadema.Core.Models.Base.Infrastructure;
using Gadema.Core.Models.Base.Infrastructure.Enums;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Content;

/// <summary>
/// Service for managing External References (sources, citations, research materials).
/// Tracks URLs, authors, titles, and notes with optional ContentMetaInfo wrapper.
/// </summary>
[ServiceLifetime(ServiceLifetime.Scoped)] public class ExternalReferenceService : CoreService, ISearchableProvider
{
    public ExternalReferenceService(GameDbContext db, ILogger<ExternalReferenceService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // GET - List all references for a project
    // ========================================================================

    public async Task<ApiResponseDto<ListResponseDto<ExternalReferenceResponseDto>>> GetReferencesAsync(
        Guid projectId,
        int? referenceType = null,
        string? authorKeyword = null)
    {
        var error = await ValidateProjectAccessAsync<ListResponseDto<ExternalReferenceResponseDto>>(projectId);
        if (error != null) return error;

        var query = _db.ExternalReferences
            .Include(er => er.ContentMetaInfo)
            .Where(er => er.ContentMetaInfo.ProjectId == projectId)
            .AsQueryable();

        if (referenceType.HasValue)
            query = query.Where(er => er.ReferenceType == (ExternalReferenceTypeEnum)referenceType.Value);

        if (!string.IsNullOrWhiteSpace(authorKeyword))
            query = query.Where(er => er.Author != null && er.Author.Contains(authorKeyword, StringComparison.OrdinalIgnoreCase));

        var references = await query.OrderByDescending(r => r.Id).ToListAsync();

        return ApiResponseDto<ListResponseDto<ExternalReferenceResponseDto>>.Success(new ListResponseDto<ExternalReferenceResponseDto>
        {
            Items = references.Select(CreateResponseDto),
            TotalCount = references.Count
        });
    }

    // ========================================================================
    // GET - Single reference by ID
    // ========================================================================

    public async Task<ApiResponseDto<ExternalReferenceResponseDto>> GetReferenceByIdAsync(Guid id)
    {
        var reference = await _db.ExternalReferences
            .Include(er => er.ContentMetaInfo)
            .FirstOrDefaultAsync(er => er.Id == id);

        if (reference is null) 
            return ApiResponseDto<ExternalReferenceResponseDto>.NotFound($"External reference with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<ExternalReferenceResponseDto>(reference.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<ExternalReferenceResponseDto>.Success(CreateResponseDto(reference));
    }

    // ========================================================================
    // POST - Create a new reference
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateReferenceAsync(
        Guid projectId,
        ExternalReferenceCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        // 1. Create the MetaInfo anchor using the generic helper
        var meta = CreateMetaInfo<ContentMetaInfo>(createDto.ContentMetaInfo, m => {
            m.ProjectId = projectId;
            m.ContentType = ContentTypeEnum.ExternalReference;
        });

        // 2. Create the ExternalReference component
        var reference = new ExternalReference
        {
            Id = meta.Id, // Link identity to the anchor
            MetaInfoId = meta.Id,
            ReferenceType = createDto.ReferenceType,
            Url = createDto.Url,
            Author = createDto.Author,
            Title = createDto.Title,
            Notes = createDto.Notes,
            IsActive = true
        };

        _db.ExternalReferences.Add(reference);
        _db.Set<ContentMetaInfo>().Add(meta);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto 
        { 
            EntityId = reference.Id, 
            MetaInfoId = meta.Id, 
            ProjectId = projectId 
        });
    }

    // ========================================================================
    // PUT - Partial update of identity and domain data
    // ========================================================================

    public async Task<ApiResponseDto<ExternalReferenceResponseDto>> UpdateReferenceAsync(Guid id, ExternalReferenceUpdateDto updateDto)
    {
        var reference = await _db.ExternalReferences
            .Include(er => er.ContentMetaInfo)
            .FirstOrDefaultAsync(er => er.Id == id);

        if (reference is null)
            return ApiResponseDto<ExternalReferenceResponseDto>.NotFound($"External reference with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<ExternalReferenceUpdateDto>(reference.ContentMetaInfo.ProjectId);
        if (error != null) return ApiResponseDto<ExternalReferenceResponseDto>.Unauthorized("not authorized");

        // 1. Delegate Identity Updates to the Strategy
        if (updateDto.ContentMetaInfo != null)
        {
            await ApplyIdentitySyncAsync(reference.MetaInfoId, updateDto.ContentMetaInfo, new ContentIdentityStrategy(_db));
        }

        // 2. Update Domain Properties
        if (updateDto.ReferenceType.HasValue) reference.ReferenceType = updateDto.ReferenceType.Value;
        if (!string.IsNullOrWhiteSpace(updateDto.Url)) reference.Url = updateDto.Url;
        if (!string.IsNullOrWhiteSpace(updateDto.Author)) reference.Author = updateDto.Author;
        if (!string.IsNullOrWhiteSpace(updateDto.Title)) reference.Title = updateDto.Title;
        if (updateDto.Notes != null) reference.Notes = updateDto.Notes;
        if (updateDto.IsActive.HasValue) reference.IsActive = updateDto.IsActive.Value;

        await _db.SaveChangesAsync();
        return ApiResponseDto<ExternalReferenceResponseDto>.Success(CreateResponseDto(reference));
    }

    // ========================================================================
    // DELETE - Remove a reference
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteReferenceAsync(Guid id)
    {
        var reference = await _db.ExternalReferences
            .Include(er => er.ContentMetaInfo)
            .FirstOrDefaultAsync(er => er.Id == id);

        if (reference is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"External reference with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(reference.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        // Deleting the MetaInfo anchor will cascade to the ExternalReference
        _db.Set<ContentMetaInfo>().Remove(reference.ContentMetaInfo);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto 
        { 
            EntityId = id, 
            ProjectId = reference.ContentMetaInfo.ProjectId 
        });
    }

    // ========================================================================
    // ISearchableProvider Implementation
    // ========================================================================

    public async Task<IEnumerable<SearchHitDto>> GetMatchesAsync(string query, Guid? projectId)
    {
        var queryable = _db.ExternalReferences
            .Include(er => er.ContentMetaInfo)
            .AsQueryable();

        if (projectId.HasValue)
            queryable = queryable.Where(er => er.ContentMetaInfo.ProjectId == projectId.Value);

        return await queryable
            .Where(er => er.ContentMetaInfo.Title.Contains(query) || 
                        er.Title.Contains(query) || 
                        er.Author != null && er.Author.Contains(query))
            .Select(er => new SearchHitDto
            {
                ResourceId = er.Id,
                DisplayName = er.ContentMetaInfo.Title,
                Slug = er.ContentMetaInfo.Slug,
                ResourceType = "ExternalReference",
                ScopeId = er.ContentMetaInfo.ProjectId,
                ResourceLink = $"/api/v1/content/external-references/{er.Id}"
            })
            .ToListAsync();
    }

    private ExternalReferenceResponseDto CreateResponseDto(ExternalReference reference)
    {
        return new ExternalReferenceResponseDto
        {
            Id = reference.Id,
            MetaInfoId = reference.MetaInfoId,
            ReferenceType = reference.ReferenceType,
            Url = reference.Url,
            Author = reference.Author,
            Title = reference.Title,
            Notes = reference.Notes,
            IsActive = reference.IsActive,
            CreatedAt = reference.ContentMetaInfo.CreatedAt // Pulling from anchor
        };
    }
    
}
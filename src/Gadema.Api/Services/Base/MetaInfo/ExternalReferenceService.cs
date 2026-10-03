using Gadema.Api.CoreServices;
using Gadema.Api.CoreServices.Strategies;
using Gadema.Api.Services.Search;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.ExternalReferences;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Dtos.Search;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Enums;
using Gadema.Core.Models.Base.Infrastructure;
using Gadema.Core.Models.Base.Infrastructure.Enums;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Data.Database.Core;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Base.MetaInfo;

[ServiceLifetime(ServiceLifetime.Scoped)] public class ExternalReferenceService : DomainService, ISearchableProvider
{
    private CoreDbContext _db;

    public ExternalReferenceService( CoreDbContext db,
        ILogger<ExternalReferenceService> logger,  
        ICoreServicesProvider coreServices) // Injected via CoreService constructor
        : base(coreServices,logger)
    {
        _db = db;
    }

    public async Task<ApiResponseDto<ListResponseDto<ExternalReferenceResponseDto>>> GetReferencesAsync(
        Guid projectId,
        int? referenceType = null,
        string? authorKeyword = null)
    {
        var error = await CheckAccessAsync<ListResponseDto<ExternalReferenceResponseDto>>(projectId, Permission.CanView);
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

    public async Task<ApiResponseDto<ExternalReferenceResponseDto>> GetReferenceByIdAsync(Guid id)
    {
        var reference = await _db.ExternalReferences
            .Include(er => er.ContentMetaInfo)
            .FirstOrDefaultAsync(er => er.Id == id);

        if (reference is null) 
            return ApiResponseDto<ExternalReferenceResponseDto>.NotFound($"External reference with ID {id} not found.");

        var error = await CheckAccessAsync<ExternalReferenceResponseDto>(reference.ContentMetaInfo.ProjectId.Value, Permission.CanView);
        if (error != null) return error;

        return ApiResponseDto<ExternalReferenceResponseDto>.Success(CreateResponseDto(reference));
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateReferenceAsync(
        Guid projectId,
        ExternalReferenceCreateDto createDto)
    {
        var error = await CheckAccessAsync<CreateResponseDto>(projectId, Permission.CanEdit);
        if (error != null) return error;

        var meta = await _core.MetadataService.CreateAsync<ContentMetaInfo>(createDto.ContentMetaInfo, m =>
        {
            m.ProjectId = projectId;
            m.ContentType = ContentTypeEnum.ExternalReference;
        });

        var reference = new ExternalReference
        {
            Id = meta.Id, 
            MetaInfoId = meta.Id,
            ReferenceType = createDto.ReferenceType,
            Url = createDto.Url,
            Author = createDto.Author,
            Title = createDto.Title,
            Notes = createDto.Notes,
            IsActive = true
        };

        _db.ExternalReferences.Add(reference);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto 
        { 
            EntityId = reference.Id, 
            MetaInfoId = meta.Id, 
            ProjectId = projectId 
        });
    }

    public async Task<ApiResponseDto<ExternalReferenceResponseDto>> UpdateReferenceAsync(Guid id, ExternalReferenceUpdateDto updateDto)
    {
        var reference = await _db.ExternalReferences
            .Include(er => er.ContentMetaInfo)
            .FirstOrDefaultAsync(er => er.Id == id);

        if (reference is null)
            return ApiResponseDto<ExternalReferenceResponseDto>.NotFound($"External reference with ID {id} not found.");

        var error = await CheckAccessAsync<ExternalReferenceResponseDto>(reference.ContentMetaInfo.ProjectId.Value, Permission.CanEdit);
        if (error != null) return error;

        if (updateDto.ContentMetaInfo != null)
        {
            var success = await SyncIdentityAsync<ContentIdentityStrategy>(reference.MetaInfoId, updateDto.ContentMetaInfo);
            if (!success) return ApiResponseDto<ExternalReferenceResponseDto>.ServerError("Sync failed.");
        }

        if (updateDto.ReferenceType.HasValue) reference.ReferenceType = updateDto.ReferenceType.Value;
        if (!string.IsNullOrWhiteSpace(updateDto.Url)) reference.Url = updateDto.Url;
        if (!string.IsNullOrWhiteSpace(updateDto.Author)) reference.Author = updateDto.Author;
        if (!string.IsNullOrWhiteSpace(updateDto.Title)) reference.Title = updateDto.Title;
        if (updateDto.Notes != null) reference.Notes = updateDto.Notes;
        if (updateDto.IsActive.HasValue) reference.IsActive = updateDto.IsActive.Value;

        await _db.SaveChangesAsync();
        return ApiResponseDto<ExternalReferenceResponseDto>.Success(CreateResponseDto(reference));
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteReferenceAsync(Guid id)
    {
        var reference = await _db.ExternalReferences
            .Include(er => er.ContentMetaInfo)
            .FirstOrDefaultAsync(er => er.Id == id);

        if (reference is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"External reference with ID {id} not found.");

        var error = await CheckAccessAsync<DeleteResponseDto>(reference.ContentMetaInfo.ProjectId.Value, Permission.CanDelete);
        if (error != null) return error;

        _db.Set<ContentMetaInfo>().Remove(reference.ContentMetaInfo);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto 
        { 
            EntityId = id, 
            ProjectId = reference.ContentMetaInfo.ProjectId.Value
        });
    }

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
            CreatedAt = reference.ContentMetaInfo.CreatedAt 
        };
    }
}
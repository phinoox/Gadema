// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Content.ExternalReferences;
using Gadema.Core.Dtos.ExternalReferences;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Content;

/// <summary>
/// Service for managing External References (sources, citations, research materials).
/// Tracks URLs, authors, titles, and notes with optional MetaInfo wrapper.
/// </summary>
public class ExternalReferenceService : CoreService
{
    public ExternalReferenceService(GameDbContext db, ILogger<ExternalReferenceService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    private ReferenceResponseDto CreateResponseDto(ExternalReference reference)
        => new()
        {
            Id = reference.Id,
            MetaInfoId = reference.MetaInfoId,
            ReferenceType = (int)reference.ReferenceType,
            Url = reference.Url,
            Author = reference.Author,
            Title = reference.Title,
            Notes = reference.Notes,
            CreatedAt = reference.CreatedAt,
        };

    private ListResponseDto<ReferenceResponseDto> CreateListResponseDto(IEnumerable<ExternalReference> references)
        => new() { Items = references.Select(CreateResponseDto).ToList(), TotalCount = references.Count() };

    // ========================================================================
    // GET /api/v1/content/external-references - List with filters
    // ========================================================================

    public async Task<ApiResponseDto<ListResponseDto<ReferenceResponseDto>>> GetReferencesAsync(
        Guid projectId,
        int? referenceType = null,
        string? authorKeyword = null)
    {
        var error = await ValidateProjectAccessAsync<ListResponseDto<ReferenceResponseDto>>(projectId);
        if (error != null) return error;

        IQueryable<ExternalReference> query = _db.ExternalReferences
            .Include(er => er.MetaInfo)
            .Where(er => er.MetaInfo.ProjectId == projectId)
            .OrderByDescending(er => er.CreatedAt);

        if (referenceType.HasValue)
            query = query.Where(er => er.ReferenceType == referenceType.Value);

        if (!string.IsNullOrWhiteSpace(authorKeyword))
            query = query.Where(er => !string.IsNullOrEmpty(er.Author) &&
                                      er.Author.Contains(authorKeyword, StringComparison.OrdinalIgnoreCase));

        var references = await query.ToListAsync();
        return ApiResponseDto<ListResponseDto<ReferenceResponseDto>>.Success(CreateListResponseDto(references));
    }

    // ========================================================================
    // GET /api/v1/content/external-references/{id} - Single reference
    // ========================================================================

    public async Task<ApiResponseDto<ReferenceResponseDto>> GetReferenceByIdAsync(Guid id)
    {
        var reference = await _db.ExternalReferences.Include(er => er.MetaInfo).FirstOrDefaultAsync(er => er.Id == id);
        if (reference is null) return ApiResponseDto<ReferenceResponseDto>.NotFound($"External reference with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<ReferenceResponseDto>(reference.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<ReferenceResponseDto>.Success(CreateResponseDto(reference));
    }

    // ========================================================================
    // POST /api/v1/content/external-references - Create a new reference
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateReferenceAsync(
        Guid projectId,
        ExternalReferenceCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        var metaInfo = CreateMetaInfo(projectId, ContentTypeEnum.ExternalReference, createDto.CreateData);

        _db.MetaInfos.Add(metaInfo);
        await _db.SaveChangesAsync();

        var reference = new ExternalReference
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id.Value,
            ReferenceType = createDto.ReferenceType ?? 0,
            Url = createDto.Url,
            Author = createDto.Author,
            Title = createDto.Title,
            Notes = createDto.Notes,
        };

        _db.ExternalReferences.Add(reference);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto { EntityId = reference.Id, MetaInfoId = metaInfo.Id.Value, ProjectId = projectId });
    }

    // ========================================================================
    // PUT /api/v1/content/external-references/{id} - Partial update
    // ========================================================================

    public async Task<ApiResponseDto<ReferenceResponseDto>> UpdateReferenceAsync(Guid id, ExternalReferenceUpdateDto updateDto)
    {
        var reference = await _db.ExternalReferences.Include(er => er.MetaInfo).FirstOrDefaultAsync(er => er.Id == id);
        if (reference is null) return ApiResponseDto<ReferenceResponseDto>.NotFound($"External reference with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<ExternalReferenceUpdateDto>(reference.MetaInfo.ProjectId);
        if (error != null) return error;

        ApplyMetaInfoUpdates(reference.MetaInfo, updateDto.MetaInfo);

        if (updateDto.ReferenceType.HasValue) reference.ReferenceType = (ExternalReferenceTypeEnum)updateDto.ReferenceType.Value;
        if (!string.IsNullOrWhiteSpace(updateDto.Url)) reference.Url = updateDto.Url;
        if (!string.IsNullOrWhiteSpace(updateDto.Author)) reference.Author = updateDto.Author;
        if (!string.IsNullOrWhiteSpace(updateDto.Title)) reference.Title = updateDto.Title;
        if (updateDto.Notes != null) reference.Notes = updateDto.Notes;

        await _db.SaveChangesAsync();
        return ApiResponseDto<ReferenceResponseDto>.Success(CreateResponseDto(reference));
    }

    // ========================================================================
    // DELETE /api/v1/content/external-references/{id} - Remove reference
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteReferenceAsync(Guid id)
    {
        var reference = await _db.ExternalReferences.Include(er => er.MetaInfo).FirstOrDefaultAsync(er => er.Id == id);
        if (reference is null) return ApiResponseDto<DeleteResponseDto>.NotFound($"External reference with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(reference.MetaInfo.ProjectId);
        if (error != null) return error;

        _db.ExternalReferences.Remove(reference);
        await _db.SaveChangesAsync();
        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto { EntityId = id, ProjectId = reference.MetaInfo.ProjectId });
    }
}
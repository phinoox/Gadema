using Gadema.Api.Services.Search;
using Gadema.Api.Services.Tags.Strategies;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Identity;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Dtos.Search;
using Gadema.Core.Models;
using Gadema.Core.Models.Base.MetaInfo;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Access.Identity;

/// <summary>
/// Service for managing IdentityValues - specific options within a definition (e.g., "Human" in Race).
/// </summary>
public class IdentityValueService : CoreService, ISearchableProvider
{
    public IdentityValueService(GameDbContext db, ILogger<IdentityValueService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // GET - List all values for a definition
    // ========================================================================

    public async Task<ApiResponseDto<ListResponseDto<IdentityValueResponseDto>>> GetValuesAsync(Guid definitionId)
    {
        var error = await ValidateProjectAccessAsync<ListResponseDto<IdentityValueResponseDto>>(await GetProjectIdForDefinition(definitionId));
        if (error != null) return error;

        var values = await _db.IdentityValues
            .Include(v => v.ContentMetaInfo)
            .Where(v => v.IdentityDefinitionId == definitionId)
            .OrderBy(v => v.OrderIndex)
            .ToListAsync();

        return ApiResponseDto<ListResponseDto<IdentityValueResponseDto>>.Success(new ListResponseDto<IdentityValueResponseDto>
        {
            Items = values.Select(CreateResponseDto),
            TotalCount = values.Count
        });
    }

    // ========================================================================
    // GET - Single value by ID
    // ========================================================================

    public async Task<ApiResponseDto<IdentityValueResponseDto>> GetValueAsync(Guid id)
    {
        var value = await _db.IdentityValues
            .Include(v => v.ContentMetaInfo)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (value is null) 
            return ApiResponseDto<IdentityValueResponseDto>.NotFound($"Identity value with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<IdentityValueResponseDto>(value.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<IdentityValueResponseDto>.Success(CreateResponseDto(value));
    }

    // ========================================================================
    // POST - Create a new value
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateValueAsync(Guid definitionId, IdentityValueCreateDto createDto)
    {
        var def = await _db.IdentityDefinitions.FindAsync(definitionId);
        if (def == null) return ApiResponseDto<CreateResponseDto>.NotFound($"Definition with ID {definitionId} not found.");

        var error = await ValidateProjectAccessAsync<CreateResponseDto>(def.ProjectId);
        if (error != null) return error;

        // 1. Create the MetaInfo anchor using the generic helper
        var meta = CreateMetaInfo<ContentMetaInfo>(createDto.ContentMetaInfo, m => {
            m.ProjectId = def.ProjectId; // Link to project via parent definition
        });

        // 2. Create the IdentityValue component
        var identityValue = new IdentityValue
        {
            Id = meta.Id,
            MetaInfoId = meta.Id,
            IdentityDefinitionId = definitionId,
            Name = createDto.Name,
            Description = createDto.Description,
            OrderIndex = createDto.OrderIndex,
            IsDefault = createDto.IsDefault
        };

        _db.IdentityValues.Add(identityValue);
        _db.Set<ContentMetaInfo>().Add(meta);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto 
        { 
            EntityId = identityValue.Id, 
            MetaInfoId = meta.Id, 
            ProjectId = def.ProjectId 
        });
    }

    // ========================================================================
    // PUT - Partial update of identity and domain data
    // ========================================================================

    public async Task<ApiResponseDto<IdentityValueResponseDto>> UpdateValueAsync(Guid id, IdentityValueUpdateDto updateDto)
    {
        var value = await _db.IdentityValues
            .Include(v => v.ContentMetaInfo)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (value is null)
            return ApiResponseDto<IdentityValueResponseDto>.NotFound($"Identity value with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<IdentityValueUpdateDto>(value.ContentMetaInfo.ProjectId);
        if ( error != null) return ApiResponseDto<IdentityValueResponseDto>.Unauthorized("not authorized");

        // 1. Delegate Identity Updates to the Strategy
        if (updateDto.ContentMetaInfo != null)
        {
            await ApplyIdentitySyncAsync(value.MetaInfoId, updateDto.ContentMetaInfo, new ContentIdentityStrategy(_db));
        }

        // 2. Update Domain Properties
        if (updateDto.Name != null) value.Name = updateDto.Name;
        if (updateDto.Description != null) value.Description = updateDto.Description;
        if (updateDto.OrderIndex.HasValue) value.OrderIndex = updateDto.OrderIndex.Value;
        if (updateDto.IsDefault.HasValue) value.IsDefault = updateDto.IsDefault.Value;

        await _db.SaveChangesAsync();
        return ApiResponseDto<IdentityValueResponseDto>.Success(CreateResponseDto(value));
    }

    // ========================================================================
    // DELETE - Remove a value
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteValueAsync(Guid id)
    {
        var value = await _db.IdentityValues
            .Include(v => v.ContentMetaInfo)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (value is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Identity value with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(value.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        // Deleting the MetaInfo anchor will cascade to the IdentityValue component
        _db.Set<ContentMetaInfo>().Remove(value.ContentMetaInfo);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto 
        { 
            EntityId = id, 
            ProjectId = value.ContentMetaInfo.ProjectId 
        });
    }

    // ========================================================================
    // ISearchableProvider Implementation
    // ========================================================================

    public async Task<IEnumerable<SearchHitDto>> GetMatchesAsync(string query, Guid? projectId)
    {
        var queryable = _db.IdentityValues
            .Include(v => v.ContentMetaInfo)
            .AsQueryable();

        if (projectId.HasValue)
            queryable = queryable.Where(v => v.ContentMetaInfo.ProjectId == projectId.Value);

        return await queryable
            .Where(v => v.ContentMetaInfo.Title.Contains(query) || 
                        v.Name.Contains(query))
            .Select(v => new SearchHitDto
            {
                ResourceId = v.Id,
                DisplayName = v.ContentMetaInfo.Title,
                Slug = v.ContentMetaInfo.Slug,
                ResourceType = "IdentityValue",
                ScopeId = v.ContentMetaInfo.ProjectId,
                ResourceLink = $"/api/v1/projects/{v.ContentMetaInfo.ProjectId}/identity-definitions/{v.IdentityDefinitionId}/values/{v.Id}"
            })
            .ToListAsync();
    }

    private IdentityValueResponseDto CreateResponseDto(IdentityValue value)
    {
        return new IdentityValueResponseDto
        {
            Id = value.Id,
            MetaInfoId = value.MetaInfoId,
            // Denormalized identity properties from the anchor
            Title = value.ContentMetaInfo.Title,
            Slug = value.ContentMetaInfo.Slug,
            IsPublic = value.ContentMetaInfo.IsPublic,
            CreatedAt = value.ContentMetaInfo.CreatedAt,
            // Domain properties
            Name = value.Name,
            Description = value.Description,
            OrderIndex = value.OrderIndex,
            IsDefault = value.IsDefault,
            IdentityDefinitionId = value.IdentityDefinitionId,
            
        };
    }

    private async Task<Guid> GetProjectIdForDefinition(Guid definitionId)
    {
        var def = await _db.IdentityDefinitions.FindAsync(definitionId);
        if (def == null) throw new Exception("Identity Definition not found.");
        return def.ProjectId;
    }
}
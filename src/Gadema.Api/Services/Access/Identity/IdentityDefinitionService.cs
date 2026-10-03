using Gadema.Api.CoreServices;
using Gadema.Api.CoreServices.Strategies;
using Gadema.Api.Services.Search;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Identity;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Dtos.Search;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Enums;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Core.Models.Identity;
using Gadema.Data.Database;
using Gadema.Data.Database.Core;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Access.Identity;

/// <summary>
/// Service for managing IdentityDefinitions - defines what attributes/properties
/// a character identity can have (e.g., "Race", "Faction").
/// </summary>
[ServiceLifetime(ServiceLifetime.Scoped)] public class IdentityDefinitionService : DomainService, ISearchableProvider
{
    private CoreDbContext _db;

    public IdentityDefinitionService( CoreDbContext db,
        ILogger<IdentityDefinitionService> logger,  
        ICoreServicesProvider coreServices) // Injected via CoreService constructor
        : base(coreServices,logger) { _db = db; }

    // ========================================================================
    // GET - List all definitions for a project
    // ========================================================================

    public async Task<ApiResponseDto<ListResponseDto<IdentityDefinitionResponseDto>>> GetDefinitionsAsync(Guid projectId)
    {
        var error = await CheckAccessAsync<ListResponseDto<IdentityDefinitionResponseDto>>(projectId, Permission.CanView);
        if (error != null) return error;

        var defs = await _db.IdentityDefinitions
            .Include(id => id.ContentMetaInfo)
            .Where(id => id.ProjectId == projectId)
            .OrderByDescending(id => id.ContentMetaInfo.CreatedAt)
            .ToListAsync();

        return ApiResponseDto<ListResponseDto<IdentityDefinitionResponseDto>>.Success(new ListResponseDto<IdentityDefinitionResponseDto>
        {
            Items = defs.Select(CreateResponseDto),
            TotalCount = defs.Count
        });
    }

    // ========================================================================
    // GET - Single definition by ID
    // ========================================================================

    public async Task<ApiResponseDto<IdentityDefinitionResponseDto>> GetDefinitionAsync(Guid id)
    {
        var def = await _db.IdentityDefinitions
            .Include(id => id.ContentMetaInfo)
            .FirstOrDefaultAsync(idef => idef.Id == id);

        if (def is null) 
            return ApiResponseDto<IdentityDefinitionResponseDto>.NotFound($"Identity definition with ID {id} not found.");

        var error = await CheckAccessAsync<IdentityDefinitionResponseDto>(def.ProjectId, Permission.CanView);
        if (error != null) return error;

        return ApiResponseDto<IdentityDefinitionResponseDto>.Success(CreateResponseDto(def));
    }

    // ========================================================================
    // POST - Create a new definition
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateDefinitionAsync(Guid projectId, IdentityDefinitionCreateDto createDto)
    {
        var error = await CheckAccessAsync<CreateResponseDto>(projectId, Permission.CanEdit);
        if (error != null) return error;

        // 1. Create the MetaInfo anchor using the generic helper
        var meta = await _core.MetadataService.CreateAsync<ContentMetaInfo>(createDto.ContentMetaInfo, m =>
        {
            m.ProjectId = projectId;
            m.ContentType = ContentTypeEnum.IdentityDefinition;
        });

        // 2. Create the IdentityDefinition component
        var def = new IdentityDefinition
        {
            Id = meta.Id,
            MetaInfoId = meta.Id,
            ProjectId = projectId,
            DataType = createDto.DataType,
            IsRequired = createDto.IsRequired,
            IsActive = true
        };

        _db.IdentityDefinitions.Add(def);
        _db.Set<ContentMetaInfo>().Add(meta);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto 
        { 
            EntityId = def.Id, 
            MetaInfoId = meta.Id, 
            ProjectId = projectId 
        });
    }

    // ========================================================================
    // PUT - Partial update of identity and domain data
    // ========================================================================

    public async Task<ApiResponseDto<IdentityDefinitionResponseDto>> UpdateDefinitionAsync(Guid id, IdentityDefinitionUpdateDto updateDto)
    {
        var def = await _db.IdentityDefinitions
            .Include(id => id.ContentMetaInfo)
            .FirstOrDefaultAsync(idef => idef.Id == id);

        if (def is null)
            return ApiResponseDto<IdentityDefinitionResponseDto>.NotFound($"Identity definition with ID {id} not found.");

        var error = await CheckAccessAsync<IdentityDefinitionResponseDto>(def.ProjectId, Permission.CanEdit);
        if (error != null) return error;

        // 1. Delegate Identity Updates to the Strategy
        if (updateDto.ContentMetaInfo != null)
        {
            var success = await SyncIdentityAsync<ContentIdentityStrategy>(def.MetaInfoId, updateDto.ContentMetaInfo);
            if (!success) return ApiResponseDto<IdentityDefinitionResponseDto>.ServerError("Sync failed.");
        }

        // 2. Update Domain Properties
        if (updateDto.Name != null) def.ContentMetaInfo.Title = updateDto.Name;
        if (updateDto.Description != null) def.ContentMetaInfo.ShortDesc = updateDto.Description;
        if (updateDto.DataType.HasValue) def.DataType = updateDto.DataType.Value;
        if (updateDto.IsRequired.HasValue) def.IsRequired = updateDto.IsRequired.Value;
        if (updateDto.IsActive.HasValue) def.IsActive = updateDto.IsActive.Value;

        await _db.SaveChangesAsync();
        return ApiResponseDto<IdentityDefinitionResponseDto>.Success(CreateResponseDto(def));
    }

    // ========================================================================
    // DELETE - Remove a definition
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteDefinitionAsync(Guid id)
    {
        var def = await _db.IdentityDefinitions
            .Include(id => id.ContentMetaInfo)
            .FirstOrDefaultAsync(idef => idef.Id == id);

        if (def is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Identity definition with ID {id} not found.");

        var error = await CheckAccessAsync<DeleteResponseDto>(def.ProjectId, Permission.CanDelete);
        if (error != null) return error;

        // Removing the MetaInfo anchor will cascade to the IdentityDefinition via the relationship
        _db.Set<ContentMetaInfo>().Remove(def.ContentMetaInfo);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto 
        { 
            EntityId = id, 
            ProjectId = def.ProjectId 
        });
    }

    // ========================================================================
    // ISearchableProvider Implementation
    // ========================================================================

    public async Task<IEnumerable<SearchHitDto>> GetMatchesAsync(string query, Guid? projectId)
    {
        var queryable = _db.IdentityDefinitions
            .Include(id => id.ContentMetaInfo)
            .AsQueryable();

        if (projectId.HasValue)
            queryable = queryable.Where(id => id.ProjectId == projectId.Value);

        return await queryable
            .Where(id => id.ContentMetaInfo.Title.Contains(query) || 
                         id.ContentMetaInfo.Title.Contains(query))
            .Select(id => new SearchHitDto
            {
                ResourceId = id.Id,
                DisplayName = id.ContentMetaInfo.Title,
                Slug = id.ContentMetaInfo.Slug,
                ResourceType = "IdentityDefinition",
                ScopeId = id.ProjectId,
                ResourceLink = $"/api/v1/projects/{id.ProjectId}/identity-definitions/{id.Id}"
            })
            .ToListAsync();
    }

    private IdentityDefinitionResponseDto CreateResponseDto(IdentityDefinition def)
    {
        return new IdentityDefinitionResponseDto
        {
            Id = def.Id,
            MetaInfoId = def.MetaInfoId,
            // Denormalized identity properties from the anchor
            Title = def.ContentMetaInfo.Title,
            Slug = def.ContentMetaInfo.Slug,
            IsPublic = def.ContentMetaInfo.IsPublic,
            CreatedAt = def.ContentMetaInfo.CreatedAt,
            // Domain properties
            DataType = def.DataType,
            Name = def.ContentMetaInfo.Title,
            Description = def.ContentMetaInfo.ShortDesc,
            IsRequired = def.IsRequired,
            ProjectId = def.ProjectId
        };
    }
}
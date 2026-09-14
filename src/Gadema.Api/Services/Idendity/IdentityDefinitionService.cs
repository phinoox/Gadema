// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Identity;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Identity;

/// <summary>
/// Service for managing IdentityDefinitions - defines what attributes/properties
/// a character identity can have (e.g., "Gender", "Age Group", "Alignment").
/// </summary>
public class IdentityDefinitionService : CoreService
{
    public IdentityDefinitionService(GameDbContext db, ILogger<IdentityDefinitionService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    private IdentityDefinitionResponseDto CreateResponseDto(IdentityDefinition def)
        => new()
        {
            Id = def.Id,
            MetaInfoId = def.MetaInfoId,
            Name = def.Name,
            Description = def.Description,
            DataType = (int)def.DataType,
            IsRequired = def.IsRequired,
            IsValidationRegex = def.ValidationRegex,
            CreatedAt = def.CreatedAt,
        };

    private ListResponseDto<IdentityDefinitionResponseDto> CreateListResponseDto(IEnumerable<IdentityDefinition> defs)
        => new() { Items = defs.Select(CreateResponseDto).ToList(), TotalCount = defs.Count() };

    public async Task<ApiResponseDto<ListResponseDto<IdentityDefinitionResponseDto>>> GetDefinitionsAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<ListResponseDto<IdentityDefinitionResponseDto>>(projectId);
        if (error != null) return error;

        var defs = await _db.IdentityDefinitions
            .Include(id => id.ContentMetaInfo)
            .Where(id => id.ContentMetaInfo.ProjectId == projectId)
            .OrderByDescending(id => id.CreatedAt)
            .Select(CreateResponseDto)
            .ToListAsync();

        return ApiResponseDto<ListResponseDto<IdentityDefinitionResponseDto>>.Success(CreateListResponseDto(defs));
    }

    public async Task<ApiResponseDto<IdentityDefinitionResponseDto>> GetDefinitionAsync(Guid id)
    {
        var def = await _db.IdentityDefinitions.Include(id => id.ContentMetaInfo).FirstOrDefaultAsync(id => id.Id == id);
        if (def is null) return ApiResponseDto<IdentityDefinitionResponseDto>.NotFound($"Identity definition with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<IdentityDefinitionResponseDto>(def.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<IdentityDefinitionResponseDto>.Success(CreateResponseDto(def));
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateDefinitionAsync(Guid projectId, IdentityDefinitionCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        var ContentMetaInfo = CreateMetaInfo(projectId, ContentTypeEnum.IdentityDefinition, createDto.CreateData);

        _db.MetaInfos.Add(ContentMetaInfo);
        await _db.SaveChangesAsync();

        var def = new IdentityDefinition
        {
            Id = Guid.NewGuid(),
            MetaInfoId = ContentMetaInfo.Id.Value,
            Name = createDto.Name,
            Description = createDto.Description,
            DataType = (IdentityDataTypeEnum)createDto.DataType,
            IsRequired = createDto.IsRequired ?? false,
            ValidationRegex = createDto.ValidationRegex,
        };

        _db.IdentityDefinitions.Add(def);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto { EntityId = def.Id, MetaInfoId = ContentMetaInfo.Id.Value, ProjectId = projectId });
    }

    public async Task<ApiResponseDto<IdentityDefinitionResponseDto>> UpdateDefinitionAsync(Guid id, IdentityDefinitionUpdateDto updateDto)
    {
        var def = await _db.IdentityDefinitions.Include(id => id.ContentMetaInfo).FirstOrDefaultAsync(id => id.Id == id);
        if (def is null) return ApiResponseDto<IdentityDefinitionResponseDto>.NotFound($"Identity definition with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<IdentityDefinitionUpdateDto>(def.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        ApplyMetaInfoUpdates(def.ContentMetaInfo, updateDto.ContentMetaInfo);

        if (!string.IsNullOrWhiteSpace(updateDto.Name)) def.Name = updateDto.Name;
        if (updateDto.Description != null) def.Description = updateDto.Description;
        if (updateDto.DataType.HasValue) def.DataType = (IdentityDataTypeEnum)updateDto.DataType.Value;
        if (updateDto.IsRequired.HasValue) def.IsRequired = updateDto.IsRequired.Value;
        if (!string.IsNullOrWhiteSpace(updateDto.ValidationRegex)) def.ValidationRegex = updateDto.ValidationRegex;

        await _db.SaveChangesAsync();
        return ApiResponseDto<IdentityDefinitionResponseDto>.Success(CreateResponseDto(def));
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteDefinitionAsync(Guid id)
    {
        var def = await _db.IdentityDefinitions.Include(id => id.ContentMetaInfo).FirstOrDefaultAsync(id => id.Id == id);
        if (def is null) return ApiResponseDto<DeleteResponseDto>.NotFound($"Identity definition with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(def.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        _db.MetaInfos.Remove(def.ContentMetaInfo);
        _db.IdentityDefinitions.Remove(def);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto { EntityId = id, ProjectId = def.ContentMetaInfo.ProjectId });
    }
}
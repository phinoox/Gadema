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
/// Service for managing IdentityValues - the actual values assigned to characters
/// for a given identity definition (e.g., "Male", "Female" for Gender).
/// </summary>
public class IdentityValueService : CoreService
{
    public IdentityValueService(GameDbContext db, ILogger<IdentityValueService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    private IdentityValueResponseDto CreateResponseDto(IdentityValue value)
        => new()
        {
            Id = value.Id,
            MetaInfoId = value.MetaInfoId,
            DefinitionId = value.DefinitionId,
            Value = value.Value,
            IsDefault = value.IsDefault,
            CreatedAt = value.CreatedAt,
        };

    private ListResponseDto<IdentityValueResponseDto> CreateListResponseDto(IEnumerable<IdentityValue> values)
        => new() { Items = values.Select(CreateResponseDto).ToList(), TotalCount = values.Count() };

    public async Task<ApiResponseDto<ListResponseDto<IdentityValueResponseDto>>> GetValuesAsync(Guid projectId, Guid? definitionId = null)
    {
        var error = await ValidateProjectAccessAsync<ListResponseDto<IdentityValueResponseDto>>(projectId);
        if (error != null) return error;

        IQueryable<IdentityValue> query = _db.IdentityValues.Where(iv => iv.ContentMetaInfo.ProjectId == projectId).OrderByDescending(iv => iv.CreatedAt);

        if (definitionId.HasValue) query = query.Where(iv => iv.DefinitionId == definitionId.Value);

        var values = await query.ToListAsync();
               return ApiResponseDto<ListResponseDto<IdentityValueResponseDto>>.Success(CreateListResponseDto(values));
    }

    public async Task<ApiResponseDto<IdentityValueResponseDto>> GetValuesByIdAsync(Guid id)
    {
        var value = await _db.IdentityValues.Include(iv => iv.Definition).FirstOrDefaultAsync(iv => iv.Id == id);
        if (value is null) return ApiResponseDto<IdentityValueResponseDto>.NotFound($"Identity value with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<IdentityValueResponseDto>(value.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<IdentityValueResponseDto>.Success(CreateResponseDto(value));
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateValueAsync(Guid projectId, IdentityValueCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        // Verify identity definition exists in the project
        var definition = await _db.IdentityDefinitions.FirstOrDefaultAsync(id => id.Id == createDto.DefinitionId && id.ContentMetaInfo.ProjectId == projectId);
        if (definition is null) return ApiResponseDto<CreateResponseDto>.BadRequest("Identity definition not found.");

        var ContentMetaInfo = CreateMetaInfo(projectId, ContentTypeEnum.IdentityValue, createDto.CreateData);

        _db.MetaInfos.Add(ContentMetaInfo);
        await _db.SaveChangesAsync();

        var value = new IdentityValue
        {
            Id = Guid.NewGuid(),
            MetaInfoId = ContentMetaInfo.Id.Value,
            DefinitionId = createDto.DefinitionId,
            Value = createDto.Value,
            IsDefault = createDto.IsDefault ?? false,
        };

        _db.IdentityValues.Add(value);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto { EntityId = value.Id, MetaInfoId = ContentMetaInfo.Id.Value, ProjectId = projectId });
    }

    public async Task<ApiResponseDto<IdentityValueResponseDto>> UpdateValueAsync(Guid id, IdentityValueUpdateDto updateDto)
    {
        var value = await _db.IdentityValues.Include(iv => iv.Definition).FirstOrDefaultAsync(iv => iv.Id == id);
        if (value is null) return ApiResponseDto<IdentityValueResponseDto>.NotFound($"Identity value with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<IdentityValueUpdateDto>(value.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        ApplyMetaInfoUpdates(value.ContentMetaInfo, updateDto.ContentMetaInfo);

        if (!string.IsNullOrWhiteSpace(updateDto.Value)) value.Value = updateDto.Value;
        if (updateDto.IsDefault.HasValue) value.IsDefault = updateDto.IsDefault.Value;

        await _db.SaveChangesAsync();
        return ApiResponseDto<IdentityValueResponseDto>.Success(CreateResponseDto(value));
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteValueAsync(Guid id)
    {
        var value = await _db.IdentityValues.Include(iv => iv.Definition).FirstOrDefaultAsync(iv => iv.Id == id);
        if (value is null) return ApiResponseDto<DeleteResponseDto>.NotFound($"Identity value with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(value.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        _db.IdentityValues.Remove(value);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto { EntityId = id, ProjectId = value.ContentMetaInfo.ProjectId });
    }
}
   
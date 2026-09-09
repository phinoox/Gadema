// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.WorldBuilding;
using Gadema.Core.Enums;

using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.WorldBuilding;

/// <summary>
/// Service for managing WorldLocations within the world-building domain.
/// Handles CRUD operations including MetaInfo creation and authorization.
/// </summary>
public class WorldLocationService : CoreService
{
    public WorldLocationService(GameDbContext db, ILogger<WorldLocationService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // GET - List all world locations for a project
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<WorldLocationResponseDto>>> GetWorldLocationsAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<WorldLocationResponseDto>>(projectId);
        if (error != null) return error;

        var locations = await _db.WorldLocations
            .Include(wl => wl.MetaInfo)
            .Where(wl => wl.MetaInfo.ProjectId == projectId)
            .OrderBy(wl => wl.MetaInfo.Title)
            .Select(wl => new WorldLocationResponseDto
            {
                Id = wl.Id,
                MetaInfoId = wl.MetaInfoId,
                MetaInfoTitle = wl.MetaInfo.Title,
                Status = wl.MetaInfo.Status,
                IsPublic = wl.MetaInfo.IsPublic,
                CreatedAt = wl.MetaInfo.CreatedAt,
                LastModifiedAt = wl.MetaInfo.LastModifiedAt,
                LocationType = wl.LocationType,
                ParentId = wl.ParentId,
                Description = wl.Description
            })
            .ToListAsync();

        return ApiResponseDto<IEnumerable<WorldLocationResponseDto>>.Success(locations);
    }

    // ========================================================================
    // GET - Single world location by ID
    // ========================================================================

    public async Task<ApiResponseDto<WorldLocationResponseDto>> GetWorldLocationAsync(Guid id)
    {
        var location = await _db.WorldLocations
            .Include(wl => wl.MetaInfo)
            .FirstOrDefaultAsync(wl => wl.Id == id);

        if (location is null)
            return ApiResponseDto<WorldLocationResponseDto>.NotFound($"World location with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<WorldLocationResponseDto>(location.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<WorldLocationResponseDto>.Success(new WorldLocationResponseDto
        {
            Id = location.Id,
            MetaInfoId = location.MetaInfoId,
            MetaInfoTitle = location.MetaInfo.Title,
            Status = location.MetaInfo.Status,
            IsPublic = location.MetaInfo.IsPublic,
            CreatedAt = location.MetaInfo.CreatedAt,
            LastModifiedAt = location.MetaInfo.LastModifiedAt,
            LocationType = location.LocationType,
            ParentId = location.ParentId,
            Description = location.Description
        });
    }

    // ========================================================================
    // POST - Create a new world location
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateWorldLocationAsync(Guid projectId, WorldLocationCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        var metaInfo = CreateMetaInfo(projectId, ContentTypeEnum.WorldLocation, createDto.CreateData);

        _db.MetaInfos.Add(metaInfo);
        await _db.SaveChangesAsync();

        var location = new WorldLocation
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id.Value,
            LocationType = createDto.LocationType,
            ParentId = createDto.ParentId,
            Description = createDto.Description,
        };

        _db.WorldLocations.Add(location);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = location.Id,
            MetaInfoId = metaInfo.Id.Value,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a world location
    // ========================================================================

    public async Task<ApiResponseDto<WorldLocationResponseDto>> UpdateWorldLocationAsync(Guid id, WorldLocationUpdateDto updateDto)
    {
        var location = await _db.WorldLocations
            .Include(wl => wl.MetaInfo)
            .FirstOrDefaultAsync(wl => wl.Id == id);

        if (location is null)
            return ApiResponseDto<WorldLocationResponseDto>.NotFound($"World location with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<WorldLocationResponseDto>(location.MetaInfo.ProjectId);
        if (error != null) return error;

        ApplyMetaInfoUpdates(location.MetaInfo, updateDto.MetaInfo);

        if (updateDto.LocationType.HasValue)
            location.LocationType = updateDto.LocationType.Value;

        if (updateDto.ParentId.HasValue)
            location.ParentId = updateDto.ParentId.Value;

        if (updateDto.Description != null)
            location.Description = updateDto.Description;

        await _db.SaveChangesAsync();

        return ApiResponseDto<WorldLocationResponseDto>.Success(new WorldLocationResponseDto
        {
            Id = location.Id,
            MetaInfoId = location.MetaInfoId,
            MetaInfoTitle = location.MetaInfo.Title,
            Status = location.MetaInfo.Status,
            IsPublic = location.MetaInfo.IsPublic,
            CreatedAt = location.MetaInfo.CreatedAt,
            LastModifiedAt = location.MetaInfo.LastModifiedAt,
            LocationType = location.LocationType,
            ParentId = location.ParentId,
            Description = location.Description
        });
    }

    // ========================================================================
    // DELETE - Remove a world location
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteWorldLocationAsync(Guid id)
    {
        var location = await _db.WorldLocations
            .Include(wl => wl.MetaInfo)
            .FirstOrDefaultAsync(wl => wl.Id == id);

        if (location is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"World location with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(location.MetaInfo.ProjectId);
        if (error != null) return error;

        _db.MetaInfos.Remove(location.MetaInfo);
        _db.WorldLocations.Remove(location);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = location.MetaInfo.ProjectId
        });
    }
}
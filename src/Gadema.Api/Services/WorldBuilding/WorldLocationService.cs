// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Dtos.WorldBuilding;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.WorldBuilding;

/// <summary>
/// Service for managing WorldLocations within the world-building domain.
/// Handles hierarchical location tree (Country → Region → City → Village).
/// </summary>
public class WorldLocationService : CoreService
{
    public WorldLocationService(GameDbContext db, ILogger<WorldLocationService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    /// <summary>Creates a response DTO from a WorldLocation entity.</summary>
    private WorldLocationResponseDto CreateResponseDto(WorldLocation location)
        => new()
        {
            Id = location.Id,
            MetaInfoId = location.MetaInfoId,
            MetaInfoTitle = location.ContentMetaInfo.Title,
            Status = location.ContentMetaInfo.Status,
            IsPublic = location.ContentMetaInfo.IsPublic,
            LocationType = location.LocationType,
            ParentId = location.ParentId,
            Description = location.Description,
            CreatedAt = location.ContentMetaInfo.CreatedAt,
            LastModifiedAt = location.ContentMetaInfo.LastModifiedAt,
        };

    /// <summary>Creates a list response DTO from collection.</summary>
    private ListResponseDto<WorldLocationResponseDto> CreateListResponseDto(IEnumerable<WorldLocation> locations)
        => new() { Items = locations.Select(CreateResponseDto).ToList(), TotalCount = locations.Count() };

    // ========================================================================
    // GET - List all world locations for a project (with optional filter)
    // ========================================================================

    public async Task<ApiResponseDto<ListResponseDto<WorldLocationResponseDto>>> GetLocationsAsync(Guid projectId, int? locationType = null, Guid? parentId = null)
    {
        var error = await ValidateProjectAccessAsync<ListResponseDto<WorldLocationResponseDto>>(projectId);
        if (error != null) return error;

        IQueryable<WorldLocation> query = _db.WorldLocations
            .Include(wl => wl.ContentMetaInfo)
            .Where(wl => wl.ContentMetaInfo.ProjectId == projectId);

        if (locationType.HasValue)
            query = query.Where(wl => wl.LocationType == (LocationType)locationType.Value);

        if (parentId.HasValue)
            query = query.Where(wl => wl.ParentId == parentId.Value || wl.ParentId == null);

        var locations = await query.OrderBy(wl => wl.LocationType).ThenBy(wl => wl.Id).ToListAsync();
        return ApiResponseDto<ListResponseDto<WorldLocationResponseDto>>.Success(CreateListResponseDto(locations));
    }

    // ========================================================================
    // GET - Single world location by ID
    // ========================================================================

    public async Task<ApiResponseDto<WorldLocationResponseDto>> GetLocationAsync(Guid id)
    {
        var location = await _db.WorldLocations
            .Include(wl => wl.ContentMetaInfo)
            .FirstOrDefaultAsync(wl => wl.Id == id);

        if (location is null)
            return ApiResponseDto<WorldLocationResponseDto>.NotFound($"World location with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<WorldLocationResponseDto>(location.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<WorldLocationResponseDto>.Success(CreateResponseDto(location));
    }

    // ========================================================================
    // POST - Create a new world location
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateLocationAsync(Guid projectId, WorldLocationCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        var ContentMetaInfo = CreateMetaInfo(projectId, ContentTypeEnum.WorldLocation, createDto.CreateData);

        _db.MetaInfos.Add(ContentMetaInfo);
        await _db.SaveChangesAsync();

        var location = new WorldLocation
        {
            Id = Guid.NewGuid(),
            MetaInfoId = ContentMetaInfo.Id,
            LocationType = (LocationType)createDto.LocationType,
            ParentId = createDto.ParentId,
            Description = createDto.Description,
        };

        _db.WorldLocations.Add(location);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = location.Id,
            MetaInfoId = ContentMetaInfo.Id,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a world location
    // ========================================================================

    public async Task<ApiResponseDto<WorldLocationResponseDto>> UpdateLocationAsync(Guid id, WorldLocationUpdateDto updateDto)
    {
        var location = await _db.WorldLocations
            .Include(wl => wl.ContentMetaInfo)
            .FirstOrDefaultAsync(wl => wl.Id == id);

        if (location is null)
            return ApiResponseDto<WorldLocationResponseDto>.NotFound($"World location with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<WorldLocationResponseDto>(location.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        ApplyMetaInfoUpdates(location.ContentMetaInfo, updateDto.ContentMetaInfo);

        if (updateDto.LocationType.HasValue)
            location.LocationType = (LocationType)updateDto.LocationType.Value;

        if (updateDto.ParentId.HasValue)
            location.ParentId = updateDto.ParentId.Value;

        if (updateDto.Description != null)
            location.Description = updateDto.Description;

        await _db.SaveChangesAsync();

        return ApiResponseDto<WorldLocationResponseDto>.Success(CreateResponseDto(location));
    }

    // ========================================================================
    // DELETE - Remove a world location
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteLocationAsync(Guid id)
    {
        var location = await _db.WorldLocations
            .Include(wl => wl.ContentMetaInfo)
            .FirstOrDefaultAsync(wl => wl.Id == id);

        if (location is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"World location with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(location.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        _db.MetaInfos.Remove(location.ContentMetaInfo);
        _db.WorldLocations.Remove(location);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = location.ContentMetaInfo.ProjectId
        });
    }
}
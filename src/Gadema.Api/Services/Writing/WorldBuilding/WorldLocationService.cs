using Gadema.Api.CoreServices;
using Gadema.Api.CoreServices.Strategies;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Dtos.Writing.WorldBuilding;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Enums;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Core.Models.Writing.WorldBuilding;
using Gadema.Data.Database.Core;
using Gadema.Data.Database.Writing;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Writing.WorldBuilding;

[ServiceLifetime(ServiceLifetime.Scoped)] public class WorldLocationService : DomainService
{
    private WritingDbContext _db;

    public WorldLocationService( WritingDbContext db,
        ILogger<WorldLocationService> logger,  
        CoreServicesProvider coreServices) 
        : base(coreServices,logger) { _db = db; }

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

    private ListResponseDto<WorldLocationResponseDto> CreateListResponseDto(IEnumerable<WorldLocation> locations)
        => new() { Items = locations.Select(CreateResponseDto).ToList(), TotalCount = locations.Count() };

    public async Task<ApiResponseDto<ListResponseDto<WorldLocationResponseDto>>> GetLocationsAsync(Guid projectId, int? locationType = null, Guid? parentId = null)
    {
        var error = await CheckAccessAsync<ListResponseDto<WorldLocationResponseDto>>(projectId, Permission.CanView);
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

    public async Task<ApiResponseDto<WorldLocationResponseDto>> GetLocationAsync(Guid id)
    {
        var location = await _db.WorldLocations
            .Include(wl => wl.ContentMetaInfo)
            .FirstOrDefaultAsync(wl => wl.Id == id);

        if (location is null)
            return ApiResponseDto<WorldLocationResponseDto>.NotFound($"World location with ID {id} not found.");

        var error = await CheckAccessAsync<WorldLocationResponseDto>(location.ContentMetaInfo.ProjectId, Permission.CanView);
        if (error != null) return error;

        return ApiResponseDto<WorldLocationResponseDto>.Success(CreateResponseDto(location));
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateLocationAsync(Guid projectId, WorldLocationCreateDto createDto)
    {
        var error = await CheckAccessAsync<CreateResponseDto>(projectId, Permission.CanEdit);
        if (error != null) return error;

        var contentMetaInfo = await _core.MetadataService.CreateAsync<ContentMetaInfo>(createDto.CreateData, m =>
        {
            m.ProjectId = projectId;
            m.ContentType = ContentTypeEnum.WorldLocation;
        });

        // Law I: Unification - Body.Id == Soul.Id
        var location = new WorldLocation
        {
            Id = contentMetaInfo.Id, 
            MetaInfoId = contentMetaInfo.Id,
            LocationType = (LocationType)createDto.LocationType,
            ParentId = createDto.ParentId,
            Description = createDto.Description,
        };

        _db.WorldLocations.Add(location);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = location.Id,
            MetaInfoId = contentMetaInfo.Id,
            ProjectId = projectId
        });
    }

    public async Task<ApiResponseDto<WorldLocationResponseDto>> UpdateLocationAsync(Guid id, WorldLocationUpdateDto updateDto)
    {
        var location = await _db.WorldLocations
            .Include(wl => wl.ContentMetaInfo)
            .FirstOrDefaultAsync(wl => wl.Id == id);

        if (location is null)
            return ApiResponseDto<WorldLocationResponseDto>.NotFound($"World location with ID {id} not found.");

        var error = await CheckAccessAsync<WorldLocationResponseDto>(location.ContentMetaInfo.ProjectId, Permission.CanEdit);
        if (error != null) return error;

        if (updateDto.ContentMetaInfo != null)
        {
            var success = await SyncIdentityAsync<ContentIdentityStrategy>(location.MetaInfoId, updateDto.ContentMetaInfo);
            if (!success) return ApiResponseDto<WorldLocationResponseDto>.ServerError("Sync failed.");
        }

        if (updateDto.LocationType.HasValue)
            location.LocationType = (LocationType)updateDto.LocationType.Value;

        if (updateDto.ParentId.HasValue)
            location.ParentId = updateDto.ParentId.Value;

        if (updateDto.Description != null)
            location.Description = updateDto.Description;

        await _db.SaveChangesAsync();

        return ApiResponseDto<WorldLocationResponseDto>.Success(CreateResponseDto(location));
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteLocationAsync(Guid id)
    {
        var location = await _db.WorldLocations
            .Include(wl => wl.ContentMetaInfo)
            .FirstOrDefaultAsync(wl => wl.Id == id);

        if (location is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"World location with ID {id} not found.");

        var error = await CheckAccessAsync<DeleteResponseDto>(location.ContentMetaInfo.ProjectId, Permission.CanDelete);
        if (error != null) return error;

        _db.WorldLocations.Remove(location);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = location.ContentMetaInfo.ProjectId.Value
        });
    }
}
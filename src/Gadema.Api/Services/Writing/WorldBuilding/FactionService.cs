using Gadema.Api.CoreServices;
using Gadema.Api.CoreServices.Strategies;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Writing.WorldBuilding;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Enums;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Core.Models.Writing.WorldBuilding;
using Gadema.Data.Database.Core;
using Gadema.Data.Database.Writing;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Writing.WorldBuilding;

[ServiceLifetime(ServiceLifetime.Scoped)] public class FactionService : DomainService
{
    private WritingDbContext _db;

    public FactionService( WritingDbContext db,
        ILogger<FactionService> logger,  
        CoreServicesProvider coreServices) // Injected via CoreService constructor
        : base(coreServices,logger) { _db = db; }

    // ========================================================================
    // GET - List all factions for a project
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<FactionResponseDto>>> GetFactionsAsync(Guid projectId)
    {
        var error = await CheckAccessAsync<IEnumerable<FactionResponseDto>>(projectId, Permission.CanView);
        if (error != null) return error;

        var factions = await _db.Factions
            .Include(f => f.ContentMetaInfo)
            .Include(f => f.Location)
            .Where(f => f.ContentMetaInfo.ProjectId == projectId)
            .OrderBy(f => f.ContentMetaInfo.Title)
            .Select(f => new FactionResponseDto
            {
                Id = f.Id,
                MetaInfoId = f.MetaInfoId,
                MetaInfoTitle = f.ContentMetaInfo.Title,
                Status = f.ContentMetaInfo.Status,
                IsPublic = f.ContentMetaInfo.IsPublic,
                CreatedAt = f.ContentMetaInfo.CreatedAt,
                LastModifiedAt = f.ContentMetaInfo.LastModifiedAt,
                Ideology = f.Ideology,
                Goals = f.Goals,
                LocationId = f.LocationId,
                LocationName = f.Location != null ? f.Location.ContentMetaInfo.Title : null
            })
            .ToListAsync();

        return ApiResponseDto<IEnumerable<FactionResponseDto>>.Success(factions);
    }

    // ========================================================================
    // GET - Single faction by ID
    // ========================================================================

    public async Task<ApiResponseDto<FactionResponseDto>> GetFactionAsync(Guid id)
    {
        var faction = await _db.Factions
            .Include(f => f.ContentMetaInfo)
            .Include(f => f.Location)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (faction is null)
            return ApiResponseDto<FactionResponseDto>.NotFound($"Faction with ID {id} not found.");

        var error = await CheckAccessAsync<FactionResponseDto>(faction.ContentMetaInfo.ProjectId, Permission.CanView);
        if (error != null) return error;

        return ApiResponseDto<FactionResponseDto>.Success(new FactionResponseDto
        {
            Id = faction.Id,
            MetaInfoId = faction.MetaInfoId,
            MetaInfoTitle = faction.ContentMetaInfo.Title,
            Status = faction.ContentMetaInfo.Status,
            IsPublic = faction.ContentMetaInfo.IsPublic,
            CreatedAt = faction.ContentMetaInfo.CreatedAt,
            LastModifiedAt = faction.ContentMetaInfo.LastModifiedAt,
            Ideology = faction.Ideology,
            Goals = faction.Goals,
            LocationId = faction.LocationId,
            LocationName = faction.Location != null ? faction.Location.ContentMetaInfo.Title : null
        });
    }

    // ========================================================================
    // POST - Create a new faction
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateFactionAsync(Guid projectId, FactionCreateDto createDto)
    {
        var error = await CheckAccessAsync<CreateResponseDto>(projectId, Permission.CanEdit);
        if (error != null) return error;
        
        var contentMetaInfo = await _core.MetadataService.CreateAsync<ContentMetaInfo>(createDto.CreateData, m =>
        {
            m.ProjectId = projectId;
            m.ContentType = ContentTypeEnum.Faction;
        });

        var faction = new Faction
        {
            Id = Guid.NewGuid(),
            MetaInfoId = contentMetaInfo.Id,
            Ideology = createDto.Ideology,
            Goals = createDto.Goals,
            LocationId = createDto.LocationId,
        };

        _db.Factions.Add(faction);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = faction.Id,
            MetaInfoId = contentMetaInfo.Id,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a faction
    // ========================================================================

    public async Task<ApiResponseDto<FactionResponseDto>> UpdateFactionAsync(Guid id, FactionUpdateDto updateDto)
    {
        var faction = await _db.Factions
            .Include(f => f.ContentMetaInfo)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (faction is null)
            return ApiResponseDto<FactionResponseDto>.NotFound($"Faction with ID {id} not found.");

        var error = await CheckAccessAsync<FactionResponseDto>(faction.ContentMetaInfo.ProjectId, Permission.CanEdit);
        if (error != null) return error;

        if (updateDto.ContentMetaInfo != null)
        {
            var success = await SyncIdentityAsync<ContentIdentityStrategy>(faction.MetaInfoId, updateDto.ContentMetaInfo);
            if (!success) return ApiResponseDto<FactionResponseDto>.ServerError("Sync failed.");
        }

        if (updateDto.Ideology != null)
            faction.Ideology = updateDto.Ideology;

        if (updateDto.Goals != null)
            faction.Goals = updateDto.Goals;

        if (updateDto.LocationId.HasValue)
            faction.LocationId = updateDto.LocationId.Value;

        await _db.SaveChangesAsync();

        return ApiResponseDto<FactionResponseDto>.Success(new FactionResponseDto
        {
            Id = faction.Id,
            MetaInfoId = faction.MetaInfoId,
            MetaInfoTitle = faction.ContentMetaInfo.Title,
            Status = faction.ContentMetaInfo.Status,
            IsPublic = faction.ContentMetaInfo.IsPublic,
            CreatedAt = faction.ContentMetaInfo.CreatedAt,
            LastModifiedAt = faction.ContentMetaInfo.LastModifiedAt,
            Ideology = faction.Ideology,
            Goals = faction.Goals,
            LocationId = faction.LocationId,
            LocationName = faction.Location != null ? faction.Location.ContentMetaInfo.Title : null
        });
    }

    // ========================================================================
    // DELETE - Remove a faction
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteFactionAsync(Guid id)
    {
        var faction = await _db.Factions
            .Include(f => f.ContentMetaInfo)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (faction is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Faction with ID {id} not found.");

        var error = await CheckAccessAsync<DeleteResponseDto>(faction.ContentMetaInfo.ProjectId, Permission.CanDelete);
        if (error != null) return error;

        
        _db.Factions.Remove(faction);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = faction.ContentMetaInfo.ProjectId.Value
        });
    }
}
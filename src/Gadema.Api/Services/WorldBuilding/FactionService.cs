// src/Gadema.Api/Services/WorldBuilding/FactionService.cs
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.WorldBuilding;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Models.WorldBuilding;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.WorldBuilding;

public class FactionService : CoreService
{
    public FactionService(GameDbContext db, ILogger<FactionService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // GET - List all factions for a project
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<FactionResponseDto>>> GetFactionsAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<FactionResponseDto>>(projectId);
        if (error != null) return error;

        var factions = await _db.Factions
            .Include(f => f.MetaInfo)
            .Include(f => f.Location)
            .Where(f => f.MetaInfo.ProjectId == projectId)
            .OrderBy(f => f.MetaInfo.Title)
            .Select(f => new FactionResponseDto
            {
                Id = f.Id,
                MetaInfoId = f.MetaInfoId,
                MetaInfoTitle = f.MetaInfo.Title,
                Status = f.MetaInfo.Status,
                IsPublic = f.MetaInfo.IsPublic,
                CreatedAt = f.MetaInfo.CreatedAt,
                LastModifiedAt = f.MetaInfo.LastModifiedAt,
                Ideology = f.Ideology,
                Goals = f.Goals,
                LocationId = f.LocationId,
                LocationName = f.Location != null ? f.Location.MetaInfo.Title : null
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
            .Include(f => f.MetaInfo)
            .Include(f => f.Location)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (faction is null)
            return ApiResponseDto<FactionResponseDto>.NotFound($"Faction with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<FactionResponseDto>(faction.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<FactionResponseDto>.Success(new FactionResponseDto
        {
            Id = faction.Id,
            MetaInfoId = faction.MetaInfoId,
            MetaInfoTitle = faction.MetaInfo.Title,
            Status = faction.MetaInfo.Status,
            IsPublic = faction.MetaInfo.IsPublic,
            CreatedAt = faction.MetaInfo.CreatedAt,
            LastModifiedAt = faction.MetaInfo.LastModifiedAt,
            Ideology = faction.Ideology,
            Goals = faction.Goals,
            LocationId = faction.LocationId,
            LocationName = faction.Location != null ? faction.Location.MetaInfo.Title : null
        });
    }

    // ========================================================================
    // POST - Create a new faction
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateFactionAsync(Guid projectId, FactionCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        var metaInfo = CreateMetaInfo(projectId, ContentTypeEnum.Faction, createDto.CreateData);

        _db.MetaInfos.Add(metaInfo);
        await _db.SaveChangesAsync();

        var faction = new Faction
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id.Value,
            Ideology = createDto.Ideology,
            Goals = createDto.Goals,
            LocationId = createDto.LocationId,
        };

        _db.Factions.Add(faction);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = faction.Id,
            MetaInfoId = metaInfo.Id.Value,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a faction
    // ========================================================================

    public async Task<ApiResponseDto<FactionResponseDto>> UpdateFactionAsync(Guid id, FactionUpdateDto updateDto)
    {
        var faction = await _db.Factions
            .Include(f => f.MetaInfo)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (faction is null)
            return ApiResponseDto<FactionResponseDto>.NotFound($"Faction with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<FactionResponseDto>(faction.MetaInfo.ProjectId);
        if (error != null) return error;

        ApplyMetaInfoUpdates(faction.MetaInfo, updateDto.MetaInfo);

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
            MetaInfoTitle = faction.MetaInfo.Title,
            Status = faction.MetaInfo.Status,
            IsPublic = faction.MetaInfo.IsPublic,
            CreatedAt = faction.MetaInfo.CreatedAt,
            LastModifiedAt = faction.MetaInfo.LastModifiedAt,
            Ideology = faction.Ideology,
            Goals = faction.Goals,
            LocationId = faction.LocationId,
            LocationName = faction.Location != null ? faction.Location.MetaInfo.Title : null
        });
    }

    // ========================================================================
    // DELETE - Remove a faction
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteFactionAsync(Guid id)
    {
        var faction = await _db.Factions
            .Include(f => f.MetaInfo)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (faction is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Faction with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(faction.MetaInfo.ProjectId);
        if (error != null) return error;

        _db.MetaInfos.Remove(faction.MetaInfo);
        _db.Factions.Remove(faction);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = faction.MetaInfo.ProjectId
        });
    }
}
i// src/Gadema.Api/Services/WorldBuilding/FactionService.cs
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.WorldBuilding;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Models.WorldBuilding;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;


namespace Gadema.Api.Services.Writing.WorldBuilding;

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

        var error = await ValidateProjectAccessAsync<FactionResponseDto>(faction.ContentMetaInfo.ProjectId);
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
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        var ContentMetaInfo = CreateMetaInfo(projectId, ContentTypeEnum.Faction, createDto.CreateData);

        _db.MetaInfos.Add(ContentMetaInfo);
        await _db.SaveChangesAsync();

        var faction = new Faction
        {
            Id = Guid.NewGuid(),
            MetaInfoId = ContentMetaInfo.Id,
            Ideology = createDto.Ideology,
            Goals = createDto.Goals,
            LocationId = createDto.LocationId,
        };

        _db.Factions.Add(faction);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = faction.Id,
            MetaInfoId = ContentMetaInfo.Id,
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

        var error = await ValidateProjectAccessAsync<FactionResponseDto>(faction.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        ApplyMetaInfoUpdates(faction.ContentMetaInfo, updateDto.ContentMetaInfo);

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

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(faction.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        _db.MetaInfos.Remove(faction.ContentMetaInfo);
        _db.Factions.Remove(faction);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = faction.ContentMetaInfo.ProjectId
        });
    }
}
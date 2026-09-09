// src/Gadema.Api/Services/Characters/CharacterStateService.cs
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Characters;
using Gadema.Core.Enums;

using Gadema.Core.Models;
using Gadema.Core.Models.Characters;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Characters;

public class CharacterStateService : CoreService
{
    public CharacterStateService(GameDbContext db, ILogger<CharacterStateService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // GET - List all character states for a project
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<CharacterStateResponseDto>>> GetCharacterStatesAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<CharacterStateResponseDto>>(projectId);
        if (error != null) return error;

        var states = await _db.CharacterStates
            .Include(cs => cs.MetaInfo)
            .Include(cs => cs.Faction)
            .Include(cs => cs.Location)
            .Where(cs => cs.MetaInfo.ProjectId == projectId)
            .OrderBy(cs => cs.MetaInfo.Title)
            .Select(cs => new CharacterStateResponseDto
            {
                Id = cs.Id,
                MetaInfoId = cs.MetaInfoId,
                MetaInfoTitle = cs.MetaInfo.Title,
                Status = cs.MetaInfo.Status,
                IsPublic = cs.MetaInfo.IsPublic,
                CreatedAt = cs.MetaInfo.CreatedAt,
                LastModifiedAt = cs.MetaInfo.LastModifiedAt,
                Role = cs.Role,
                FactionId = cs.FactionId,
                FactionName = cs.Faction != null ? cs.Faction.MetaInfo.Title : null,
                LocationId = cs.LocationId,
                LocationName = cs.Location != null ? cs.Location.MetaInfo.Title : null,
                LifeStatus = cs.LifeStatus,
                Note = cs.Note
            })
            .ToListAsync();

        return ApiResponseDto<IEnumerable<CharacterStateResponseDto>>.Success(states);
    }

    // ========================================================================
    // GET - Single character state by ID
    // ========================================================================

    public async Task<ApiResponseDto<CharacterStateResponseDto>> GetCharacterStateAsync(Guid id)
    {
        var state = await _db.CharacterStates
            .Include(cs => cs.MetaInfo)
            .Include(cs => cs.Faction)
            .Include(cs => cs.Location)
            .FirstOrDefaultAsync(cs => cs.Id == id);

        if (state is null)
            return ApiResponseDto<CharacterStateResponseDto>.NotFound($"Character state with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<CharacterStateResponseDto>(state.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<CharacterStateResponseDto>.Success(new CharacterStateResponseDto
        {
            Id = state.Id,
            MetaInfoId = state.MetaInfoId,
            MetaInfoTitle = state.MetaInfo.Title,
            Status = state.MetaInfo.Status,
            IsPublic = state.MetaInfo.IsPublic,
            CreatedAt = state.MetaInfo.CreatedAt,
            LastModifiedAt = state.MetaInfo.LastModifiedAt,
            Role = state.Role,
            FactionId = state.FactionId,
            FactionName = state.Faction != null ? state.Faction.MetaInfo.Title : null,
            LocationId = state.LocationId,
            LocationName = state.Location != null ? state.Location.MetaInfo.Title : null,
            LifeStatus = state.LifeStatus,
            Note = state.Note
        });
    }

    // ========================================================================
    // POST - Create a new character state
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateCharacterStateAsync(Guid projectId, CharacterStateCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        var metaInfo = CreateMetaInfo(projectId, ContentTypeEnum.CharacterState, createDto.CreateData);

        _db.MetaInfos.Add(metaInfo);
        await _db.SaveChangesAsync();

        var state = new CharacterState
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id.Value,
            Role = createDto.Role,
            FactionId = createDto.FactionId,
            LocationId = createDto.LocationId,
            LifeStatus = createDto.LifeStatus,
            Note = createDto.Note,
        };

        _db.CharacterStates.Add(state);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = state.Id,
            MetaInfoId = metaInfo.Id.Value,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a character state
    // ========================================================================

    public async Task<ApiResponseDto<CharacterStateResponseDto>> UpdateCharacterStateAsync(Guid id, CharacterStateUpdateDto updateDto)
    {
        var state = await _db.CharacterStates
            .Include(cs => cs.MetaInfo)
            .FirstOrDefaultAsync(cs => cs.Id == id);

        if (state is null)
            return ApiResponseDto<CharacterStateResponseDto>.NotFound($"Character state with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<CharacterStateResponseDto>(state.MetaInfo.ProjectId);
        if (error != null) return error;

        ApplyMetaInfoUpdates(state.MetaInfo, updateDto.MetaInfo);

        if (updateDto.Role.HasValue)
            state.Role = updateDto.Role.Value;

        if (updateDto.FactionId.HasValue)
            state.FactionId = updateDto.FactionId.Value;

        if (updateDto.LocationId.HasValue)
            state.LocationId = updateDto.LocationId.Value;

        if (updateDto.LifeStatus.HasValue)
            state.LifeStatus = updateDto.LifeStatus.Value;

        if (updateDto.Note != null)
            state.Note = updateDto.Note;

        await _db.SaveChangesAsync();

        return ApiResponseDto<CharacterStateResponseDto>.Success(new CharacterStateResponseDto
        {
            Id = state.Id,
            MetaInfoId = state.MetaInfoId,
            MetaInfoTitle = state.MetaInfo.Title,
            Status = state.MetaInfo.Status,
            IsPublic = state.MetaInfo.IsPublic,
            CreatedAt = state.MetaInfo.CreatedAt,
            LastModifiedAt = state.MetaInfo.LastModifiedAt,
            Role = state.Role,
            FactionId = state.FactionId,
            FactionName = state.Faction != null ? state.Faction.MetaInfo.Title : null,
            LocationId = state.LocationId,
            LocationName = state.Location != null ? state.Location.MetaInfo.Title : null,
            LifeStatus = state.LifeStatus,
            Note = state.Note
        });
    }

    // ========================================================================
    // DELETE - Remove a character state
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteCharacterStateAsync(Guid id)
    {
        var state = await _db.CharacterStates
            .Include(cs => cs.MetaInfo)
            .FirstOrDefaultAsync(cs => cs.Id == id);

        if (state is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Character state with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(state.MetaInfo.ProjectId);
        if (error != null) return error;

        _db.MetaInfos.Remove(state.MetaInfo);
        _db.CharacterStates.Remove(state);
        await _db.SaveChangesAsync();

         return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = state.MetaInfo.ProjectId
        });
    }
}
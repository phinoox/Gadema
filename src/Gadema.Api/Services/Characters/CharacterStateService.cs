// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Characters;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Models.Characters;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Characters;

/// <summary>
/// Service for managing CharacterStates - tracking a character's state at a specific point in time.
/// Links characters to scenes via the scene trigger mechanism.
/// </summary>
public class CharacterStateService : CoreService
{
    public CharacterStateService(GameDbContext db, ILogger<CharacterStateService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    /// <summary>Creates a response DTO from a CharacterState entity.</summary>
    private CharacterStateResponseDto CreateResponseDto(CharacterState state)
        => new()
        {
            Id = state.Id,
            MetaInfoId = state.MetaInfoId,
            StateName = state.StateName,
            Description = state.Description,
            TriggerSceneId = state.TriggerSceneId,
            CurrentValue = state.CurrentValue,
            CreatedAt = state.CreatedAt,
        };

    // ========================================================================
    // GET - List all character states for a project
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<CharacterStateResponseDto>>> GetStatesAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<CharacterStateResponseDto>>(projectId);
        if (error != null) return error;

        var states = await _db.CharacterStates
            .Include(cs => cs.MetaInfo)
            .Where(cs => cs.MetaInfo.ProjectId == projectId)
            .OrderByDescending(cs => cs.CreatedAt)
            .Select(CreateResponseDto)
            .ToListAsync();

        return ApiResponseDto<IEnumerable<CharacterStateResponseDto>>.Success(states);
    }

    // ========================================================================
    // GET - Single character state by ID
    // ========================================================================

    public async Task<ApiResponseDto<CharacterStateResponseDto>> GetStateAsync(Guid id)
    {
        var state = await _db.CharacterStates
            .Include(cs => cs.MetaInfo)
            .FirstOrDefaultAsync(cs => cs.Id == id);

        if (state is null)
            return ApiResponseDto<CharacterStateResponseDto>.NotFound($"Character state with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<CharacterStateResponseDto>(state.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<CharacterStateResponseDto>.Success(CreateResponseDto(state));
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
            StateName = createDto.StateName,
            Description = createDto.Description,
            TriggerSceneId = createDto.TriggerSceneId,
            CurrentValue = createDto.CurrentValue,
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
        var state = await _db.CharacterStates.Include(cs => cs.MetaInfo).FirstOrDefaultAsync(cs => cs.Id == id);

        if (state is null)
            return ApiResponseDto<CharacterStateResponseDto>.NotFound($"Character state with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<CharacterStateUpdateDto>(state.MetaInfo.ProjectId);
        if (error != null) return error;

        ApplyMetaInfoUpdates(state.MetaInfo, updateDto.MetaInfo);

        if (!string.IsNullOrWhiteSpace(updateDto.StateName)) state.StateName = updateDto.StateName;
        if (updateDto.Description != null) state.Description = updateDto.Description;
        if (updateDto.TriggerSceneId.HasValue) state.TriggerSceneId = updateDto.TriggerSceneId.Value;
        if (updateDto.CurrentValue != null) state.CurrentValue = updateDto.CurrentValue;

        await _db.SaveChangesAsync();

        return ApiResponseDto<CharacterStateResponseDto>.Success(CreateResponseDto(state));
    }

    // ========================================================================
    // DELETE - Remove a character state
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteCharacterStateAsync(Guid id)
    {
        var state = await _db.CharacterStates.Include(cs => cs.MetaInfo).FirstOrDefaultAsync(cs => cs.Id == id);

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
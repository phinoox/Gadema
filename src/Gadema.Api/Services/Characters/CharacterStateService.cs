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
            .Include(cs => cs.ContentMetaInfo)
            .Where(cs => cs.ContentMetaInfo.ProjectId == projectId)
            .OrderByDescending(cs => cs.CreatedAt)
            .ToListAsync();

        // 2. Perform the projection in-memory using your C# method
        var projectedStates = states.Select(CreateResponseDto).ToList();

        return ApiResponseDto<IEnumerable<CharacterStateResponseDto>>.Success(projectedStates);
    }

    // ========================================================================
    // GET - Single character state by ID
    // ========================================================================

   public async Task<ApiResponseDto<CharacterStateResponseDto>> GetStateAsync(Guid id)
    {
        var state = await _db.CharacterStates
            .Include(cs => cs.ContentMetaInfo)
            .FirstOrDefaultAsync(cs => cs.Id == id);

        if (state is null)
            return ApiResponseDto<CharacterStateResponseDto>.NotFound($"Character state with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<CharacterStateResponseDto>(state.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<CharacterStateResponseDto>.Success(CreateResponseDto(state));
    }

    // ========================================================================
    // POST - Create a new character state
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateCharacterStateAsync(Guid projectId, Guid characterId, CharacterStateCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        // 1. Ensure the character belongs to this project
        var character = await _db.Characters
            .Include(c => c.ContentMetaInfo)
            .FirstOrDefaultAsync(c => c.Id == characterId && c.ContentMetaInfo.ProjectId == projectId);

        if (character is null)
            return ApiResponseDto<CreateResponseDto>.NotFound($"Character with ID {characterId} not found in this project.");

        // 2. Create ContentMetaInfo wrapper
        var ContentMetaInfo = CreateMetaInfo(projectId, ContentTypeEnum.CharacterState, createDto.CreateData);

        _db.MetaInfos.Add(ContentMetaInfo);
        await _db.SaveChangesAsync();

        // 3. Create the state entity
        var state = new CharacterState
        {
            Id = Guid.NewGuid(),
            MetaInfoId = ContentMetaInfo.Id,
            StateName = createDto.StateName,
            Description = createDto.Description,
            CurrentValue = createDto.CurrentValue,
            Role = createDto.Role,
            FactionId = createDto.FactionId,
            LocationId = createDto.LocationId,
            LifeStatus = createDto.LifeStatus,
            Note = createDto.Note,
            TriggerSceneId = createDto.TriggerSceneId,
            CharacterId = character.Id
        };

        _db.CharacterStates.Add(state);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = state.Id,
            MetaInfoId = ContentMetaInfo.Id,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a character state
    // ========================================================================

   public async Task<ApiResponseDto<CharacterStateResponseDto>> UpdateCharacterStateAsync(Guid id, CharacterStateUpdateDto updateDto)
    {
        var state = await _db.CharacterStates
            .Include(cs => cs.ContentMetaInfo)
            .FirstOrDefaultAsync(cs => cs.Id == id);

        if (state is null)
            return ApiResponseDto<CharacterStateResponseDto>.NotFound($"Character state with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<CharacterStateUpdateDto>(state.ContentMetaInfo.ProjectId);
        if (error != null) return ApiResponseDto<CharacterStateResponseDto>.Unauthorized(error.Message ?? "not authorized");

        // Update ContentMetaInfo via helper
        if (updateDto.ContentMetaInfo != null)
        {
            ApplyMetaInfoUpdates(state.ContentMetaInfo, updateDto.ContentMetaInfo);
        }

        // Update State properties
        if (!string.IsNullOrWhiteSpace(updateDto.StateName)) state.StateName = updateDto.StateName;
        if (updateDto.Description != null) state.Description = updateDto.Description;
        if (updateDto.TriggerSceneId.HasValue) state.TriggerSceneId = updateDto.TriggerSceneId.Value;
        if (updateDto.CurrentValue.HasValue) state.CurrentValue = updateDto.CurrentValue.Value;
        
        if (updateDto.Role.HasValue) state.Role = updateDto.Role.Value;
        if (updateDto.FactionId.HasValue) state.FactionId = updateDto.FactionId.Value;
        if (updateDto.LocationId.HasValue) state.LocationId = updateDto.LocationId.Value;
        if (updateDto.LifeStatus.HasValue) state.LifeStatus = updateDto.LifeStatus.Value;
        if (updateDto.Note != null) state.Note = updateDto.Note;

        await _db.SaveChangesAsync();

        return ApiResponseDto<CharacterStateResponseDto>.Success(CreateResponseDto(state));
    }

    // ========================================================================
    // DELETE - Remove a character state
    // ========================================================================

     public async Task<ApiResponseDto<DeleteResponseDto>> DeleteCharacterStateAsync(Guid id)
    {
        var state = await _db.CharacterStates
            .Include(cs => cs.ContentMetaInfo)
            .FirstOrDefaultAsync(cs => cs.Id == id);

        if (state is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Character state with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(state.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        // Remove ContentMetaInfo and the associated State
        _db.MetaInfos.Remove(state.ContentMetaInfo);
        _db.CharacterStates.Remove(state);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = state.ContentMetaInfo.ProjectId
        });
    }
   
}
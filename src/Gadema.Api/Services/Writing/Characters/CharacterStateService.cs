using Gadema.Api.CoreServices;
using Gadema.Api.CoreServices.Strategies;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Writing.Characters;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Enums;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Core.Models.Writing.Characters;
using Gadema.Data.Database.Core;
using Gadema.Data.Database.Writing;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Writing.Characters;

/// <summary>
/// Service for managing CharacterStates - tracking a character's state at a specific point in time.
/// Links characters to scenes via the scene trigger mechanism.
/// </summary>
[ServiceLifetime(ServiceLifetime.Scoped)] public class CharacterStateService : DomainService
{
    private WritingDbContext _db;

    public CharacterStateService( WritingDbContext db,
        ILogger<CharacterStateService> logger,  
        CoreServicesProvider coreServices) // Injected via CoreService constructor
        : base(coreServices,logger) { _db = db; }

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
        var error = await CheckAccessAsync<IEnumerable<CharacterStateResponseDto>>(projectId, Permission.CanView);
        if (error != null) return error;

        var states = await _db.CharacterStates
            .Include(cs => cs.ContentMetaInfo)
            .Where(cs => cs.ContentMetaInfo.ProjectId == projectId)
            .OrderByDescending(cs => cs.CreatedAt)
            .ToListAsync();

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

        var error = await CheckAccessAsync<CharacterStateResponseDto>(state.ContentMetaInfo.ProjectId, Permission.CanView);
        if (error != null) return error;

        return ApiResponseDto<CharacterStateResponseDto>.Success(CreateResponseDto(state));
    }

    // ========================================================================
    // POST - Create a new character state
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateCharacterStateAsync(Guid projectId, Guid characterId, CharacterStateCreateDto createDto)
    {
        var error = await CheckAccessAsync<CreateResponseDto>(projectId, Permission.CanEdit);
        if (error != null) return error;

        // 1. Ensure the character belongs to this project
        var character = await _db.Characters
            .Include(c => c.ContentMetaInfo)
            .FirstOrDefaultAsync(c => c.Id == characterId && c.ContentMetaInfo.ProjectId == projectId);

        if (character is null)
            return ApiResponseDto<CreateResponseDto>.NotFound($"Character with ID {characterId} not found in this project.");

        // 2. Create ContentMetaInfo wrapper
        var contentMetaInfo = await _core.MetadataService.CreateAsync<ContentMetaInfo>(createDto.CreateData, m =>
        {
            m.ProjectId = projectId;
            m.ContentType = ContentTypeEnum.CharacterState;
        });

        // 3. Create the state entity
        var state = new CharacterState
        {
            Id = Guid.NewGuid(),
            MetaInfoId = contentMetaInfo.Id,
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
            MetaInfoId = contentMetaInfo.Id,
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

        var error = await CheckAccessAsync<CharacterStateResponseDto>(state.ContentMetaInfo.ProjectId, Permission.CanEdit);
        if (error != null) return error;

        // Update ContentMetaInfo via helper
        if (updateDto.ContentMetaInfo != null)
        {
            var success = await SyncIdentityAsync<ContentIdentityStrategy>(state.MetaInfoId, updateDto.ContentMetaInfo);
            if (!success) return ApiResponseDto<CharacterStateResponseDto>.ServerError("Sync failed.");
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

        var error = await CheckAccessAsync<DeleteResponseDto>(state.ContentMetaInfo.ProjectId, Permission.CanDelete);
        if (error != null) return error;

        // Remove ContentMetaInfo and the associated State
        _db.Set<ContentMetaInfo>().Remove(state.ContentMetaInfo);
        _db.CharacterStates.Remove(state);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = state.ContentMetaInfo.ProjectId.Value
        });
    }
}
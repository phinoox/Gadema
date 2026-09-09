// src/Gadema.Api/Services/Characters/StoryEventService.cs
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Characters;
using Gadema.Core.Models;
using Gadema.Core.Models.Characters;
using Gadema.Core.Models.Content;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Characters;

public class StoryEventService : CoreService
{
    public StoryEventService(GameDbContext db, ILogger<StoryEventService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // GET - List all story events for a scene
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<StoryEventResponseDto>>> GetStoryEventsAsync(Guid sceneId)
    {
        var scene = await _db.Scenes
            .Include(s => s.MetaInfo)
            .FirstOrDefaultAsync(s => s.Id == sceneId);

        if (scene is null)
            return ApiResponseDto<IEnumerable<StoryEventResponseDto>>.NotFound($"Scene with ID {sceneId} not found.");

        var error = await ValidateProjectAccessAsync<IEnumerable<StoryEventResponseDto>>(scene.MetaInfo.ProjectId);
        if (error != null) return error;

        var events = await _db.StoryEvents
            .Include(se => se.Scene)
            .Include(se => se.ActorMetaInfo)
            .Where(se => se.SceneId == sceneId)
            .OrderByDescending(se => se.CreatedAt)
            .Select(se => new StoryEventResponseDto
            {
                Id = se.Id,
                SceneId = se.SceneId,
                SceneTitle = se.Scene.MetaInfo.Title,
                ActorMetaInfoId = se.ActorMetaInfoId,
                ActorName = se.ActorMetaInfo.Title,
                EventType = se.EventType,
                TargetEntityTypeId = se.TargetEntityTypeId,
                TargetEntityId = se.TargetEntityId,
                Description = se.Description,
                CreatedAt = se.CreatedAt
            })
            .ToListAsync();

        return ApiResponseDto<IEnumerable<StoryEventResponseDto>>.Success(events);
    }

    // ========================================================================
    // GET - Single story event by ID
    // ========================================================================

    public async Task<ApiResponseDto<StoryEventResponseDto>> GetStoryEventAsync(Guid id)
    {
        var evt = await _db.StoryEvents
            .Include(se => se.Scene).ThenInclude(s => s.MetaInfo)
            .Include(se => se.ActorMetaInfo)
            .FirstOrDefaultAsync(se => se.Id == id);

        if (evt is null)
            return ApiResponseDto<StoryEventResponseDto>.NotFound($"Story event with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<StoryEventResponseDto>(evt.Scene.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<StoryEventResponseDto>.Success(new StoryEventResponseDto
        {
            Id = evt.Id,
            SceneId = evt.SceneId,
            SceneTitle = evt.Scene.MetaInfo.Title,
            ActorMetaInfoId = evt.ActorMetaInfoId,
            ActorName = evt.ActorMetaInfo.Title,
            EventType = evt.EventType,
            TargetEntityTypeId = evt.TargetEntityTypeId,
            TargetEntityId = evt.TargetEntityId,
            Description = evt.Description,
            CreatedAt = evt.CreatedAt
        });
    }

    // ========================================================================
    // POST - Create a new story event
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateStoryEventAsync(Guid projectId, Guid sceneId, StoryEventCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        // Verify scene exists and belongs to this project
        var scene = await _db.Scenes
            .Include(s => s.MetaInfo)
            .FirstOrDefaultAsync(s => s.Id == sceneId);

        if (scene is null)
            return ApiResponseDto<CreateResponseDto>.NotFound($"Scene with ID {sceneId} not found.");

        if (scene.MetaInfo.ProjectId != projectId)
            return ApiResponseDto<CreateResponseDto>.BadRequest("Scene does not belong to this project.");

        // Verify actor MetaInfo exists and belongs to this project
        var actorMetaInfo = await _db.MetaInfos
            .FirstOrDefaultAsync(m => m.Id == createDto.ActorMetaInfoId);

        if (actorMetaInfo is null)
            return ApiResponseDto<CreateResponseDto>.NotFound($"Actor MetaInfo with ID {createDto.ActorMetaInfoId} not found.");

        if (actorMetaInfo.ProjectId != projectId)
            return ApiResponseDto<CreateResponseDto>.BadRequest("Actor does not belong to this project.");

        var evt = new StoryEvent
        {
            Id = Guid.NewGuid(),
            SceneId = sceneId,
            ActorMetaInfoId = createDto.ActorMetaInfoId,
            EventType = createDto.EventType,
            TargetEntityTypeId = createDto.TargetEntityTypeId,
            TargetEntityId = createDto.TargetEntityId,
            Description = createDto.Description
        };

        _db.StoryEvents.Add(evt);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = evt.Id,
            MetaInfoId = evt.ActorMetaInfoId,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a story event
    // ========================================================================

    public async Task<ApiResponseDto<StoryEventResponseDto>> UpdateStoryEventAsync(Guid id, StoryEventUpdateDto updateDto)
    {
        var evt = await _db.StoryEvents
            .Include(se => se.Scene).ThenInclude(s => s.MetaInfo)
            .Include(se => se.ActorMetaInfo)
            .FirstOrDefaultAsync(se => se.Id == id);

        if (evt is null)
            return ApiResponseDto<StoryEventResponseDto>.NotFound($"Story event with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<StoryEventResponseDto>(evt.Scene.MetaInfo.ProjectId);
        if (error != null) return error;

        if (updateDto.SceneId.HasValue)
            evt.SceneId = updateDto.SceneId.Value;

        if (updateDto.ActorMetaInfoId.HasValue)
            evt.ActorMetaInfoId = updateDto.ActorMetaInfoId.Value;

        if (updateDto.EventType.HasValue)
            evt.EventType = updateDto.EventType.Value;

        if (updateDto.Description != null)
            evt.Description = updateDto.Description;

        await _db.SaveChangesAsync();

        return ApiResponseDto<StoryEventResponseDto>.Success(new StoryEventResponseDto
        {
            Id = evt.Id,
            SceneId = evt.SceneId,
            SceneTitle = evt.Scene.MetaInfo.Title,
            ActorMetaInfoId = evt.ActorMetaInfoId,
            ActorName = evt.ActorMetaInfo.Title,
            EventType = evt.EventType,
            TargetEntityTypeId = evt.TargetEntityTypeId,
            TargetEntityId = evt.TargetEntityId,
            Description = evt.Description,
            CreatedAt = evt.CreatedAt
        });
    }

    // ========================================================================
    // DELETE - Remove a story event
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteStoryEventAsync(Guid id)
    {
        var evt = await _db.StoryEvents
            .Include(se => se.Scene).ThenInclude(s => s.MetaInfo)
            .FirstOrDefaultAsync(se => se.Id == id);

        if (evt is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Story event with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(evt.Scene.MetaInfo.ProjectId);
        if (error != null) return error;

        _db.StoryEvents.Remove(evt);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = evt.Scene.MetaInfo.ProjectId
        });
    }
}

// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Writing.Narrative;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Enums;
using Gadema.Core.Models.Writing.Narrative;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Writing.Narrative;

/// <summary>
/// Service for managing Scenes within the narrative domain.
/// Handles CRUD operations including ContentMetaInfo creation and authorization.
/// </summary>
[ServiceLifetime(ServiceLifetime.Scoped)] public class SceneService : CoreService
{
    public SceneService(GameDbContext db, ILogger<SceneService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // GET - List all scenes for a project
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<SceneResponseDto>>> GetScenesAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<SceneResponseDto>>(projectId);
        if (error != null) return error;

        var scenes = await _db.Scenes
            .Include(s => s.ContentMetaInfo)
            .Where(s => s.ContentMetaInfo.ProjectId == projectId)
            .OrderBy(s => s.OrderIndex)
            .Select(s => new SceneResponseDto
            {
                Id = s.Id,
                MetaInfoId = s.MetaInfoId,
                MetaInfoTitle = s.ContentMetaInfo.Title,
                RawText = s.RawText,
                StoryChapterId = s.StoryChapterId,
                OrderIndex = s.OrderIndex,
                Status = s.ContentMetaInfo.Status,
                CreatedAt = s.ContentMetaInfo.CreatedAt,
                LastModifiedAt = s.ContentMetaInfo.LastModifiedAt,
            })
            .ToListAsync();

        return ApiResponseDto<IEnumerable<SceneResponseDto>>.Success(scenes);
    }

    // ========================================================================
    // GET - Single scene by ID
    // ========================================================================

    public async Task<ApiResponseDto<SceneResponseDto>> GetSceneAsync(Guid id)
    {
        var scene = await _db.Scenes
            .Include(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (scene is null)
            return ApiResponseDto<SceneResponseDto>.NotFound($"Scene with ID {id} not found.");

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<SceneResponseDto>(scene.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<SceneResponseDto>.Success(new SceneResponseDto
        {
            Id = scene.Id,
            MetaInfoId = scene.MetaInfoId,
            MetaInfoTitle = scene.ContentMetaInfo.Title,
            RawText = scene.RawText,
            StoryChapterId = scene.StoryChapterId,
            OrderIndex = scene.OrderIndex,
            Status = scene.ContentMetaInfo.Status,
            CreatedAt = scene.ContentMetaInfo.CreatedAt,
            LastModifiedAt = scene.ContentMetaInfo.LastModifiedAt
        });
    }

    // ========================================================================
    // POST - Create a new scene
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateSceneAsync(Guid projectId, SceneCreateDto createDto)
    {
        // Validate project access (projectId from route matches CreateData.ProjectId)
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        var user = _userContext.CurrentUser!;
        var ContentMetaInfo = CreateMetaInfo(projectId, ContentTypeEnum.Scene, createDto.CreateData);

        _db.MetaInfos.Add(ContentMetaInfo);
        await _db.SaveChangesAsync();

        // Create the Scene entity
        var scene = new Scene
        {
            Id = Guid.NewGuid(),
            MetaInfoId = ContentMetaInfo.Id,
            RawText = string.Empty,
            StoryChapterId = createDto.StoryChapterId,
            OrderIndex = createDto.OrderIndex ?? 0,
        };

        _db.Scenes.Add(scene);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            
            
                EntityId = scene.Id,
                MetaInfoId = ContentMetaInfo.Id,
                ProjectId = projectId
            
        });
    }

    // ========================================================================
    // PUT - Partial update of a scene
    // ========================================================================

    public async Task<ApiResponseDto<SceneResponseDto>> UpdateSceneAsync(Guid id, SceneUpdateDto updateDto)
    {
        var scene = await _db.Scenes
            .Include(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (scene is null)
            return ApiResponseDto<SceneResponseDto>.NotFound($"Scene with ID {id} not found.");

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<SceneResponseDto>(scene.ContentMetaInfo.ProjectId);
        if (error != null) return error;


        if (updateDto.RawText != null)
            scene.RawText = updateDto.RawText;

            scene.StoryChapterId = updateDto.StoryChapterId;

        if (updateDto.OrderIndex.HasValue)
            scene.OrderIndex = updateDto.OrderIndex.Value;


        ApplyMetaInfoUpdates(scene.ContentMetaInfo, updateDto.ContentMetaInfo);
        

        scene.ContentMetaInfo.LastModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ApiResponseDto<SceneResponseDto>.Success(new SceneResponseDto
        {
            Id = scene.Id,
            MetaInfoId = scene.MetaInfoId,
            MetaInfoTitle = scene.ContentMetaInfo.Title,
            RawText = scene.RawText,
            StoryChapterId = scene.StoryChapterId,
            OrderIndex = scene.OrderIndex,
            Status = scene.ContentMetaInfo.Status,
            CreatedAt = scene.ContentMetaInfo.CreatedAt,
            LastModifiedAt = scene.ContentMetaInfo.LastModifiedAt
        });
    }

    // ========================================================================
    // DELETE - Remove a scene
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteSceneAsync(Guid id)
    {
        var scene = await _db.Scenes
            .Include(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (scene is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Scene with ID {id} not found.");

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(scene.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        // Delete ContentMetaInfo first (FK dependency), then Scene
        _db.MetaInfos.Remove(scene.ContentMetaInfo);
        _db.Scenes.Remove(scene);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = scene.ContentMetaInfo.ProjectId
        });
    }
}

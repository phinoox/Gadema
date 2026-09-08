// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.MetaInfos;
using Gadema.Core.Dtos.Narrative;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Models.Narrative;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Narrative;

/// <summary>
/// Service for managing Scenes within the narrative domain.
/// Handles CRUD operations including MetaInfo creation and authorization.
/// </summary>
public class SceneService : CoreService
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
            .Include(s => s.MetaInfo)
            .Where(s => s.MetaInfo.ProjectId == projectId)
            .OrderBy(s => s.OrderIndex)
            .Select(s => new SceneResponseDto
            {
                Id = s.Id,
                MetaInfoId = s.MetaInfoId,
                MetaInfoTitle = s.MetaInfo.Title,
                RawText = s.RawText,
                StoryOutlineId = s.StoryOutlineId,
                OrderIndex = s.OrderIndex,
                HasGameLogic = s.HasGameLogic,
                Status = s.Status,
                CreatedAt = s.CreatedAt,
                LastModifiedAt = s.LastModifiedAt
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
            .Include(s => s.MetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (scene is null)
            return ApiResponseDto<SceneResponseDto>.NotFound($"Scene with ID {id} not found.");

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<SceneResponseDto>(scene.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<SceneResponseDto>.Success(new SceneResponseDto
        {
            Id = scene.Id,
            MetaInfoId = scene.MetaInfoId,
            MetaInfoTitle = scene.MetaInfo.Title,
            RawText = scene.RawText,
            StoryOutlineId = scene.StoryOutlineId,
            OrderIndex = scene.OrderIndex,
            HasGameLogic = scene.HasGameLogic,
            Status = scene.Status,
            CreatedAt = scene.CreatedAt,
            LastModifiedAt = scene.LastModifiedAt
        });
    }

    // ========================================================================
    // POST - Create a new scene
    // ========================================================================

    public async Task<ApiResponseDto<SceneCreateResponseDto>> CreateSceneAsync(Guid projectId, SceneCreateDto createDto)
    {
        // Validate project access (projectId from route matches CreateData.ProjectId)
        var error = await ValidateProjectAccessAsync<SceneCreateResponseDto>(projectId);
        if (error != null) return error;

        var user = _userContext.CurrentUser!;

        // Verify ProjectId in body matches route parameter
        if (createDto.CreateData.ProjectId != projectId)
            return ApiResponseDto<SceneCreateResponseDto>.BadRequest("Project ID in request body does not match route.");

        // Create MetaInfo first
        var metaInfo = new MetaInfo
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            ContentType = ContentTypeEnum.Scene,
            Title = createDto.CreateData.Title,
            Slug = string.IsNullOrWhiteSpace(createDto.CreateData.Slug)
                ? GenerateSlug(createDto.CreateData.Title)
                : createDto.CreateData.Slug,
            ShortDesc = createDto.CreateData.ShortDesc,
            Status = ContentStatusEnum.Draft,
            ViewMode = ViewModeEnum.PrivateWriting,
            CreatedByUserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow
        };

        _db.MetaInfos.Add(metaInfo);
        await _db.SaveChangesAsync();

        // Create the Scene entity
        var scene = new Scene
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id.Value,
            RawText = string.Empty,
            StoryOutlineId = createDto.StoryOutlineId,
            OrderIndex = createDto.OrderIndex ?? 0,
            HasGameLogic = false,
            Status = ContentStatusEnum.Draft,
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow
        };

        _db.Scenes.Add(scene);
        await _db.SaveChangesAsync();

        return ApiResponseDto<SceneCreateResponseDto>.Success(new SceneCreateResponseDto
        {
            Data = new CreateResponseDto
            {
                EntityId = scene.Id,
                MetaInfoId = metaInfo.Id.Value,
                ProjectId = projectId
            }
        });
    }

    // ========================================================================
    // PUT - Partial update of a scene
    // ========================================================================

    public async Task<ApiResponseDto<SceneResponseDto>> UpdateSceneAsync(Guid id, SceneUpdateDto updateDto)
    {
        var scene = await _db.Scenes
            .Include(s => s.MetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (scene is null)
            return ApiResponseDto<SceneResponseDto>.NotFound($"Scene with ID {id} not found.");

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<SceneResponseDto>(scene.MetaInfo.ProjectId);
        if (error != null) return error;

        // Apply only non-null fields (partial update)
        if (!string.IsNullOrWhiteSpace(updateDto.MetaInfo?.Title))
        {
            scene.MetaInfo.Title = updateDto.MetaInfo.Title;
        }

        if (!string.IsNullOrWhiteSpace(updateDto.MetaInfo?.Slug))
                scene.MetaInfo.Slug = updateDto.MetaInfo.Slug;

        if (updateDto.MetaInfo?.ShortDesc != null)
            scene.MetaInfo.ShortDesc = updateDto.MetaInfo.ShortDesc;

        if (updateDto.RawText != null)
            scene.RawText = updateDto.RawText;

        if (updateDto.StoryOutlineId.HasValue)
            scene.StoryOutlineId = updateDto.StoryOutlineId.Value;

        if (updateDto.OrderIndex.HasValue)
            scene.OrderIndex = updateDto.OrderIndex.Value;

        if (updateDto.HasGameLogic.HasValue)
            scene.HasGameLogic = updateDto.HasGameLogic.Value;

        if (updateDto.Status.HasValue)
            scene.Status = updateDto.Status.Value;

        scene.LastModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ApiResponseDto<SceneResponseDto>.Success(new SceneResponseDto
        {
            Id = scene.Id,
            MetaInfoId = scene.MetaInfoId,
            MetaInfoTitle = scene.MetaInfo.Title,
            RawText = scene.RawText,
            StoryOutlineId = scene.StoryOutlineId,
            OrderIndex = scene.OrderIndex,
            HasGameLogic = scene.HasGameLogic,
            Status = scene.Status,
            CreatedAt = scene.CreatedAt,
            LastModifiedAt = scene.LastModifiedAt
        });
    }

    // ========================================================================
    // DELETE - Remove a scene
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteSceneAsync(Guid id)
    {
        var scene = await _db.Scenes
            .Include(s => s.MetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (scene is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Scene with ID {id} not found.");

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(scene.MetaInfo.ProjectId);
        if (error != null) return error;

        // Delete MetaInfo first (FK dependency), then Scene
        _db.MetaInfos.Remove(scene.MetaInfo);
        _db.Scenes.Remove(scene);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = scene.MetaInfo.ProjectId
        });
    }
}

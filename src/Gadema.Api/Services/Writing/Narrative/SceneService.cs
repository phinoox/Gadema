using Gadema.Api.CoreServices;
using Gadema.Api.CoreServices.Strategies;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Writing.Narrative;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Enums;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Core.Models.Writing.Narrative;
using Gadema.Data.Database.Core;
using Gadema.Data.Database.Writing;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Writing.Narrative;

[ServiceLifetime(ServiceLifetime.Scoped)] public class SceneService : DomainService
{
    private WritingDbContext _db;

    public SceneService( WritingDbContext db,
        ILogger<SceneService> logger,  
        ICoreServicesProvider coreServices) 
        : base(coreServices,logger) { _db = db; }

    public async Task<ApiResponseDto<IEnumerable<SceneResponseDto>>> GetScenesAsync(Guid projectId)
    {
        var error = await CheckAccessAsync<IEnumerable<SceneResponseDto>>(projectId, Permission.CanView);
        if (error != null) return error;

        var scenes = await _db.Scenes
            .Include(s => s.ContentMetaInfo)
            .Where(s => s.ContentMetaInfo.ProjectId == projectId)
            .OrderBy(s => s.OrderIndex)
            .ToListAsync();

        return ApiResponseDto<IEnumerable<SceneResponseDto>>.Success(scenes.Select(CreateResponseDto));
    }

    public async Task<ApiResponseDto<SceneResponseDto>> GetSceneAsync(Guid id)
    {
        var scene = await _db.Scenes
            .Include(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (scene is null)
            return ApiResponseDto<SceneResponseDto>.NotFound($"Scene with ID {id} not found.");

        var error = await CheckAccessAsync<SceneResponseDto>(scene.ContentMetaInfo.ProjectId, Permission.CanView);
        if (error != null) return error;

        return ApiResponseDto<SceneResponseDto>.Success(CreateResponseDto(scene));
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateSceneAsync(Guid projectId, SceneCreateDto createDto)
    {
        var error = await CheckAccessAsync<CreateResponseDto>(projectId, Permission.CanEdit);
        if (error != null) return error;

        var contentMetaInfo = await _core.MetadataService.CreateAsync<ContentMetaInfo>(createDto.CreateData, m =>
        {
            m.ProjectId = projectId;
            m.ContentType = ContentTypeEnum.Scene;
        });

        // Law I: Unification - Body.Id == Soul.Id
        var scene = new Scene
        {
            Id = contentMetaInfo.Id, 
            MetaInfoId = contentMetaInfo.Id,
            RawText = string.Empty,
            StoryChapterId = createDto.StoryChapterId,
            OrderIndex = createDto.OrderIndex ?? 0,
        };

        _db.Scenes.Add(scene);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = scene.Id,
            MetaInfoId = contentMetaInfo.Id,
            ProjectId = projectId
        });
    }

    public async Task<ApiResponseDto<SceneResponseDto>> UpdateSceneAsync(Guid id, SceneUpdateDto updateDto)
    {
        var scene = await _db.Scenes
            .Include(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (scene is null)
            return ApiResponseDto<SceneResponseDto>.NotFound($"Scene with ID {id} not found.");

        var error = await CheckAccessAsync<SceneResponseDto>(scene.ContentMetaInfo.ProjectId, Permission.CanEdit);
        if (error != null) return error;

        if (updateDto.RawText != null)
            scene.RawText = updateDto.RawText;

        scene.StoryChapterId = updateDto.StoryChapterId;

        if (updateDto.OrderIndex.HasValue)
            scene.OrderIndex = updateDto.OrderIndex.Value;

        if (updateDto.ContentMetaInfo != null)
        {
            var success = await SyncIdentityAsync<ContentIdentityStrategy>(scene.MetaInfoId, updateDto.ContentMetaInfo);
            if (!success) return ApiResponseDto<SceneResponseDto>.ServerError("Sync failed.");
        }

        await _db.SaveChangesAsync();

        return ApiResponseDto<SceneResponseDto>.Success(CreateResponseDto(scene));
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteSceneAsync(Guid id)
    {
        var scene = await _db.Scenes
            .Include(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (scene is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Scene with ID {id} not found.");

        var error = await CheckAccessAsync<DeleteResponseDto>(scene.ContentMetaInfo.ProjectId, Permission.CanDelete);
        if (error != null) return error;

        _db.Set<ContentMetaInfo>().Remove(scene.ContentMetaInfo);
        _db.Scenes.Remove(scene);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = scene.ContentMetaInfo.ProjectId.Value
        });
    }

    private SceneResponseDto CreateResponseDto(Scene scene)
    {
        return new SceneResponseDto
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
        };
    }
}
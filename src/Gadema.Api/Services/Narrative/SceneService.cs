// src/Gadema.Api/Services/Narrative/SceneService.cs

using Gadema.Core.Dtos;
using Gadema.Core.Dtos.MetaInfos;
using Gadema.Core.Dtos.Narrative;
using Gadema.Core.Enums;
using Gadema.Core.Interfaces;
using Gadema.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Narrative;

public class SceneService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<SceneService> _logger;
    private readonly IUserContext _userContext;

    public SceneService(ApplicationDbContext db, ILogger<SceneService> logger, IUserContext userContext)
    {
        _db = db;
        _logger = logger;
        _userContext = userContext;
    }

    public async Task<ApiResponseDto<IEnumerable<SceneResponseDto>>> GetScenesAsync(Guid projectId)
    {
        var scenes = await _db.Scenes
            .Where(s => s.ProjectId == projectId)
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

    public async Task<ApiResponseDto<SceneResponseDto>> GetSceneAsync(Guid id)
    {
        var scene = await _db.Scenes
            .Include(s => s.MetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (scene is null)
            return ApiResponseDto<SceneResponseDto>.NotFound($"Scene with ID {id} not found.");

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

    public async Task<ApiResponseDto<SceneCreateResponseDto>> CreateSceneAsync(Guid projectId, SceneCreateDto createDto)
    {
        var user = _userContext.CurrentUser;
        if (user == null)
            return ApiResponseDto<SceneCreateResponseDto>.Unauthorized("Not authenticated.");

        // Validate project access
        var project = await _db.Projects.FindAsync(projectId);
        if (project is null || !project.IsActive)
            return ApiResponseDto<SceneCreateResponseDto>.NotFound($"Project with ID {projectId} not found.");

        // Create MetaInfo first
        var metaInfo = new MetaInfo
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            ContentType = ContentTypeEnum.Scene,
            Title = createDto.MetaInfo.Title,
            Slug = string.IsNullOrWhiteSpace(createDto.MetaInfo.Slug)
                ? GenerateSlug(createDto.MetaInfo.Title)
                : createDto.MetaInfo.Slug,
            ShortDesc = createDto.MetaInfo.ShortDesc,
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
            MetaInfoId = metaInfo.Id,
            RawText = string.Empty,
            StoryOutlineId = Guid.Empty,
            OrderIndex = createDto.OrderIndex,
            HasGameLogic = false,
            Status = ContentStatusEnum.Draft,
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow
        };

        _db.Scenes.Add(scene);
        await _db.SaveChangesAsync();

        return ApiResponseDto<SceneCreateResponseDto>.Success(new SceneCreateResponseDto
        {
            Id = scene.Id,
            MetaInfoId = metaInfo.Id,
            ProjectId = projectId
        });
    }

    public async Task<ApiResponseDto<SceneResponseDto>> UpdateSceneAsync(Guid id, SceneUpdateDto updateDto)
    {
        var user = _userContext.CurrentUser;
        if (user == null)
            return ApiResponseDto<SceneResponseDto>.Unauthorized("Not authenticated.");

        var scene = await _db.Scenes
            .Include(s => s.MetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (scene is null)
            return ApiResponseDto<SceneResponseDto>.NotFound($"Scene with ID {id} not found.");

        // Apply only non-null fields
        if (!string.IsNullOrWhiteSpace(updateDto.Title))
        {
            scene.MetaInfo.Title = updateDto.Title;
            if (!string.IsNullOrWhiteSpace(updateDto.Slug))
                scene.MetaInfo.Slug = updateDto.Slug;
        }

        if (updateDto.RawText != null)
            scene.RawText = updateDto.RawText;

        if (updateDto.StoryOutlineId.HasValue)
            scene.StoryOutlineId = updateDto.StoryOutlineId.Value;

        if (updateDto.OrderIndex != null)
            scene.OrderIndex = updateDto.OrderIndex.Value;

        if (updateDto.HasGameLogic != null)
            scene.HasGameLogic = updateDto.HasGameLogic.Value;

        if (updateDto.Status != null)
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

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteSceneAsync(Guid id)
    {
        var user = _userContext.CurrentUser;
        if (user == null)
            return ApiResponseDto<DeleteResponseDto>.Unauthorized("Not authenticated.");

        var scene = await _db.Scenes
            .Include(s => s.MetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (scene is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Scene with ID {id} not found.");

        // Delete MetaInfo first (cascade), then Scene
        _db.MetaInfos.Remove(scene.MetaInfo);
        _db.Scenes.Remove(scene);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = scene.ProjectId
        });
    }

    private static string GenerateSlug(string title)
    {
        var slug = title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-");
        
        foreach (var c in new[] { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')' })
            slug = slug.Replace(c.ToString(), string.Empty);

        return slug;
    }
}
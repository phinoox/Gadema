// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Narrative;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Models.Writing;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Narrative;

/// <summary>
/// Service for managing Stories within the narrative domain.
/// Handles CRUD operations including MetaInfo creation and authorization.
/// </summary>
public class StoryService : CoreService
{
    public StoryService(GameDbContext db, ILogger<StoryService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // GET - List all stories for a project
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<StoryResponseDto>>> GetStoriesAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<StoryResponseDto>>(projectId);
        if (error != null) return error;

        var stories = await _db.Stories
            .Include(s => s.MetaInfo)
            .Where(s => s.MetaInfo.ProjectId == projectId)
            .OrderBy(s => s.MetaInfo.Title)
            .Select(s => new StoryResponseDto
            {
                Id = s.Id,
                MetaInfoId = s.MetaInfoId,
                MetaInfoTitle = s.MetaInfo.Title,
                Status = s.MetaInfo.Status,
                IsPublic = s.MetaInfo.IsPublic,
                CreatedAt = s.MetaInfo.CreatedAt,
                LastModifiedAt = s.MetaInfo.LastModifiedAt,
                Description = s.Description
            })
            .ToListAsync();

        return ApiResponseDto<IEnumerable<StoryResponseDto>>.Success(stories);
    }

    // ========================================================================
    // GET - Single story by ID
    // ========================================================================

    public async Task<ApiResponseDto<StoryResponseDto>> GetStoryAsync(Guid id)
    {
        var story = await _db.Stories
            .Include(s => s.MetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (story is null)
            return ApiResponseDto<StoryResponseDto>.NotFound($"Story with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<StoryResponseDto>(story.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<StoryResponseDto>.Success(new StoryResponseDto
        {
            Id = story.Id,
            MetaInfoId = story.MetaInfoId,
            MetaInfoTitle = story.MetaInfo.Title,
            Status = story.MetaInfo.Status,
            IsPublic = story.MetaInfo.IsPublic,
            CreatedAt = story.MetaInfo.CreatedAt,
            LastModifiedAt = story.MetaInfo.LastModifiedAt,
            Description = story.Description
        });
    }

    // ========================================================================
    // POST - Create a new story
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateStoryAsync(Guid projectId, StoryCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        var metaInfo = CreateMetaInfo(projectId, ContentTypeEnum.StoryOutline, createDto.CreateData);

        _db.MetaInfos.Add(metaInfo);
        await _db.SaveChangesAsync();

        var story = new Story
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id,
            Name = createDto.CreateData.Title,
            Description = createDto.Description,
        };

        _db.Stories.Add(story);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = story.Id,
            MetaInfoId = metaInfo.Id,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a story
    // ========================================================================

    public async Task<ApiResponseDto<StoryResponseDto>> UpdateStoryAsync(Guid id, StoryUpdateDto updateDto)
    {
        var story = await _db.Stories
            .Include(s => s.MetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (story is null)
            return ApiResponseDto<StoryResponseDto>.NotFound($"Story with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<StoryResponseDto>(story.MetaInfo.ProjectId);
        if (error != null) return error;

        ApplyMetaInfoUpdates(story.MetaInfo, updateDto.MetaInfo);

        if (updateDto.Description != null)
            story.Description = updateDto.Description;

        await _db.SaveChangesAsync();

        return ApiResponseDto<StoryResponseDto>.Success(new StoryResponseDto
        {
            Id = story.Id,
            MetaInfoId = story.MetaInfoId,
            MetaInfoTitle = story.MetaInfo.Title,
            Status = story.MetaInfo.Status,
            IsPublic = story.MetaInfo.IsPublic,
            CreatedAt = story.MetaInfo.CreatedAt,
            LastModifiedAt = story.MetaInfo.LastModifiedAt,
            Description = story.Description
        });
    }

    // ========================================================================
    // DELETE - Remove a story
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteStoryAsync(Guid id)
    {
        var story = await _db.Stories
            .Include(s => s.MetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (story is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Story with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(story.MetaInfo.ProjectId);
        if (error != null) return error;

        _db.MetaInfos.Remove(story.MetaInfo);
        _db.Stories.Remove(story);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = story.MetaInfo.ProjectId
        });
    }
}

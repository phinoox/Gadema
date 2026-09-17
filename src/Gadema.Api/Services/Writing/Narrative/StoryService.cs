// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Narrative;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Models.Writing;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Writing.Narrative;

/// <summary>
/// Service for managing Stories within the narrative domain.
/// Handles CRUD operations including ContentMetaInfo creation and authorization.
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
            .Include(s => s.ContentMetaInfo)
            .Where(s => s.ContentMetaInfo.ProjectId == projectId)
            .OrderBy(s => s.ContentMetaInfo.Title)
            .Select(s => new StoryResponseDto
            {
                Id = s.Id,
                MetaInfoId = s.MetaInfoId,
                MetaInfoTitle = s.ContentMetaInfo.Title,
                Status = s.ContentMetaInfo.Status,
                IsPublic = s.ContentMetaInfo.IsPublic,
                CreatedAt = s.ContentMetaInfo.CreatedAt,
                LastModifiedAt = s.ContentMetaInfo.LastModifiedAt,
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
            .Include(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (story is null)
            return ApiResponseDto<StoryResponseDto>.NotFound($"Story with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<StoryResponseDto>(story.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<StoryResponseDto>.Success(new StoryResponseDto
        {
            Id = story.Id,
            MetaInfoId = story.MetaInfoId,
            MetaInfoTitle = story.ContentMetaInfo.Title,
            Status = story.ContentMetaInfo.Status,
            IsPublic = story.ContentMetaInfo.IsPublic,
            CreatedAt = story.ContentMetaInfo.CreatedAt,
            LastModifiedAt = story.ContentMetaInfo.LastModifiedAt,
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

        var ContentMetaInfo = CreateMetaInfo(projectId, ContentTypeEnum.StoryOutline, createDto.CreateData);

        _db.MetaInfos.Add(ContentMetaInfo);
        await _db.SaveChangesAsync();

        var story = new Story
        {
            Id = Guid.NewGuid(),
            MetaInfoId = ContentMetaInfo.Id,
            Name = createDto.CreateData.Title,
            Description = createDto.Description,
        };

        _db.Stories.Add(story);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = story.Id,
            MetaInfoId = ContentMetaInfo.Id,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a story
    // ========================================================================

    public async Task<ApiResponseDto<StoryResponseDto>> UpdateStoryAsync(Guid id, StoryUpdateDto updateDto)
    {
        var story = await _db.Stories
            .Include(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (story is null)
            return ApiResponseDto<StoryResponseDto>.NotFound($"Story with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<StoryResponseDto>(story.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        ApplyMetaInfoUpdates(story.ContentMetaInfo, updateDto.ContentMetaInfo);

        if (updateDto.Description != null)
            story.Description = updateDto.Description;

        await _db.SaveChangesAsync();

        return ApiResponseDto<StoryResponseDto>.Success(new StoryResponseDto
        {
            Id = story.Id,
            MetaInfoId = story.MetaInfoId,
            MetaInfoTitle = story.ContentMetaInfo.Title,
            Status = story.ContentMetaInfo.Status,
            IsPublic = story.ContentMetaInfo.IsPublic,
            CreatedAt = story.ContentMetaInfo.CreatedAt,
            LastModifiedAt = story.ContentMetaInfo.LastModifiedAt,
            Description = story.Description
        });
    }

    // ========================================================================
    // DELETE - Remove a story
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteStoryAsync(Guid id)
    {
        var story = await _db.Stories
            .Include(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (story is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Story with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(story.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        _db.MetaInfos.Remove(story.ContentMetaInfo);
        _db.Stories.Remove(story);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = story.ContentMetaInfo.ProjectId
        });
    }
}

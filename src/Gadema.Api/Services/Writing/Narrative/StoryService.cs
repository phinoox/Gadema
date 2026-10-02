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

[ServiceLifetime(ServiceLifetime.Scoped)] public class StoryService : DomainService
{
    private WritingDbContext _db;

    public StoryService( WritingDbContext db,
        ILogger<StoryService> logger,  
        CoreServicesProvider coreServices) 
        : base(coreServices,logger) { _db = db; }

    public async Task<ApiResponseDto<IEnumerable<StoryResponseDto>>> GetStoriesAsync(Guid projectId)
    {
        var error = await CheckAccessAsync<IEnumerable<StoryResponseDto>>(projectId, Permission.CanView);
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

    public async Task<ApiResponseDto<StoryResponseDto>> GetStoryAsync(Guid id)
    {
        var story = await _db.Stories
            .Include(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (story is null)
            return ApiResponseDto<StoryResponseDto>.NotFound($"Story with ID {id} not found.");

        var error = await CheckAccessAsync<StoryResponseDto>(story.ContentMetaInfo.ProjectId, Permission.CanView);
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

    public async Task<ApiResponseDto<CreateResponseDto>> CreateStoryAsync(Guid projectId, StoryCreateDto createDto)
    {
        var error = await CheckAccessAsync<CreateResponseDto>(projectId, Permission.CanEdit);
        if (error != null) return error;

        var contentMetaInfo = await _core.MetadataService.CreateAsync<ContentMetaInfo>(createDto.CreateData, m =>
        {
            m.ProjectId = projectId;
            m.ContentType = ContentTypeEnum.StoryOutline;
        });

        var story = new Story
        {
            Id = contentMetaInfo.Id, // Law I: Unification - Body.Id == Soul.Id
            MetaInfoId = contentMetaInfo.Id,
            Name = createDto.CreateData.Title,
            Description = createDto.Description,
        };

        _db.Stories.Add(story);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = story.Id,
            MetaInfoId = contentMetaInfo.Id,
            ProjectId = projectId
        });
    }

    public async Task<ApiResponseDto<StoryResponseDto>> UpdateStoryAsync(Guid id, StoryUpdateDto updateDto)
    {
        var story = await _db.Stories
            .Include(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (story is null)
            return ApiResponseDto<StoryResponseDto>.NotFound($"Story with ID {id} not found.");

        var error = await CheckAccessAsync<StoryResponseDto>(story.ContentMetaInfo.ProjectId, Permission.CanEdit);
        if (error != null) return error;

        if (updateDto.ContentMetaInfo != null)
        {
            var success = await SyncIdentityAsync<ContentIdentityStrategy>(story.MetaInfoId, updateDto.ContentMetaInfo);
            if (!success) return ApiResponseDto<StoryResponseDto>.ServerError("Sync failed.");
        }

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

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteStoryAsync(Guid id)
    {
        var story = await _db.Stories
            .Include(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (story is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Story with ID {id} not found.");

        var error = await CheckAccessAsync<DeleteResponseDto>(story.ContentMetaInfo.ProjectId, Permission.CanDelete);
        if (error != null) return error;

        _db.Stories.Remove(story);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = story.ContentMetaInfo.ProjectId.Value
        });
    }
}
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

/// <summary>
/// Service for managing StoryChapters within the narrative domain.
/// Handles CRUD operations including ContentMetaInfo creation and authorization.
/// </summary>
[ServiceLifetime(ServiceLifetime.Scoped)] public class StoryChapterService : DomainService
{
    private WritingDbContext _db;

    public StoryChapterService( WritingDbContext db,
        ILogger<StoryChapterService> logger,  
        CoreServicesProvider coreServices) // Injected via CoreService constructor
        : base(coreServices,logger) { _db = db; }

    // ========================================================================
    // GET - List all story chapters for a project
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<StoryChapterResponseDto>>> GetStoryChaptersAsync(Guid projectId)
    {
        var error = await CheckAccessAsync<IEnumerable<StoryChapterResponseDto>>(projectId, Permission.CanView);
        if (error != null) return error;

        var chapters = await _db.StoryChapters
            .Include(sc => sc.ContentMetaInfo)
            .Where(sc => sc.ContentMetaInfo.ProjectId == projectId)
            .OrderBy(sc => sc.OrderIndex)
            .Select(sc => new StoryChapterResponseDto
            {
                Id = sc.Id,
                MetaInfoId = sc.MetaInfoId,
                MetaInfoTitle = sc.ContentMetaInfo.Title,
                Status = sc.ContentMetaInfo.Status,
                IsPublic = sc.ContentMetaInfo.IsPublic,
                CreatedAt = sc.ContentMetaInfo.CreatedAt,
                LastModifiedAt = sc.ContentMetaInfo.LastModifiedAt,
                StoryId = sc.StoryId,
                Description = sc.Description,
                OrderIndex = sc.OrderIndex
            })
            .ToListAsync();

        return ApiResponseDto<IEnumerable<StoryChapterResponseDto>>.Success(chapters);
    }

    // ========================================================================
    // GET - Single story chapter by ID
    // ========================================================================

    public async Task<ApiResponseDto<StoryChapterResponseDto>> GetStoryChapterAsync(Guid id)
    {
        var chapter = await _db.StoryChapters
            .Include(sc => sc.ContentMetaInfo)
            .FirstOrDefaultAsync(sc => sc.Id == id);

        if (chapter is null)
            return ApiResponseDto<StoryChapterResponseDto>.NotFound($"Story chapter with ID {id} not found.");

        var error = await CheckAccessAsync<StoryChapterResponseDto>(chapter.ContentMetaInfo.ProjectId, Permission.CanView);
        if (error != null) return error;

        return ApiResponseDto<StoryChapterResponseDto>.Success(new StoryChapterResponseDto
        {
            Id = chapter.Id,
            MetaInfoId = chapter.MetaInfoId,
            MetaInfoTitle = chapter.ContentMetaInfo.Title,
            Status = chapter.ContentMetaInfo.Status,
            IsPublic = chapter.ContentMetaInfo.IsPublic,
            CreatedAt = chapter.ContentMetaInfo.CreatedAt,
            LastModifiedAt = chapter.ContentMetaInfo.LastModifiedAt,
            StoryId = chapter.StoryId,
            Description = chapter.Description,
            OrderIndex = chapter.OrderIndex
        });
    }

    // ========================================================================
    // POST - Create a new story chapter
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateStoryChapterAsync(Guid projectId, StoryChapterCreateDto createDto)
    {
        var error = await CheckAccessAsync<CreateResponseDto>(projectId, Permission.CanEdit);
        if (error != null) return error;

        var contentMetaInfo = await _core.MetadataService.CreateAsync<ContentMetaInfo>(createDto.CreateData, m =>
        {
            m.ProjectId = projectId;
            m.ContentType = ContentTypeEnum.StoryOutline;
        });

        var chapter = new StoryChapter
        {
            Id = Guid.NewGuid(),
            MetaInfoId = contentMetaInfo.Id,
            StoryId = createDto.StoryId,
            Description = createDto.Description,
            OrderIndex = createDto.OrderIndex ?? 0,
        };

        _db.StoryChapters.Add(chapter);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = chapter.Id,
            MetaInfoId = contentMetaInfo.Id,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a story chapter
    // ========================================================================

    public async Task<ApiResponseDto<StoryChapterResponseDto>> UpdateStoryChapterAsync(Guid id, StoryChapterUpdateDto updateDto)
    {
        var chapter = await _db.StoryChapters
            .Include(sc => sc.ContentMetaInfo)
            .FirstOrDefaultAsync(sc => sc.Id == id);

        if (chapter is null)
            return ApiResponseDto<StoryChapterResponseDto>.NotFound($"Story chapter with ID {id} not found.");

        var error = await CheckAccessAsync<StoryChapterResponseDto>(chapter.ContentMetaInfo.ProjectId, Permission.CanEdit);
        if (error != null) return error;

        if (updateDto.ContentMetaInfo != null)
        {
            var success = await SyncIdentityAsync<ContentIdentityStrategy>(chapter.MetaInfoId, updateDto.ContentMetaInfo);
            if (!success) return ApiResponseDto<StoryChapterResponseDto>.ServerError("Sync failed.");
        }

        if (updateDto.StoryId != Guid.Empty)
            chapter.StoryId = updateDto.StoryId;

        if (updateDto.Description != null)
            chapter.Description = updateDto.Description;

        if (updateDto.OrderIndex.HasValue)
            chapter.OrderIndex = updateDto.OrderIndex.Value;

        await _db.SaveChangesAsync();

        return ApiResponseDto<StoryChapterResponseDto>.Success(new StoryChapterResponseDto
        {
            Id = chapter.Id,
            MetaInfoId = chapter.MetaInfoId,
            MetaInfoTitle = chapter.ContentMetaInfo.Title,
            Status = chapter.ContentMetaInfo.Status,
            IsPublic = chapter.ContentMetaInfo.IsPublic,
            CreatedAt = chapter.ContentMetaInfo.CreatedAt,
            LastModifiedAt = chapter.ContentMetaInfo.LastModifiedAt,
            StoryId = chapter.StoryId,
            Description = chapter.Description,
            OrderIndex = chapter.OrderIndex
        });
    }

    // ========================================================================
    // DELETE - Remove a story chapter
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteStoryChapterAsync(Guid id)
    {
        var chapter = await _db.StoryChapters
            .Include(sc => sc.ContentMetaInfo)
            .FirstOrDefaultAsync(sc => sc.Id == id);

        if (chapter is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Story chapter with ID {id} not found.");

        var error = await CheckAccessAsync<DeleteResponseDto>(chapter.ContentMetaInfo.ProjectId, Permission.CanDelete);
        if (error != null) return error;

        //_db.MetaInfos.Remove(chapter.ContentMetaInfo);
        _db.StoryChapters.Remove(chapter);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = chapter.ContentMetaInfo.ProjectId.Value
        });
    }
}
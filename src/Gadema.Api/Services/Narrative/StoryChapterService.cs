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
/// Service for managing StoryChapters within the narrative domain.
/// Handles CRUD operations including MetaInfo creation and authorization.
/// </summary>
public class StoryChapterService : CoreService
{
    public StoryChapterService(GameDbContext db, ILogger<StoryChapterService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // GET - List all story chapters for a project
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<StoryChapterResponseDto>>> GetStoryChaptersAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<StoryChapterResponseDto>>(projectId);
        if (error != null) return error;

        var chapters = await _db.StorySequences
            .Include(sc => sc.MetaInfo)
            .Where(sc => sc.MetaInfo.ProjectId == projectId)
            .OrderBy(sc => sc.OrderIndex)
            .Select(sc => new StoryChapterResponseDto
            {
                Id = sc.Id,
                MetaInfoId = sc.MetaInfoId,
                MetaInfoTitle = sc.MetaInfo.Title,
                Status = sc.MetaInfo.Status,
                IsPublic = sc.MetaInfo.IsPublic,
                CreatedAt = sc.MetaInfo.CreatedAt,
                LastModifiedAt = sc.MetaInfo.LastModifiedAt,
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
        var chapter = await _db.StorySequences
            .Include(sc => sc.MetaInfo)
            .FirstOrDefaultAsync(sc => sc.Id == id);

        if (chapter is null)
            return ApiResponseDto<StoryChapterResponseDto>.NotFound($"Story chapter with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<StoryChapterResponseDto>(chapter.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<StoryChapterResponseDto>.Success(new StoryChapterResponseDto
        {
            Id = chapter.Id,
            MetaInfoId = chapter.MetaInfoId,
            MetaInfoTitle = chapter.MetaInfo.Title,
            Status = chapter.MetaInfo.Status,
            IsPublic = chapter.MetaInfo.IsPublic,
            CreatedAt = chapter.MetaInfo.CreatedAt,
            LastModifiedAt = chapter.MetaInfo.LastModifiedAt,
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
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        var metaInfo = CreateMetaInfo(projectId, ContentTypeEnum.StoryOutline, createDto.CreateData);

        _db.MetaInfos.Add(metaInfo);
        await _db.SaveChangesAsync();

        var chapter = new StoryChapter
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id,
            StoryId = createDto.StoryId,
            Description = createDto.Description,
            OrderIndex = createDto.OrderIndex ?? 0,
        };

        _db.StorySequences.Add(chapter);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = chapter.Id,
            MetaInfoId = metaInfo.Id,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a story chapter
    // ========================================================================

    public async Task<ApiResponseDto<StoryChapterResponseDto>> UpdateStoryChapterAsync(Guid id, StoryChapterUpdateDto updateDto)
    {
        var chapter = await _db.StorySequences
            .Include(sc => sc.MetaInfo)
            .FirstOrDefaultAsync(sc => sc.Id == id);

        if (chapter is null)
            return ApiResponseDto<StoryChapterResponseDto>.NotFound($"Story chapter with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<StoryChapterResponseDto>(chapter.MetaInfo.ProjectId);
        if (error != null) return error;

        ApplyMetaInfoUpdates(chapter.MetaInfo, updateDto.MetaInfo);

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
            MetaInfoTitle = chapter.MetaInfo.Title,
            Status = chapter.MetaInfo.Status,
            IsPublic = chapter.MetaInfo.IsPublic,
            CreatedAt = chapter.MetaInfo.CreatedAt,
            LastModifiedAt = chapter.MetaInfo.LastModifiedAt,
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
        var chapter = await _db.StorySequences
            .Include(sc => sc.MetaInfo)
            .FirstOrDefaultAsync(sc => sc.Id == id);

        if (chapter is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Story chapter with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(chapter.MetaInfo.ProjectId);
        if (error != null) return error;

        _db.MetaInfos.Remove(chapter.MetaInfo);
        _db.StorySequences.Remove(chapter);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = chapter.MetaInfo.ProjectId
        });
    }
}

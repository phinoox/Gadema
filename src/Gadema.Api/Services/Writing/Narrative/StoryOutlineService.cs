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
/// Service for managing StoryOutlines within the narrative domain.
/// Handles CRUD operations including ContentMetaInfo creation and authorization.
/// </summary>
public class StoryOutlineService : CoreService
{
    public StoryOutlineService(GameDbContext db, ILogger<StoryOutlineService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // GET - List all story outlines for a project
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<StoryOutlineResponseDto>>> GetStoryOutlinesAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<StoryOutlineResponseDto>>(projectId);
        if (error != null) return error;

        var outlines = await _db.StoryOutlines
            .Include(so => so.ContentMetaInfo)
            .Where(so => so.ContentMetaInfo.ProjectId == projectId)
            .OrderBy(so => so.ContentMetaInfo.Title)
            .Select(so => new StoryOutlineResponseDto
            {
                Id = so.Id,
                MetaInfoId = so.MetaInfoId,
                MetaInfoTitle = so.ContentMetaInfo.Title,
                Status = so.ContentMetaInfo.Status,
                IsPublic = so.ContentMetaInfo.IsPublic,
                CreatedAt = so.ContentMetaInfo.CreatedAt,
                LastModifiedAt = so.ContentMetaInfo.LastModifiedAt,
                Summary = so.Summary,
            })
            .ToListAsync();

        return ApiResponseDto<IEnumerable<StoryOutlineResponseDto>>.Success(outlines);
    }

    // ========================================================================
    // GET - Single story outline by ID
    // ========================================================================

    public async Task<ApiResponseDto<StoryOutlineResponseDto>> GetStoryOutlineAsync(Guid id)
    {
        var outline = await _db.StoryOutlines
            .Include(so => so.ContentMetaInfo)
            .FirstOrDefaultAsync(so => so.Id == id);

        if (outline is null)
            return ApiResponseDto<StoryOutlineResponseDto>.NotFound($"Story outline with ID {id} not found.");

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<StoryOutlineResponseDto>(outline.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<StoryOutlineResponseDto>.Success(new StoryOutlineResponseDto
        {
            Id = outline.Id,
            MetaInfoId = outline.MetaInfoId,
            MetaInfoTitle = outline.ContentMetaInfo.Title,
            Status = outline.ContentMetaInfo.Status,
            IsPublic = outline.ContentMetaInfo.IsPublic,
            CreatedAt = outline.ContentMetaInfo.CreatedAt,
            LastModifiedAt = outline.ContentMetaInfo.LastModifiedAt,
            Summary = outline.Summary,
        });
    }

    // ========================================================================
    // POST - Create a new story outline
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateStoryOutlineAsync(Guid projectId, StoryOutlineCreateDto createDto)
    {
        // Validate project access
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        // Create ContentMetaInfo using helper
        var ContentMetaInfo = CreateMetaInfo(projectId, ContentTypeEnum.StoryOutline, createDto.CreateData);

        _db.MetaInfos.Add(ContentMetaInfo);
        await _db.SaveChangesAsync();

        // Create the StoryOutline entity
        var outline = new StoryOutline
        {
            Id = Guid.NewGuid(),
            MetaInfoId = ContentMetaInfo.Id,
            Summary = createDto.Summary,
        };

        _db.StoryOutlines.Add(outline);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = outline.Id,
            MetaInfoId = ContentMetaInfo.Id,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a story outline
    // ========================================================================

    public async Task<ApiResponseDto<StoryOutlineResponseDto>> UpdateStoryOutlineAsync(Guid id, StoryOutlineUpdateDto updateDto)
    {
        var outline = await _db.StoryOutlines
            .Include(so => so.ContentMetaInfo)
            .FirstOrDefaultAsync(so => so.Id == id);

        if (outline is null)
            return ApiResponseDto<StoryOutlineResponseDto>.NotFound($"Story outline with ID {id} not found.");

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<StoryOutlineResponseDto>(outline.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        // Apply ContentMetaInfo updates via helper (replaces manual if-blocks)
        ApplyMetaInfoUpdates(outline.ContentMetaInfo, updateDto.ContentMetaInfo);


        if (updateDto.Summary != null)
            outline.Summary = updateDto.Summary;


        await _db.SaveChangesAsync();

        return ApiResponseDto<StoryOutlineResponseDto>.Success(new StoryOutlineResponseDto
        {
            Id = outline.Id,
            MetaInfoId = outline.MetaInfoId,
            MetaInfoTitle = outline.ContentMetaInfo.Title,
            Summary = outline.Summary,
            CreatedAt = outline.ContentMetaInfo.CreatedAt,
            LastModifiedAt = outline.ContentMetaInfo.LastModifiedAt
        });
    }

    // ========================================================================
    // DELETE - Remove a story outline
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteStoryOutlineAsync(Guid id)
    {
        var outline = await _db.StoryOutlines
            .Include(so => so.ContentMetaInfo)
            .FirstOrDefaultAsync(so => so.Id == id);

        if (outline is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Story outline with ID {id} not found.");

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(outline.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        // Delete ContentMetaInfo first (FK dependency), then StoryOutline
        _db.MetaInfos.Remove(outline.ContentMetaInfo);
        _db.StoryOutlines.Remove(outline);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = outline.ContentMetaInfo.ProjectId
        });
    }
}

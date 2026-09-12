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
/// Service for managing StoryOutlines within the narrative domain.
/// Handles CRUD operations including MetaInfo creation and authorization.
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
            .Include(so => so.MetaInfo)
            .Where(so => so.MetaInfo.ProjectId == projectId)
            .OrderBy(so => so.MetaInfo.Title)
            .Select(so => new StoryOutlineResponseDto
            {
                Id = so.Id,
                MetaInfoId = so.MetaInfoId,
                MetaInfoTitle = so.MetaInfo.Title,
                Status = so.MetaInfo.Status,
                IsPublic = so.MetaInfo.IsPublic,
                CreatedAt = so.MetaInfo.CreatedAt,
                LastModifiedAt = so.MetaInfo.LastModifiedAt,
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
            .Include(so => so.MetaInfo)
            .FirstOrDefaultAsync(so => so.Id == id);

        if (outline is null)
            return ApiResponseDto<StoryOutlineResponseDto>.NotFound($"Story outline with ID {id} not found.");

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<StoryOutlineResponseDto>(outline.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<StoryOutlineResponseDto>.Success(new StoryOutlineResponseDto
        {
            Id = outline.Id,
            MetaInfoId = outline.MetaInfoId,
            MetaInfoTitle = outline.MetaInfo.Title,
            Status = outline.MetaInfo.Status,
            IsPublic = outline.MetaInfo.IsPublic,
            CreatedAt = outline.MetaInfo.CreatedAt,
            LastModifiedAt = outline.MetaInfo.LastModifiedAt,
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

        // Create MetaInfo using helper
        var metaInfo = CreateMetaInfo(projectId, ContentTypeEnum.StoryOutline, createDto.CreateData);

        _db.MetaInfos.Add(metaInfo);
        await _db.SaveChangesAsync();

        // Create the StoryOutline entity
        var outline = new StoryOutline
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id,
            Summary = createDto.Summary,
        };

        _db.StoryOutlines.Add(outline);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = outline.Id,
            MetaInfoId = metaInfo.Id,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a story outline
    // ========================================================================

    public async Task<ApiResponseDto<StoryOutlineResponseDto>> UpdateStoryOutlineAsync(Guid id, StoryOutlineUpdateDto updateDto)
    {
        var outline = await _db.StoryOutlines
            .Include(so => so.MetaInfo)
            .FirstOrDefaultAsync(so => so.Id == id);

        if (outline is null)
            return ApiResponseDto<StoryOutlineResponseDto>.NotFound($"Story outline with ID {id} not found.");

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<StoryOutlineResponseDto>(outline.MetaInfo.ProjectId);
        if (error != null) return error;

        // Apply MetaInfo updates via helper (replaces manual if-blocks)
        ApplyMetaInfoUpdates(outline.MetaInfo, updateDto.MetaInfo);


        if (updateDto.Summary != null)
            outline.Summary = updateDto.Summary;


        await _db.SaveChangesAsync();

        return ApiResponseDto<StoryOutlineResponseDto>.Success(new StoryOutlineResponseDto
        {
            Id = outline.Id,
            MetaInfoId = outline.MetaInfoId,
            MetaInfoTitle = outline.MetaInfo.Title,
            Summary = outline.Summary,
            CreatedAt = outline.MetaInfo.CreatedAt,
            LastModifiedAt = outline.MetaInfo.LastModifiedAt
        });
    }

    // ========================================================================
    // DELETE - Remove a story outline
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteStoryOutlineAsync(Guid id)
    {
        var outline = await _db.StoryOutlines
            .Include(so => so.MetaInfo)
            .FirstOrDefaultAsync(so => so.Id == id);

        if (outline is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Story outline with ID {id} not found.");

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(outline.MetaInfo.ProjectId);
        if (error != null) return error;

        // Delete MetaInfo first (FK dependency), then StoryOutline
        _db.MetaInfos.Remove(outline.MetaInfo);
        _db.StoryOutlines.Remove(outline);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = outline.MetaInfo.ProjectId
        });
    }
}

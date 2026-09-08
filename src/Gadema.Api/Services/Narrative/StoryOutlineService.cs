// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Narrative;
using Gadema.Core.Enums;
using Gadema.Core.Models;
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
                RawText = so.RawText,
                Summary = so.Summary,
                CharacterSnapshot = so.CharacterSnapshot,
                ThemeStatement = so.ThemeStatement,
                OutlineStatus = so.OutlineStatus,
                CreatedAt = so.MetaInfo.CreatedAt,
                LastModifiedAt = so.MetaInfo.LastModifiedAt
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
            RawText = outline.RawText,
            Summary = outline.Summary,
            CharacterSnapshot = outline.CharacterSnapshot,
            ThemeStatement = outline.ThemeStatement,
            OutlineStatus = outline.OutlineStatus,
            CreatedAt = outline.MetaInfo.CreatedAt,
            LastModifiedAt = outline.MetaInfo.LastModifiedAt
        });
    }

    // ========================================================================
    // POST - Create a new story outline
    // ========================================================================

    public async Task<ApiResponseDto<StoryOutlineCreateResponseDto>> CreateStoryOutlineAsync(Guid projectId, StoryOutlineCreateDto createDto)
    {
        // Validate project access
        var error = await ValidateProjectAccessAsync<StoryOutlineCreateResponseDto>(projectId);
        if (error != null) return error;

        var user = _userContext.CurrentUser!;

        // Verify ProjectId in body matches route parameter
        if (createDto.CreateData.ProjectId != projectId)
            return ApiResponseDto<StoryOutlineCreateResponseDto>.BadRequest("Project ID in request body does not match route.");

        // Create MetaInfo first
        var metaInfo = new MetaInfo
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            ContentType = ContentTypeEnum.StoryOutline,
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

        // Create the StoryOutline entity
        var outline = new StoryOutline
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id.Value,
            RawText = string.Empty,
            Summary = createDto.Summary,
            CharacterSnapshot = createDto.CharacterSnapshot,
            ThemeStatement = createDto.ThemeStatement,
            OutlineStatus = createDto.OutlineStatus ?? OutlineStatusEnum.DraftOutline,
        };

        _db.StoryOutlines.Add(outline);
        await _db.SaveChangesAsync();

        return ApiResponseDto<StoryOutlineCreateResponseDto>.Success(new StoryOutlineCreateResponseDto
        {
            Data = new CreateResponseDto
            {
                EntityId = outline.Id,
                MetaInfoId = metaInfo.Id.Value,
                ProjectId = projectId
            }
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

        // Apply only non-null fields (partial update)
        if (updateDto.MetaInfo != null)
        {
            if (!string.IsNullOrWhiteSpace(updateDto.MetaInfo.Title))
                outline.MetaInfo.Title = updateDto.MetaInfo.Title;

            if (!string.IsNullOrWhiteSpace(updateDto.MetaInfo.Slug))
                outline.MetaInfo.Slug = updateDto.MetaInfo.Slug;

            if (updateDto.MetaInfo.ShortDesc != null)
                outline.MetaInfo.ShortDesc = updateDto.MetaInfo.ShortDesc;
        }

        if (updateDto.RawText != null)
            outline.RawText = updateDto.RawText;

        if (updateDto.Summary != null)
            outline.Summary = updateDto.Summary;

        if (updateDto.CharacterSnapshot != null)
            outline.CharacterSnapshot = updateDto.CharacterSnapshot;

        if (updateDto.ThemeStatement != null)
            outline.ThemeStatement = updateDto.ThemeStatement;

        if (updateDto.OutlineStatus.HasValue)
            outline.OutlineStatus = updateDto.OutlineStatus.Value;

        await _db.SaveChangesAsync();

        return ApiResponseDto<StoryOutlineResponseDto>.Success(new StoryOutlineResponseDto
        {
            Id = outline.Id,
            MetaInfoId = outline.MetaInfoId,
            MetaInfoTitle = outline.MetaInfo.Title,
            RawText = outline.RawText,
            Summary = outline.Summary,
            CharacterSnapshot = outline.CharacterSnapshot,
            ThemeStatement = outline.ThemeStatement,
            OutlineStatus = outline.OutlineStatus,
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

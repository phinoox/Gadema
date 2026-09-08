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
/// Service for managing LoreEntries within the narrative domain.
/// Handles CRUD operations including MetaInfo creation and authorization.
/// </summary>
public class LoreEntryService : CoreService
{
    public LoreEntryService(GameDbContext db, ILogger<LoreEntryService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // GET - List all lore entries for a project
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<LoreEntryResponseDto>>> GetLoreEntriesAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<LoreEntryResponseDto>>(projectId);
        if (error != null) return error;

        var loreEntries = await _db.LoreEntries
            .Include(le => le.MetaInfo)
            .Where(le => le.MetaInfo.ProjectId == projectId)
            .OrderBy(le => le.MetaInfo.Title)
            .Select(le => new LoreEntryResponseDto
            {
                Id = le.Id,
                MetaInfoId = le.MetaInfoId,
                MetaInfoTitle = le.MetaInfo.Title,
                LoreType = (int)le.LoreType,
                RawText = le.RawText,
                Published = le.Published,
                CreatedAt = le.MetaInfo.CreatedAt,
                LastModifiedAt = le.MetaInfo.LastModifiedAt
            })
            .ToListAsync();

        return ApiResponseDto<IEnumerable<LoreEntryResponseDto>>.Success(loreEntries);
    }

    // ========================================================================
    // GET - Single lore entry by ID
    // ========================================================================

    public async Task<ApiResponseDto<LoreEntryResponseDto>> GetLoreEntryAsync(Guid id)
    {
        var loreEntry = await _db.LoreEntries
            .Include(le => le.MetaInfo)
            .FirstOrDefaultAsync(le => le.Id == id);

        if (loreEntry is null)
            return ApiResponseDto<LoreEntryResponseDto>.NotFound($"Lore entry with ID {id} not found.");

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<LoreEntryResponseDto>(loreEntry.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<LoreEntryResponseDto>.Success(new LoreEntryResponseDto
        {
            Id = loreEntry.Id,
            MetaInfoId = loreEntry.MetaInfoId,
            MetaInfoTitle = loreEntry.MetaInfo.Title,
            LoreType = (int)loreEntry.LoreType,
            RawText = loreEntry.RawText,
            Published = loreEntry.Published,
            CreatedAt = loreEntry.MetaInfo.CreatedAt,
            LastModifiedAt = loreEntry.MetaInfo.LastModifiedAt
        });
    }

    // ========================================================================
    // POST - Create a new lore entry
    // ========================================================================

    public async Task<ApiResponseDto<LoreEntryCreateResponseDto>> CreateLoreEntryAsync(Guid projectId, LoreEntryCreateDto createDto)
    {
        // Validate project access
        var error = await ValidateProjectAccessAsync<LoreEntryCreateResponseDto>(projectId);
        if (error != null) return error;

        var user = _userContext.CurrentUser!;

        // Verify ProjectId in body matches route parameter
        if (createDto.CreateData.ProjectId != projectId)
            return ApiResponseDto<LoreEntryCreateResponseDto>.BadRequest("Project ID in request body does not match route.");

        // Create MetaInfo first
        var metaInfo = new MetaInfo
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            ContentType = ContentTypeEnum.LoreEntry,
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

        // Create the LoreEntry entity
        var loreEntry = new LoreEntry
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id.Value,
            RawText = string.Empty,
            LoreType = createDto.LoreType,
            Published = false,
        };

        _db.LoreEntries.Add(loreEntry);
        await _db.SaveChangesAsync();

        return ApiResponseDto<LoreEntryCreateResponseDto>.Success(new LoreEntryCreateResponseDto
        {
            Data = new CreateResponseDto
            {
                EntityId = loreEntry.Id,
                MetaInfoId = metaInfo.Id.Value,
                ProjectId = projectId
            }
        });
    }

    // ========================================================================
    // PUT - Partial update of a lore entry
    // ========================================================================

    public async Task<ApiResponseDto<LoreEntryResponseDto>> UpdateLoreEntryAsync(Guid id, LoreEntryUpdateDto updateDto)
    {
        var loreEntry = await _db.LoreEntries
            .Include(le => le.MetaInfo)
            .FirstOrDefaultAsync(le => le.Id == id);

        if (loreEntry is null)
            return ApiResponseDto<LoreEntryResponseDto>.NotFound($"Lore entry with ID {id} not found.");

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<LoreEntryResponseDto>(loreEntry.MetaInfo.ProjectId);
        if (error != null) return error;

        // Apply only non-null fields (partial update)
        if (updateDto.MetaInfo != null)
        {
            if (!string.IsNullOrWhiteSpace(updateDto.MetaInfo.Title))
                loreEntry.MetaInfo.Title = updateDto.MetaInfo.Title;

            if (!string.IsNullOrWhiteSpace(updateDto.MetaInfo.Slug))
                loreEntry.MetaInfo.Slug = updateDto.MetaInfo.Slug;

            if (updateDto.MetaInfo.ShortDesc != null)
                loreEntry.MetaInfo.ShortDesc = updateDto.MetaInfo.ShortDesc;
        }

        if (updateDto.RawText != null)
            loreEntry.RawText = updateDto.RawText;

        if (updateDto.Published.HasValue)
            loreEntry.Published = updateDto.Published.Value;

        loreEntry.MetaInfo.LastModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ApiResponseDto<LoreEntryResponseDto>.Success(new LoreEntryResponseDto
        {
            Id = loreEntry.Id,
            MetaInfoId = loreEntry.MetaInfoId,
            MetaInfoTitle = loreEntry.MetaInfo.Title,
            LoreType = (int)loreEntry.LoreType,
            RawText = loreEntry.RawText,
            Published = loreEntry.Published,
            CreatedAt = loreEntry.MetaInfo.CreatedAt,
            LastModifiedAt = loreEntry.MetaInfo.LastModifiedAt
        });
    }

    // ========================================================================
    // DELETE - Remove a lore entry
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteLoreEntryAsync(Guid id)
    {
        var loreEntry = await _db.LoreEntries
            .Include(le => le.MetaInfo)
            .FirstOrDefaultAsync(le => le.Id == id);

        if (loreEntry is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Lore entry with ID {id} not found.");

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(loreEntry.MetaInfo.ProjectId);
        if (error != null) return error;

        // Delete MetaInfo first (FK dependency), then LoreEntry
        _db.MetaInfos.Remove(loreEntry.MetaInfo);
        _db.LoreEntries.Remove(loreEntry);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = loreEntry.MetaInfo.ProjectId
        });
    }
}

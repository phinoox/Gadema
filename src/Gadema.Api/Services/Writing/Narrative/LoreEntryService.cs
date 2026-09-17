// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Narrative;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Writing.Narrative;

/// <summary>
/// Service for managing LoreEntries within the narrative domain.
/// Handles CRUD operations including ContentMetaInfo creation and authorization.
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
            .Include(le => le.ContentMetaInfo)
            .Where(le => le.ContentMetaInfo.ProjectId == projectId)
            .OrderBy(le => le.ContentMetaInfo.Title)
            .Select(le => new LoreEntryResponseDto
            {
                Id = le.Id,
                MetaInfoId = le.MetaInfoId,
                MetaInfoTitle = le.ContentMetaInfo.Title,
                Status = le.ContentMetaInfo.Status,
                IsPublic = le.ContentMetaInfo.IsPublic,
                CreatedAt = le.ContentMetaInfo.CreatedAt,
                LastModifiedAt = le.ContentMetaInfo.LastModifiedAt,
                LoreType = (int)le.LoreType,
                RawText = le.RawText
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
            .Include(le => le.ContentMetaInfo)
            .FirstOrDefaultAsync(le => le.Id == id);

        if (loreEntry is null)
            return ApiResponseDto<LoreEntryResponseDto>.NotFound($"Lore entry with ID {id} not found.");

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<LoreEntryResponseDto>(loreEntry.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<LoreEntryResponseDto>.Success(new LoreEntryResponseDto
        {
            Id = loreEntry.Id,
            MetaInfoId = loreEntry.MetaInfoId,
            MetaInfoTitle = loreEntry.ContentMetaInfo.Title,
            Status = loreEntry.ContentMetaInfo.Status,
            IsPublic = loreEntry.ContentMetaInfo.IsPublic,
            CreatedAt = loreEntry.ContentMetaInfo.CreatedAt,
            LastModifiedAt = loreEntry.ContentMetaInfo.LastModifiedAt,
            LoreType = (int)loreEntry.LoreType,
            RawText = loreEntry.RawText
        });
    }

    // ========================================================================
    // POST - Create a new lore entry
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateLoreEntryAsync(Guid projectId, LoreEntryCreateDto createDto)
    {
        // Validate project access
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        
        // Create ContentMetaInfo using helper
        var ContentMetaInfo = CreateMetaInfo(projectId, ContentTypeEnum.LoreEntry, createDto.CreateData);

        _db.MetaInfos.Add(ContentMetaInfo);
        await _db.SaveChangesAsync();

        // Create the LoreEntry entity
        var loreEntry = new LoreEntry
        {
            Id = Guid.NewGuid(),
            MetaInfoId = ContentMetaInfo.Id,
            RawText = string.Empty,
            LoreType = createDto.LoreType,
        };

        _db.LoreEntries.Add(loreEntry);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = loreEntry.Id,
            MetaInfoId = ContentMetaInfo.Id,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a lore entry
    // ========================================================================

    public async Task<ApiResponseDto<LoreEntryResponseDto>> UpdateLoreEntryAsync(Guid id, LoreEntryUpdateDto updateDto)
    {
        var loreEntry = await _db.LoreEntries
            .Include(le => le.ContentMetaInfo)
            .FirstOrDefaultAsync(le => le.Id == id);

        if (loreEntry is null)
            return ApiResponseDto<LoreEntryResponseDto>.NotFound($"Lore entry with ID {id} not found.");

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<LoreEntryResponseDto>(loreEntry.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        // Apply ContentMetaInfo updates via helper (replaces manual if-blocks)
        ApplyMetaInfoUpdates(loreEntry.ContentMetaInfo, updateDto.ContentMetaInfo);

        if (updateDto.RawText != null)
            loreEntry.RawText = updateDto.RawText;


        loreEntry.ContentMetaInfo.LastModifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        return ApiResponseDto<LoreEntryResponseDto>.Success(new LoreEntryResponseDto
        {
            Id = loreEntry.Id,
            MetaInfoId = loreEntry.MetaInfoId,
            MetaInfoTitle = loreEntry.ContentMetaInfo.Title,
            Status = loreEntry.ContentMetaInfo.Status,
            IsPublic = loreEntry.ContentMetaInfo.IsPublic,
            CreatedAt = loreEntry.ContentMetaInfo.CreatedAt,
            LastModifiedAt = loreEntry.ContentMetaInfo.LastModifiedAt,
            LoreType = (int)loreEntry.LoreType,
            RawText = loreEntry.RawText
        });
    }

    // ========================================================================
    // DELETE - Remove a lore entry
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteLoreEntryAsync(Guid id)
    {
        var loreEntry = await _db.LoreEntries
            .Include(le => le.ContentMetaInfo)
            .FirstOrDefaultAsync(le => le.Id == id);

        if (loreEntry is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Lore entry with ID {id} not found.");

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(loreEntry.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        // Delete ContentMetaInfo first (FK dependency), then LoreEntry
        _db.MetaInfos.Remove(loreEntry.ContentMetaInfo);
        _db.LoreEntries.Remove(loreEntry);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = loreEntry.ContentMetaInfo.ProjectId
        });
    }
}

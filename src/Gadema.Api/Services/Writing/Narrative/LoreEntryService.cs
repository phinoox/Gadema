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
/// Service for managing LoreEntries within the narrative domain.
/// Handles CRUD operations including ContentMetaInfo creation and authorization.
/// </summary>
[ServiceLifetime(ServiceLifetime.Scoped)] public class LoreEntryService : DomainService
{
    private WritingDbContext _db;

    public LoreEntryService( WritingDbContext db,
        ILogger<LoreEntryService> logger,  
        CoreServicesProvider coreServices) // Injected via CoreService constructor
        : base(coreServices,logger) { _db = db; }

    // ========================================================================
    // GET - List all lore entries for a project
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<LoreEntryResponseDto>>> GetLoreEntriesAsync(Guid projectId)
    {
        var error = await CheckAccessAsync<IEnumerable<LoreEntryResponseDto>>(projectId, Permission.CanView);
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

        var error = await CheckAccessAsync<LoreEntryResponseDto>(loreEntry.ContentMetaInfo.ProjectId, Permission.CanView);
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
        var error = await CheckAccessAsync<CreateResponseDto>(projectId, Permission.CanEdit);
        if (error != null) return error;

        // Create ContentMetaInfo using helper
        var contentMetaInfo = await _core.MetadataService.CreateAsync<ContentMetaInfo>(createDto.CreateData, m =>
        {
            m.ProjectId = projectId;
            m.ContentType = ContentTypeEnum.LoreEntry;
        });

        // Create the LoreEntry entity
        var loreEntry = new LoreEntry
        {
            Id = Guid.NewGuid(),
            MetaInfoId = contentMetaInfo.Id,
            RawText = string.Empty,
            LoreType = createDto.LoreType,
        };

        _db.LoreEntries.Add(loreEntry);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = loreEntry.Id,
            MetaInfoId = contentMetaInfo.Id,
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

        var error = await CheckAccessAsync<LoreEntryResponseDto>(loreEntry.ContentMetaInfo.ProjectId, Permission.CanEdit);
        if (error != null) return error;

        // Apply ContentMetaInfo updates via helper
        if (updateDto.ContentMetaInfo != null)
        {
            var success = await SyncIdentityAsync<ContentIdentityStrategy>(loreEntry.MetaInfoId, updateDto.ContentMetaInfo);
            if (!success) return ApiResponseDto<LoreEntryResponseDto>.ServerError("Sync failed.");
        }

        if (updateDto.RawText != null)
            loreEntry.RawText = updateDto.RawText;

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

        var error = await CheckAccessAsync<DeleteResponseDto>(loreEntry.ContentMetaInfo.ProjectId, Permission.CanDelete);
        if (error != null) return error;

        // Delete ContentMetaInfo first (FK dependency), then LoreEntry
        await _core.MetadataService.DeleteAsync<ContentMetaInfo>(loreEntry.ContentMetaInfo.Id);
        _db.LoreEntries.Remove(loreEntry);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = loreEntry.ContentMetaInfo.ProjectId.Value
        });
    }
}
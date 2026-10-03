// =============================================================================
using Gadema.Api.CoreServices;
using Gadema.Api.CoreServices.Strategies;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Writing.Narrative;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Enums;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Core.Models.Writing.Narrative;
using Gadema.Data.Database;
using Gadema.Data.Database.Writing;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Writing.Narrative;

/// <summary>
/// Service for managing StoryOutlines within the narrative domain.
/// Handles CRUD operations including ContentMetaInfo creation and authorization.
/// </summary>
[ServiceLifetime(ServiceLifetime.Scoped)] public class StoryOutlineService : DomainService
{
    private WritingDbContext _db;

    public StoryOutlineService( WritingDbContext db,
        ILogger<StoryOutlineService> logger,  
        ICoreServicesProvider coreServices) // Injected via CoreService constructor
        : base(coreServices,logger) { _db = db; }

    // ========================================================================
    // GET - List all story outlines for a project
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<StoryOutlineResponseDto>>> GetStoryOutlinesAsync(Guid projectId)
    {
        var error = await CheckAccessAsync<IEnumerable<StoryOutlineResponseDto>>(projectId, Permission.CanView);
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
        var error = await CheckAccessAsync<StoryOutlineResponseDto>(outline.ContentMetaInfo.ProjectId, Permission.CanView);
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
        var error = await CheckAccessAsync<CreateResponseDto>(projectId, Permission.CanEdit);
        if (error != null) return error;

        // Create ContentMetaInfo using helper
        var contentMetaInfo = await _core.MetadataService.CreateAsync<ContentMetaInfo>(createDto.CreateData, m =>
        {
            m.ProjectId = projectId;
            m.ContentType = ContentTypeEnum.StoryOutline;
        });

        // Create the StoryOutline entity
        var outline = new StoryOutline
        {
            Id = Guid.NewGuid(),
            MetaInfoId = contentMetaInfo.Id,
            Summary = createDto.Summary,
        };

        _db.StoryOutlines.Add(outline);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = outline.Id,
            MetaInfoId = contentMetaInfo.Id,
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
        var error = await CheckAccessAsync<StoryOutlineResponseDto>(outline.ContentMetaInfo.ProjectId, Permission.CanEdit);
        if (error != null) return error;

        // Apply ContentMetaInfo updates via helper (replaces manual if-blocks)
        if (updateDto.ContentMetaInfo != null)
        {
            var success = await SyncIdentityAsync<ContentIdentityStrategy>(outline.MetaInfoId, updateDto.ContentMetaInfo);
            if (!success) return ApiResponseDto<StoryOutlineResponseDto>.ServerError("Sync failed.");
        }


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
        var error = await CheckAccessAsync<DeleteResponseDto>(outline.ContentMetaInfo.ProjectId, Permission.CanDelete);
        if (error != null) return error;

        // Delete ContentMetaInfo first (FK dependency), then StoryOutline
        
        _db.StoryOutlines.Remove(outline);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = outline.ContentMetaInfo.ProjectId.Value
        });
    }
}

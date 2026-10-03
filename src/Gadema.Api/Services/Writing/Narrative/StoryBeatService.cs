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
/// Service for managing StoryBeats within the narrative domain.
/// Handles CRUD operations including ContentMetaInfo creation and authorization.
/// </summary>
[ServiceLifetime(ServiceLifetime.Scoped)] public class StoryBeatService : DomainService
{
    private WritingDbContext _db;

    public StoryBeatService( WritingDbContext db,
        ILogger<StoryBeatService> logger,  
        ICoreServicesProvider coreServices) // Injected via CoreService constructor
        : base(coreServices,logger) { _db = db; }

    // ========================================================================
    // GET - List all story beats for a project
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<StoryBeatResponseDto>>> GetStoryBeatsAsync(Guid projectId)
    {
        var error = await CheckAccessAsync<IEnumerable<StoryBeatResponseDto>>(projectId, Permission.CanView);
        if (error != null) return error;

        var beats = await _db.StoryBeats
            .Include(sb => sb.ContentMetaInfo)
            .Where(sb => sb.ContentMetaInfo.ProjectId == projectId)
            .OrderBy(sb => sb.OrderIndex)
            .Select(sb => new StoryBeatResponseDto
            {
                Id = sb.Id,
                MetaInfoId = sb.MetaInfoId,
                MetaInfoTitle = sb.ContentMetaInfo.Title,
                Status = sb.ContentMetaInfo.Status,
                IsPublic = sb.ContentMetaInfo.IsPublic,
                CreatedAt = sb.ContentMetaInfo.CreatedAt,
                LastModifiedAt = sb.ContentMetaInfo.LastModifiedAt,
                StoryId = sb.StoryId,
                Description = sb.Description,
                OrderIndex = sb.OrderIndex
            })
            .ToListAsync();

        return ApiResponseDto<IEnumerable<StoryBeatResponseDto>>.Success(beats);
    }

    // ========================================================================
    // GET - Single story beat by ID
    // ========================================================================

    public async Task<ApiResponseDto<StoryBeatResponseDto>> GetStoryBeatAsync(Guid id)
    {
        var beat = await _db.StoryBeats
            .Include(sb => sb.ContentMetaInfo)
            .FirstOrDefaultAsync(sb => sb.Id == id);

        if (beat is null)
            return ApiResponseDto<StoryBeatResponseDto>.NotFound($"Story beat with ID {id} not found.");

        var error = await CheckAccessAsync<StoryBeatResponseDto>(beat.ContentMetaInfo.ProjectId, Permission.CanView);
        if (error != null) return error;

        return ApiResponseDto<StoryBeatResponseDto>.Success(new StoryBeatResponseDto
        {
            Id = beat.Id,
            MetaInfoId = beat.MetaInfoId,
            MetaInfoTitle = beat.ContentMetaInfo.Title,
            Status = beat.ContentMetaInfo.Status,
            IsPublic = beat.ContentMetaInfo.IsPublic,
            CreatedAt = beat.ContentMetaInfo.CreatedAt,
            LastModifiedAt = beat.ContentMetaInfo.LastModifiedAt,
            StoryId = beat.StoryId,
            Description = beat.Description,
            OrderIndex = beat.OrderIndex
        });
    }

    // ========================================================================
    // POST - Create a new story beat
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateStoryBeatAsync(Guid projectId, StoryBeatCreateDto createDto)
    {
        var error = await CheckAccessAsync<CreateResponseDto>(projectId, Permission.CanEdit);
        if (error != null) return error;

        // Create ContentMetaInfo using helper
        var contentMetaInfo = await _core.MetadataService.CreateAsync<ContentMetaInfo>(createDto.CreateData, m =>
        {
            m.ProjectId = projectId;
            m.ContentType = ContentTypeEnum.StoryBeat;
        });

        // Create the StoryBeat entity
        var beat = new StoryBeat
        {
            Id = Guid.NewGuid(),
            MetaInfoId = contentMetaInfo.Id,
            StoryId = createDto.StoryId,
            Description = createDto.Description,
            OrderIndex = createDto.OrderIndex ?? 0,
        };

        _db.StoryBeats.Add(beat);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = beat.Id,
            MetaInfoId = contentMetaInfo.Id,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a story beat
    // ========================================================================

    public async Task<ApiResponseDto<StoryBeatResponseDto>> UpdateStoryBeatAsync(Guid id, StoryBeatUpdateDto updateDto)
    {
        var beat = await _db.StoryBeats
            .Include(sb => sb.ContentMetaInfo)
            .FirstOrDefaultAsync(sb => sb.Id == id);

        if (beat is null)
            return ApiResponseDto<StoryBeatResponseDto>.NotFound($"Story beat with ID {id} not found.");

        var error = await CheckAccessAsync<StoryBeatResponseDto>(beat.ContentMetaInfo.ProjectId, Permission.CanEdit);
        if (error != null) return error;

        // Apply ContentMetaInfo updates via helper
        if (updateDto.ContentMetaInfo != null)
        {
            var success = await SyncIdentityAsync<ContentIdentityStrategy>(beat.MetaInfoId, updateDto.ContentMetaInfo);
            if (!success) return ApiResponseDto<StoryBeatResponseDto>.ServerError("Sync failed.");
        }

        if (updateDto.Description != null)
            beat.Description = updateDto.Description;

        if (updateDto.OrderIndex.HasValue)
            beat.OrderIndex = updateDto.OrderIndex.Value;

        await _db.SaveChangesAsync();

        return ApiResponseDto<StoryBeatResponseDto>.Success(new StoryBeatResponseDto
        {
            Id = beat.Id,
            MetaInfoId = beat.MetaInfoId,
            MetaInfoTitle = beat.ContentMetaInfo.Title,
            Status = beat.ContentMetaInfo.Status,
            IsPublic = beat.ContentMetaInfo.IsPublic,
            CreatedAt = beat.ContentMetaInfo.CreatedAt,
            LastModifiedAt = beat.ContentMetaInfo.LastModifiedAt,
            StoryId = beat.StoryId,
            Description = beat.Description,
            OrderIndex = beat.OrderIndex
        });
    }

    // ========================================================================
    // DELETE - Remove a story beat
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteStoryBeatAsync(Guid id)
    {
        var beat = await _db.StoryBeats
            .Include(sb => sb.ContentMetaInfo)
            .FirstOrDefaultAsync(sb => sb.Id == id);

        if (beat is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Story beat with ID {id} not found.");

        var error = await CheckAccessAsync<DeleteResponseDto>(beat.ContentMetaInfo.ProjectId, Permission.CanDelete);
        if (error != null) return error;

        // Delete ContentMetaInfo first (FK dependency), then StoryBeat
        _db.StoryBeats.Remove(beat);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = beat.ContentMetaInfo.ProjectId.Value
        });
    }
}
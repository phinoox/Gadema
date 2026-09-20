// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Writing.Narrative;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Enums;
using Gadema.Core.Models.Writing.Narrative;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Writing.Narrative;

/// <summary>
/// Service for managing StoryBeats within the narrative domain.
/// Handles CRUD operations including ContentMetaInfo creation and authorization.
/// </summary>
public class StoryBeatService : CoreService
{
    public StoryBeatService(GameDbContext db, ILogger<StoryBeatService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // GET - List all story beats for a project (via ContentMetaInfo.ProjectId)
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<StoryBeatResponseDto>>> GetStoryBeatsAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<StoryBeatResponseDto>>(projectId);
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

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<StoryBeatResponseDto>(beat.ContentMetaInfo.ProjectId);
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
        // Validate project access
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;
        
        // Create ContentMetaInfo using helper
        var ContentMetaInfo = CreateMetaInfo(projectId, ContentTypeEnum.StoryBeat, createDto.CreateData);

        _db.MetaInfos.Add(ContentMetaInfo);
        await _db.SaveChangesAsync();

        // Create the StoryBeat entity
        var beat = new StoryBeat
        {
            Id = Guid.NewGuid(),
            MetaInfoId = ContentMetaInfo.Id,
            StoryId = createDto.StoryId,
            Description = createDto.Description,
            OrderIndex = createDto.OrderIndex ?? 0,
        };

        _db.StoryBeats.Add(beat);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = beat.Id,
            MetaInfoId = ContentMetaInfo.Id,
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

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<StoryBeatResponseDto>(beat.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        // Apply ContentMetaInfo updates via helper
        ApplyMetaInfoUpdates(beat.ContentMetaInfo, updateDto.ContentMetaInfo);

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

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(beat.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        // Delete ContentMetaInfo first (FK dependency), then StoryBeat
        _db.MetaInfos.Remove(beat.ContentMetaInfo);
        _db.StoryBeats.Remove(beat);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = beat.ContentMetaInfo.ProjectId
        });
    }
}
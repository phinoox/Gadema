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
/// Service for managing StoryBeats within the narrative domain.
/// Handles CRUD operations including MetaInfo creation and authorization.
/// </summary>
public class StoryBeatService : CoreService
{
    public StoryBeatService(GameDbContext db, ILogger<StoryBeatService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // GET - List all story beats for a project (via MetaInfo.ProjectId)
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<StoryBeatResponseDto>>> GetStoryBeatsAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<StoryBeatResponseDto>>(projectId);
        if (error != null) return error;

        var beats = await _db.StoryBeats
            .Include(sb => sb.MetaInfo)
            .Where(sb => sb.MetaInfo.ProjectId == projectId)
            .OrderBy(sb => sb.OrderIndex)
            .Select(sb => new StoryBeatResponseDto
            {
                Id = sb.Id,
                MetaInfoId = sb.MetaInfoId,
                MetaInfoTitle = sb.MetaInfo.Title,
                Status = sb.MetaInfo.Status,
                IsPublic = sb.MetaInfo.IsPublic,
                CreatedAt = sb.MetaInfo.CreatedAt,
                LastModifiedAt = sb.MetaInfo.LastModifiedAt,
                StoryOutlineId = sb.StoryOutlineId,
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
            .Include(sb => sb.MetaInfo)
            .FirstOrDefaultAsync(sb => sb.Id == id);

        if (beat is null)
            return ApiResponseDto<StoryBeatResponseDto>.NotFound($"Story beat with ID {id} not found.");

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<StoryBeatResponseDto>(beat.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<StoryBeatResponseDto>.Success(new StoryBeatResponseDto
        {
            Id = beat.Id,
            MetaInfoId = beat.MetaInfoId,
            MetaInfoTitle = beat.MetaInfo.Title,
            Status = beat.MetaInfo.Status,
            IsPublic = beat.MetaInfo.IsPublic,
            CreatedAt = beat.MetaInfo.CreatedAt,
            LastModifiedAt = beat.MetaInfo.LastModifiedAt,
            StoryOutlineId = beat.StoryOutlineId,
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
        
        // Create MetaInfo using helper
        var metaInfo = CreateMetaInfo(projectId, ContentTypeEnum.StoryBeat, createDto.CreateData);

        _db.MetaInfos.Add(metaInfo);
        await _db.SaveChangesAsync();

        // Create the StoryBeat entity
        var beat = new StoryBeat
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfo.Id.Value,
            StoryOutlineId = createDto.StoryOutlineId,
            Description = createDto.Description,
            OrderIndex = createDto.OrderIndex ?? 0,
        };

        _db.StoryBeats.Add(beat);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = beat.Id,
            MetaInfoId = metaInfo.Id.Value,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a story beat
    // ========================================================================

    public async Task<ApiResponseDto<StoryBeatResponseDto>> UpdateStoryBeatAsync(Guid id, StoryBeatUpdateDto updateDto)
    {
        var beat = await _db.StoryBeats
            .Include(sb => sb.MetaInfo)
            .FirstOrDefaultAsync(sb => sb.Id == id);

        if (beat is null)
            return ApiResponseDto<StoryBeatResponseDto>.NotFound($"Story beat with ID {id} not found.");

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<StoryBeatResponseDto>(beat.MetaInfo.ProjectId);
        if (error != null) return error;

        // Apply MetaInfo updates via helper
        ApplyMetaInfoUpdates(beat.MetaInfo, updateDto.MetaInfo);

        if (updateDto.Description != null)
            beat.Description = updateDto.Description;

        if (updateDto.OrderIndex.HasValue)
            beat.OrderIndex = updateDto.OrderIndex.Value;

        await _db.SaveChangesAsync();

        return ApiResponseDto<StoryBeatResponseDto>.Success(new StoryBeatResponseDto
        {
            Id = beat.Id,
            MetaInfoId = beat.MetaInfoId,
            MetaInfoTitle = beat.MetaInfo.Title,
            Status = beat.MetaInfo.Status,
            IsPublic = beat.MetaInfo.IsPublic,
            CreatedAt = beat.MetaInfo.CreatedAt,
            LastModifiedAt = beat.MetaInfo.LastModifiedAt,
            StoryOutlineId = beat.StoryOutlineId,
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
            .Include(sb => sb.MetaInfo)
            .FirstOrDefaultAsync(sb => sb.Id == id);

        if (beat is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Story beat with ID {id} not found.");

        // Authorization: verify user has access to the project
        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(beat.MetaInfo.ProjectId);
        if (error != null) return error;

        // Delete MetaInfo first (FK dependency), then StoryBeat
        _db.MetaInfos.Remove(beat.MetaInfo);
        _db.StoryBeats.Remove(beat);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = beat.MetaInfo.ProjectId
        });
    }
}
// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.DialogueTrees;
using Gadema.Core.Models;
using Gadema.Core.Models.Writing;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.DialogueTrees;

/// <summary>
/// Service for managing SceneSegments within the dialogue trees domain.
/// Handles CRUD operations including authorization.
/// </summary>
public class SceneSegmentService : CoreService
{
    public SceneSegmentService(GameDbContext db, ILogger<SceneSegmentService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // GET - List all scene segments for a project
    // ========================================================================

    public async Task<ApiResponseDto<IEnumerable<SceneSegmentResponseDto>>> GetSceneSegmentsAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<SceneSegmentResponseDto>>(projectId);
        if (error != null) return error;

        var segments = await _db.SceneSegments
            .Include(ss => ss.Scene)
            .ThenInclude(s => s.MetaInfo)
            .Where(ss => ss.Scene.MetaInfo.ProjectId == projectId)
            .OrderBy(ss => ss.SceneId)
            .ThenBy(ss => ss.OrderIndex)
            .Select(ss => new SceneSegmentResponseDto
            {
                Id = ss.Id,
                SceneId = ss.SceneId,
                Type = ss.Type,
                CreatedAt = ss.MetaInfo.CreatedAt,
                LastSyncedAt = null
            })
            .ToListAsync();

        return ApiResponseDto<IEnumerable<SceneSegmentResponseDto>>.Success(segments);
    }

    // ========================================================================
    // GET - Single scene segment by ID
    // ========================================================================

    public async Task<ApiResponseDto<SceneSegmentResponseDto>> GetSceneSegmentAsync(Guid id)
    {
        var segment = await _db.SceneSegments
            .Include(ss => ss.Scene)
            .ThenInclude(s => s.MetaInfo)
            .FirstOrDefaultAsync(ss => ss.Id == id);

        if (segment is null)
            return ApiResponseDto<SceneSegmentResponseDto>.NotFound($"Scene segment with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<SceneSegmentResponseDto>(segment.Scene.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<SceneSegmentResponseDto>.Success(new SceneSegmentResponseDto
        {
            Id = segment.Id,
            SceneId = segment.SceneId,
            Type = segment.Type,
            CreatedAt = segment.MetaInfo.CreatedAt,
            LastSyncedAt = null
        });
    }

    // ========================================================================
    // POST - Create a new scene segment
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateSceneSegmentAsync(Guid projectId, SceneSegmentCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        // Validate that the Scene belongs to the project
        var scene = await _db.Scenes
            .Include(s => s.MetaInfo)
            .FirstOrDefaultAsync(s => s.Id == createDto.SceneId);

        if (scene is null)
            return ApiResponseDto<CreateResponseDto>.NotFound($"Scene with ID {createDto.SceneId} not found.");

        if (scene.MetaInfo.ProjectId != projectId)
            return ApiResponseDto<CreateResponseDto>.BadRequest("Scene does not belong to the specified project.");

        var segment = new SceneSegment
        {
            Id = Guid.NewGuid(),
            MetaInfoId = Guid.NewGuid(), // SceneSegments have their own MetaInfo
            SceneId = createDto.SceneId,
            Type = (SegmentType)createDto.Type,
            OrderIndex = createDto.OrderIndex ?? 0,
        };

        _db.SceneSegments.Add(segment);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = segment.Id,
            MetaInfoId = segment.MetaInfoId,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a scene segment
    // ========================================================================

    public async Task<ApiResponseDto<SceneSegmentResponseDto>> UpdateSceneSegmentAsync(Guid id, SceneSegmentUpdateDto updateDto)
    {
        var segment = await _db.SceneSegments
            .Include(ss => ss.Scene)
            .ThenInclude(s => s.MetaInfo)
            .FirstOrDefaultAsync(ss => ss.Id == id);

        if (segment is null)
            return ApiResponseDto<SceneSegmentResponseDto>.NotFound($"Scene segment with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<SceneSegmentResponseDto>(segment.Scene.MetaInfo.ProjectId);
        if (error != null) return error;

        if (updateDto.SceneId != Guid.Empty)
            segment.SceneId = updateDto.SceneId;

        segment.Type = (SegmentType)updateDto.Type;


        await _db.SaveChangesAsync();

        return ApiResponseDto<SceneSegmentResponseDto>.Success(new SceneSegmentResponseDto
        {
            Id = segment.Id,
            SceneId = segment.SceneId,
            Type = segment.Type,
            CreatedAt = segment.MetaInfo.CreatedAt,
            LastSyncedAt = null
        });
    }

    // ========================================================================
    // DELETE - Remove a scene segment
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteSceneSegmentAsync(Guid id)
    {
        var segment = await _db.SceneSegments
            .Include(ss => ss.Scene)
            .ThenInclude(s => s.MetaInfo)
            .FirstOrDefaultAsync(ss => ss.Id == id);

        if (segment is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Scene segment with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(segment.Scene.MetaInfo.ProjectId);
        if (error != null) return error;

        _db.MetaInfos.Remove(segment.MetaInfo);
        _db.SceneSegments.Remove(segment);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = segment.Scene.MetaInfo.ProjectId
        });
    }
}

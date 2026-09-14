// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.DialogueTrees;
using Gadema.Core.Dtos.Response;

using Gadema.Core.Models;
using Gadema.Core.Models.Writing;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Content;

/// <summary>
/// Service for managing SceneSegments - markers within a scene's RawText that indicate
/// interactive elements like [dialog:nodeId], [action:], etc.
/// </summary>
public class SceneSegmentService : CoreService
{
    public SceneSegmentService(GameDbContext db, ILogger<SceneSegmentService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    private SceneSegmentResponseDto CreateResponseDto(SceneSegment segment)
        => new()
        {
            Id = segment.Id,
            MetaInfoId = segment.MetaInfoId,
            Position = segment.Position,
            Type = segment.Type,
            Value = segment.Value,
            CreatedAt = segment.CreatedAt,
        };

    private ListResponseDto<SceneSegmentResponseDto> CreateListResponseDto(IEnumerable<SceneSegment> segments)
        => new() { Items = segments.Select(CreateResponseDto).ToList(), TotalCount = segments.Count() };

    public async Task<ApiResponseDto<ListResponseDto<SceneSegmentResponseDto>>> GetSegmentsAsync(Guid projectId, Guid? sceneId = null)
    {
        var error = await ValidateProjectAccessAsync<ListResponseDto<SceneSegmentResponseDto>>(projectId);
        if (error != null) return error;
        var query = _db.SceneSegments.Where(s => s.ContentMetaInfo.ProjectId == projectId).OrderByDescending(s => s.Position);

        if (sceneId.HasValue) query = query.Where(s => s.ContentMetaInfo.SceneId == sceneId.Value);

        var segments = await query.ToListAsync();
        return ApiResponseDto<ListResponseDto<SceneSegmentResponseDto>>.Success(CreateListResponseDto(segments));
    }

    public async Task<ApiResponseDto<SceneSegmentResponseDto>> GetSegmentByIdAsync(Guid id)
    {
        var segment = await _db.SceneSegments.Include(s => s.ContentMetaInfo).FirstOrDefaultAsync(s => s.Id == id);
        if (segment is null) return ApiResponseDto<SceneSegmentResponseDto>.NotFound($"Scene segment with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<SceneSegmentResponseDto>(segment.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<SceneSegmentResponseDto>.Success(CreateResponseDto(segment));
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateSegmentAsync(Guid projectId, SceneSegmentCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        // Find the scene by ContentMetaInfo.SceneId
        var ContentMetaInfo = _db.MetaInfos.FirstOrDefault(m => m.Id == createDto.SceneId && m.ProjectId == projectId);
        if (ContentMetaInfo is null) return ApiResponseDto<CreateResponseDto>.BadRequest("Scene not found.");

        var segment = new SceneSegment
        {
            Id = Guid.NewGuid(),
            MetaInfoId = ContentMetaInfo.Id,
            Position = createDto.Position,
            Type = createDto.Type,
            Value = createDto.Value,
        };

        _db.SceneSegments.Add(segment);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto { EntityId = segment.Id, MetaInfoId = ContentMetaInfo.Id, ProjectId = projectId });
    }

    public async Task<ApiResponseDto<SceneSegmentResponseDto>> UpdateSegmentAsync(Guid id, SceneSegmentUpdateDto updateDto)
    {
        var segment = await _db.SceneSegments.Include(s => s.ContentMetaInfo).FirstOrDefaultAsync(s => s.Id == id);
        if (segment is null) return ApiResponseDto<SceneSegmentResponseDto>.NotFound($"Scene segment with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<SceneSegmentUpdateDto>(segment.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        ApplyMetaInfoUpdates(segment.ContentMetaInfo, updateDto.ContentMetaInfo);

        if (updateDto.Position.HasValue) segment.Position = updateDto.Position.Value;
        if (!string.IsNullOrEmpty(updateDto.Type)) segment.Type = updateDto.Type;
        if (updateDto.Value != null) segment.Value = updateDto.Value;

        await _db.SaveChangesAsync();
        return ApiResponseDto<SceneSegmentResponseDto>.Success(CreateResponseDto(segment));
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteSegmentAsync(Guid id)
    {
        var segment = await _db.SceneSegments.Include(s => s.ContentMetaInfo).FirstOrDefaultAsync(s => s.Id == id);
        if (segment is null) return ApiResponseDto<DeleteResponseDto>.NotFound($"Scene segment with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(segment.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        _db.SceneSegments.Remove(segment);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto { EntityId = id, ProjectId = segment.ContentMetaInfo.ProjectId });
    }
}
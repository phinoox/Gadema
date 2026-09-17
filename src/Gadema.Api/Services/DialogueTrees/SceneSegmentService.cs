using Gadema.Api.Services.Search;
using Gadema.Api.Services.Tags.Strategies;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.DialogueTrees;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Dtos.Search;
using Gadema.Core.Models;
using Gadema.Core.Models.Writing;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Writing.Narrative;

/// <summary>
/// Service for managing SceneSegments - markers within a scene's RawText that indicate
/// interactive elements like [dialog:nodeId], [action:], etc.
/// </summary>
public class SceneSegmentService : CoreService, ISearchableProvider
{
    public SceneSegmentService(GameDbContext db, ILogger<SceneSegmentService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

   
    private ListResponseDto<SceneSegmentResponseDto> CreateListResponseDto(IEnumerable<SceneSegment> segments)
        => new() { Items = segments.Select(CreateResponseDto).ToList(), TotalCount = segments.Count() };

    public async Task<ApiResponseDto<ListResponseDto<SceneSegmentResponseDto>>> GetSegmentsAsync(Guid projectId, Guid? sceneId = null)
    {
        var error = await ValidateProjectAccessAsync<ListResponseDto<SceneSegmentResponseDto>>(projectId);
        if (error != null) return error;

        var query = _db.SceneSegments
            .Include(s => s.ContentMetaInfo)
            .Where(s => s.ContentMetaInfo.ProjectId == projectId)
            .OrderByDescending(s => s.Position)
            .AsQueryable();

        if (sceneId.HasValue) 
            query = query.Where(s => s.SceneId == sceneId.Value);

        var segments = await query.ToListAsync();
        return ApiResponseDto<ListResponseDto<SceneSegmentResponseDto>>.Success(CreateListResponseDto(segments));
    }

    public async Task<ApiResponseDto<SceneSegmentResponseDto>> GetSegmentByIdAsync(Guid id)
    {
        var segment = await _db.SceneSegments
            .Include(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (segment is null) 
            return ApiResponseDto<SceneSegmentResponseDto>.NotFound($"Scene segment with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<SceneSegmentResponseDto>(segment.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<SceneSegmentResponseDto>.Success(CreateResponseDto(segment));
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateSegmentAsync(Guid projectId, SceneSegmentCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        // 1. Find the parent scene to ensure it belongs to this project
        var scene = await _db.Scenes
            .Include(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(s => s.Id == createDto.SceneId && s.ContentMetaInfo.ProjectId == projectId);

        if (scene == null) 
            return ApiResponseDto<CreateResponseDto>.BadRequest("Target scene not found or access denied.");

        // 2. Create the MetaInfo anchor using the generic helper
        var meta = CreateMetaInfo<ContentMetaInfo>(createDto.CreateData, m => {
            m.ProjectId = projectId;
            m.ContentType = ContentTypeEnum.SceneSegment; // Assuming this exists in your enum
        });

        // 3. Create the SceneSegment component
        var segment = new SceneSegment
        {
            Id = Guid.NewGuid(),
            MetaInfoId = meta.Id,
            SceneId = scene.Id,
            Type = createDto.Type,
            Position = createDto.Position,
            Value = createDto.Value,
            OrderIndex = createDto.OrderIndex
        };

        _db.SceneSegments.Add(segment);
        _db.Set<ContentMetaInfo>().Add(meta);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto 
        { 
            EntityId = segment.Id, 
            MetaInfoId = meta.Id, 
            ProjectId = projectId 
        });
    }

    public async Task<ApiResponseDto<SceneSegmentResponseDto>> UpdateSegmentAsync(Guid id, SceneSegmentUpdateDto updateDto)
    {
        var segment = await _db.SceneSegments
            .Include(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (segment is null)
            return ApiResponseDto<SceneSegmentResponseDto>.NotFound($"Scene segment with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<SceneSegmentUpdateDto>(segment.ContentMetaInfo.ProjectId);
        if (error != null) return ApiResponseDto<SceneSegmentResponseDto>.Unauthorized("not authorized");

        // 1. Delegate Identity Updates to the Strategy
        if (updateDto.ContentMetaInfo != null)
        {
            await ApplyIdentitySyncAsync(segment.MetaInfoId, updateDto.ContentMetaInfo, new ContentIdentityStrategy(_db));
        }

        // 2. Update Domain Properties
        if (updateDto.Position.HasValue) segment.Position = updateDto.Position.Value;
        if (updateDto.Type.HasValue) segment.Type = updateDto.Type.Value;
        if (updateDto.Value != null) segment.Value = updateDto.Value;
        if (updateDto.OrderIndex.HasValue) segment.OrderIndex = updateDto.OrderIndex.Value;

        await _db.SaveChangesAsync();
        return ApiResponseDto<SceneSegmentResponseDto>.Success(CreateResponseDto(segment));
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteSegmentAsync(Guid id)
    {
        var segment = await _db.SceneSegments
            .Include(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (segment is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Scene segment with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(segment.ContentMetaInfo.ProjectId);
        if (error != null) return error;

        // Removing the MetaInfo anchor will cascade to the Segment via the relationship
        _db.Set<ContentMetaInfo>().Remove(segment.ContentMetaInfo);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto 
        { 
            EntityId = id, 
            ProjectId = segment.ContentMetaInfo.ProjectId 
        });
    }

    public async Task<IEnumerable<SearchHitDto>> GetMatchesAsync(string query, Guid? projectId)
    {
        var queryable = _db.SceneSegments
            .Include(s => s.ContentMetaInfo)
            .AsQueryable();

        if (projectId.HasValue)
            queryable = queryable.Where(s => s.ContentMetaInfo.ProjectId == projectId.Value);

        return await queryable
            .Where(s => s.ContentMetaInfo.Title.Contains(query) || 
                        s.Type.ToString().Contains(query))
            .Select(s => new SearchHitDto
            {
                ResourceId = s.Id,
                DisplayName = s.ContentMetaInfo.Title,
                Slug = s.ContentMetaInfo.Slug,
                ResourceType = "SceneSegment",
                ScopeId = s.ContentMetaInfo.ProjectId,
                ResourceLink = $"/api/v1/projects/{s.ContentMetaInfo.ProjectId}/scenes/{s.SceneId}/segments/{s.Id}"
            })
            .ToListAsync();
    }

    private SceneSegmentResponseDto CreateResponseDto(SceneSegment segment)
    {
        return new SceneSegmentResponseDto
        {
            Id = segment.Id,
            Position = segment.Position,
            Type = segment.Type,
            Value = segment.Value,
            OrderIndex = segment.OrderIndex,
            CreatedAt = segment.ContentMetaInfo.CreatedAt
        };
    }
}
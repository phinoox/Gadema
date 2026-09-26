using Gadema.Api.CoreServices;
using Gadema.Api.CoreServices.Strategies;
using Gadema.Api.Services.Search;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Dtos.Search;
using Gadema.Core.Dtos.Writing.DialogueTrees;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Enums;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Core.Models.Writing.Narrative;
using Gadema.Data.Database.Core;
using Gadema.Data.Database.Writing;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Writing.Narrative;

[ServiceLifetime(ServiceLifetime.Scoped)] public class SceneSegmentService : DomainService, ISearchableProvider
{
    private WritingDbContext _db;

    public SceneSegmentService( WritingDbContext db,
        ILogger<SceneSegmentService> logger,  
        CoreServicesProvider coreServices) // Injected via CoreService constructor
        : base(coreServices,logger) { _db = db; }

   
    private ListResponseDto<SceneSegmentResponseDto> CreateListResponseDto(IEnumerable<SceneSegment> segments)
        => new() { Items = segments.Select(CreateResponseDto).ToList(), TotalCount = segments.Count() };

    public async Task<ApiResponseDto<ListResponseDto<SceneSegmentResponseDto>>> GetSegmentsAsync(Guid projectId, Guid? sceneId = null)
    {
        var error = await CheckAccessAsync<ListResponseDto<SceneSegmentResponseDto>>(projectId, Permission.CanView);
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

        var error = await CheckAccessAsync<SceneSegmentResponseDto>(segment.ContentMetaInfo.ProjectId, Permission.CanView);
        if (error != null) return error;

        return ApiResponseDto<SceneSegmentResponseDto>.Success(CreateResponseDto(segment));
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateSegmentAsync(Guid projectId, SceneSegmentCreateDto createDto)
    {
        var error = await CheckAccessAsync<CreateResponseDto>(projectId, Permission.CanEdit);
        if (error != null) return error;

        var scene = await _db.Scenes
            .Include(s => s.ContentMetaInfo)
            .FirstOrDefaultAsync(s => s.Id == createDto.SceneId && s.ContentMetaInfo.ProjectId == projectId);

        if (scene == null) 
            return ApiResponseDto<CreateResponseDto>.BadRequest("Target scene not found or access denied.");

        var contentMetaInfo = await _core.MetadataService.CreateAsync<ContentMetaInfo>(createDto.CreateData, m =>
        {
            m.ProjectId = projectId;
            m.ContentType = ContentTypeEnum.SceneSegment;
        });

        var segment = new SceneSegment
        {
            Id = Guid.NewGuid(),
            MetaInfoId = contentMetaInfo.Id,
            SceneId = scene.Id,
            Type = createDto.Type,
            Position = createDto.Position,
            Value = createDto.Value,
            OrderIndex = createDto.OrderIndex
        };

        _db.SceneSegments.Add(segment);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto 
        { 
            EntityId = segment.Id, 
            MetaInfoId = contentMetaInfo.Id, // Error here: meta should be contentMetaInfo
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

        var error = await CheckAccessAsync<SceneSegmentResponseDto>(segment.ContentMetaInfo.ProjectId, Permission.CanEdit);
        if (error != null) return error;

        if (updateDto.ContentMetaInfo != null)
        {
            var success = await SyncIdentityAsync<ContentIdentityStrategy>(segment.MetaInfoId, updateDto.ContentMetaInfo);
            if (!success) return ApiResponseDto<SceneSegmentResponseDto>.ServerError("Sync failed.");
        }

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

        var error = await CheckAccessAsync<DeleteResponseDto>(segment.ContentMetaInfo.ProjectId, Permission.CanDelete);
        if (error != null) return error;

        _db.Set<ContentMetaInfo>().Remove(segment.ContentMetaInfo);
        _db.SceneSegments.Remove(segment);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto 
        { 
            EntityId = id, 
            ProjectId = segment.ContentMetaInfo.ProjectId.Value 
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
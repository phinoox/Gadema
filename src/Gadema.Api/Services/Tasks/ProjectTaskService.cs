using Gadema.Api.Services.Search;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Tasks;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Models.Base.MetaInfo;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;
using Gadema.Api.Services.Tags.Strategies;
using Gadema.Core.Dtos.Search;

namespace Gadema.Api.Services.Tasks;

/// <summary>
/// Service for managing ProjectTasks - actionable items within a project workflow.
/// </summary>
public class ProjectTaskService : CoreService, ISearchableProvider
{
    public ProjectTaskService(GameDbContext db, ILogger<ProjectTaskService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // GET - List all tasks for a project
    // ========================================================================

    public async Task<ApiResponseDto<ListResponseDto<ProjectTaskResponseDto>>> GetTasksAsync(Guid projectId)
    {
        var error = await ValidateProjectAccessAsync<ListResponseDto<ProjectTaskResponseDto>>(projectId);
        if (error != null) return error;

        var tasks = await _db.ProjectTasks
            .Include(t => t.MetaInfo)
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.DueDate ?? DateTime.MaxValue)
            .ToListAsync();

        return ApiResponseDto<ListResponseDto<ProjectTaskResponseDto>>.Success(new ListResponseDto<ProjectTaskResponseDto>
        {
            Items = tasks.Select(CreateResponseDto),
            TotalCount = tasks.Count
        });
    }

    // ========================================================================
    // GET - Single task by ID
    // ========================================================================

    public async Task<ApiResponseDto<ProjectTaskResponseDto>> GetTaskAsync(Guid id)
    {
        var task = await _db.ProjectTasks
            .Include(t => t.MetaInfo)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task is null) 
            return ApiResponseDto<ProjectTaskResponseDto>.NotFound($"Task with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<ProjectTaskResponseDto>(task.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<ProjectTaskResponseDto>.Success(CreateResponseDto(task));
    }

    // ========================================================================
    // POST - Create a new task
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateTaskAsync(Guid projectId, ProjectTaskCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        // 1. Create the MetaInfo anchor using the generic helper
        var meta = CreateMetaInfo<ProjectTaskMetaInfo>(createDto.MetaInfo, m => {
            m.ProjectId = projectId;
        });

        // 2. Create the ProjectTask component
        var task = new ProjectTask
        {
            Id = meta.Id,
            MetaInfoId = meta.Id,
            ProjectId = projectId,
            AssignedToUserId = createDto.AssignedToUserId,
            DueDate = createDto.DueDate,
            CreatedByUserId = createDto.CreatedByUserId
        };

        _db.ProjectTasks.Add(task);
        _db.Set<ProjectTaskMetaInfo>().Add(meta);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto 
        { 
            EntityId = task.Id, 
            MetaInfoId = task.MetaInfoId, 
            ProjectId = projectId 
        });
    }

    // ========================================================================
    // PUT - Partial update of identity and domain data
    // ========================================================================

    public async Task<ApiResponseDto<ProjectTaskResponseDto>> UpdateTaskAsync(Guid id, ProjectTaskUpdateDto updateDto)
    {
        var task = await _db.ProjectTasks
            .Include(t => t.MetaInfo)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task is null)
            return ApiResponseDto<ProjectTaskResponseDto>.NotFound($"Task with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<ProjectTaskUpdateDto>(task.ProjectId);
        if (error != null) return ApiResponseDto<ProjectTaskResponseDto>.Unauthorized("not authorized");

        // 1. Delegate Identity Updates to the Strategy
        if (updateDto.MetaInfo != null)
        {
            await ApplyIdentitySyncAsync(task.MetaInfoId, updateDto.MetaInfo, new ContentIdentityStrategy(_db));
        }

        // 2. Update Domain Properties
        if (updateDto.AssignedToUserId.HasValue) task.AssignedToUserId = updateDto.AssignedToUserId;
        if (updateDto.DueDate.HasValue) task.DueDate = updateDto.DueDate;

        await _db.SaveChangesAsync();
        return ApiResponseDto<ProjectTaskResponseDto>.Success(CreateResponseDto(task));
    }

    // ========================================================================
    // DELETE - Remove a task
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteTaskAsync(Guid id)
    {
        var task = await _db.ProjectTasks
            .Include(t => t.MetaInfo)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Task with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(task.ProjectId);
        if (error != null) return error;

        // Deleting the MetaInfo anchor will cascade to the ProjectTask component
        _db.Set<ProjectTaskMetaInfo>().Remove(task.MetaInfo);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto 
        { 
            EntityId = id, 
            ProjectId = task.ProjectId 
        });
    }

    // ========================================================================
    // ISearchableProvider Implementation
    // ========================================================================

    public async Task<IEnumerable<SearchHitDto>> GetMatchesAsync(string query, Guid? projectId)
    {
        var queryable = _db.ProjectTasks
            .Include(t => t.MetaInfo)
            .AsQueryable();

        if (projectId.HasValue)
            queryable = queryable.Where(t => t.ProjectId == projectId.Value);

        return await queryable
            .Where(t => t.MetaInfo.Title.Contains(query) || 
                        t.MetaInfo.ShortDesc.Contains(query))
            .Select(t => new SearchHitDto
            {
                ResourceId = t.Id,
                DisplayName = t.MetaInfo.Title,
                Slug = t.MetaInfo.Slug,
                ResourceType = "ProjectTask",
                ScopeId = t.ProjectId,
                ResourceLink = $"/api/v1/projects/{t.ProjectId}/tasks/{t.Id}"
            })
            .ToListAsync();
    }

    private ProjectTaskResponseDto CreateResponseDto(ProjectTask task)
    {
        return new ProjectTaskResponseDto
        {
            Id = task.Id,
            MetaInfoId = task.MetaInfoId,
            // Denormalized identity properties from the anchor
            Title = task.MetaInfo.Title,
            Slug = task.MetaInfo.Slug,
            IsPublic = task.MetaInfo.IsPublic,
            CreatedAt = task.MetaInfo.CreatedAt,
            // Domain properties
            ProjectId = task.ProjectId,
            AssignedToUserId = task.AssignedToUserId,
            DueDate = task.DueDate,
            CreatedByUserId = task.CreatedByUserId
        };
    }
}
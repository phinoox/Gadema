// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Dtos.Tags;
using Gadema.Core.Dtos.Tasks;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Tasks;

/// <summary>
/// Service for managing ProjectTasks within the tasks domain.
/// Flat task structure with ADHD-friendly features (quick win flag, difficulty).
/// </summary>
public class ProjectTaskService : CoreService
{
    public ProjectTaskService(GameDbContext db, ILogger<ProjectTaskService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    /// <summary>Creates a response DTO from a ProjectTask entity.</summary>
    private TagResponseDto CreateResponseDto(ProjectTask task)
        => new()
        {
            Id = task.Id,
            MetaInfoId = task.MetaInfoId,
            Status = (int)task.Status,
            Priority = (int)task.Priority,
            Difficulty = (int)task.Difficulty,
            TaskTitle = task.TaskTitle,
            Description = task.Description,
            EstimatedMinutes = task.EstimatedMinutes,
            AssignedToUserId = task.AssignedToUserId,
            DueDate = task.DueDate,
            IsQuickWin = task.IsQuickWin,
            CreatedAt = task.CreatedAt,
            LastModifiedAt = task.LastModifiedAt,
        };

    /// <summary>Creates a list response DTO from collection.</summary>
    private ListResponseDto<TaskResponseDto> CreateListResponseDto(IEnumerable<ProjectTask> tasks)
        => new() { Items = tasks.Select(CreateResponseDto).ToList(), TotalCount = tasks.Count() };

    // ========================================================================
    // GET - List all tasks for a project (with optional filters)
    // ========================================================================

    public async Task<ApiResponseDto<ListResponseDto<TaskResponseDto>>> GetTasksAsync(Guid projectId, int? status = null, Guid? assignedToUserId = null, bool? isQuickWin = null, string? searchQuery = null)
    {
        var error = await ValidateProjectAccessAsync<ListResponseDto<TaskResponseDto>>(projectId);
        if (error != null) return error;

        IQueryable<ProjectTask> query = _db.ProjectTasks
            .Include(pt => pt.ContentMetaInfo)
            .Where(pt => pt.ProjectId == projectId);

        if (status.HasValue)
            query = query.Where(pt => pt.Status == status.Value);

        if (assignedToUserId.HasValue)
            query = query.Where(pt => pt.AssignedToUserId == assignedToUserId.Value || pt.AssignedToUserId == null);

        if (isQuickWin.HasValue)
            query = query.Where(pt => pt.IsQuickWin == isQuickWin.Value);

        if (!string.IsNullOrWhiteSpace(searchQuery))
            query = query.Where(pt => pt.TaskTitle.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));

        var tasks = await query.OrderBy(pt => pt.Status).ThenByDescending(pt => pt.Priority).ThenBy(pt => pt.DueDate ?? DateTime.MaxValue).ToListAsync();
        return ApiResponseDto<ListResponseDto<TaskResponseDto>>.Success(CreateListResponseDto(tasks));
    }

    // ========================================================================
    // GET - Single task by ID
    // ========================================================================

    public async Task<ApiResponseDto<TaskResponseDto>> GetTaskAsync(Guid id)
    {
        var task = await _db.ProjectTasks
            .Include(pt => pt.ContentMetaInfo)
            .FirstOrDefaultAsync(pt => pt.Id == id);

        if (task is null)
            return ApiResponseDto<TaskResponseDto>.NotFound($"Project task with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<TaskResponseDto>(task.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<TaskResponseDto>.Success(CreateResponseDto(task));
    }

    // ========================================================================
    // POST - Create a new project task
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> CreateTaskAsync(Guid projectId, ProjectTaskCreateDto createDto)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        // Determine MetaInfoId from task title or description keywords
        Guid? metaInfoId = null;
        if (!string.IsNullOrWhiteSpace(createDto.TaskTitle))
            metaInfoId = GetOrCreateMetaInfoForTask(projectId, createDto.TaskTitle);

        var task = new ProjectTask
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            MetaInfoId = metaInfoId,
            TaskTitle = createDto.TaskTitle,
            Description = createDto.Description,
            Status = (int)createDto.Status ?? 0,
            Priority = (int)createDto.Priority ?? 1,
            Difficulty = (int)createDto.Difficulty ?? 1,
            EstimatedMinutes = createDto.EstimatedMinutes,
            AssignedToUserId = createDto.AssignedToUserId,
            DueDate = createDto.DueDate,
            IsQuickWin = createDto.IsQuickWin ?? false,
            CreatedByUserId = _userContext.CurrentUser!.Id,
        };

        _db.ProjectTasks.Add(task);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto
        {
            EntityId = task.Id,
            MetaInfoId = metaInfoId ?? Guid.Empty,
            ProjectId = projectId
        });
    }

    // ========================================================================
    // PUT - Partial update of a project task
    // ========================================================================

    public async Task<ApiResponseDto<TaskResponseDto>> UpdateTaskAsync(Guid id, ProjectTaskUpdateDto updateDto)
    {
        var task = await _db.ProjectTasks
            .Include(pt => pt.ContentMetaInfo)
            .FirstOrDefaultAsync(pt => pt.Id == id);

        if (task is null)
            return ApiResponseDto<TaskResponseDto>.NotFound($"Project task with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<ProjectTaskUpdateDto>(task.ProjectId);
        if (error != null) return error;

        if (!string.IsNullOrWhiteSpace(updateDto.TaskTitle))
            task.TaskTitle = updateDto.TaskTitle;

        if (updateDto.Description != null)
            task.Description = updateDto.Description;

        if (updateDto.Status.HasValue)
            task.Status = updateDto.Status.Value;

        if (updateDto.Priority.HasValue)
            task.Priority = updateDto.Priority.Value;

        if (updateDto.Difficulty.HasValue)
            task.Difficulty = updateDto.Difficulty.Value;

        if (updateDto.EstimatedMinutes != null)
            task.EstimatedMinutes = updateDto.EstimatedMinutes.Value;

        if (updateDto.AssignedToUserId.HasValue)
            task.AssignedToUserId = updateDto.AssignedToUserId.Value;

        if (updateDto.DueDate.HasValue)
            task.DueDate = updateDto.DueDate.Value;

        if (updateDto.IsQuickWin.HasValue)
            task.IsQuickWin = updateDto.IsQuickWin.Value;

        await _db.SaveChangesAsync();

        return ApiResponseDto<TaskResponseDto>.Success(CreateResponseDto(task));
    }

    // ========================================================================
    // DELETE - Remove a project task
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteTaskAsync(Guid id)
    {
        var task = await _db.ProjectTasks
            .Include(pt => pt.ContentMetaInfo)
            .FirstOrDefaultAsync(pt => pt.Id == id);

        if (task is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Project task with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(task.ProjectId);
        if (error != null) return error;

        // Soft delete: set title to empty and mark as done, don't cascade delete ContentMetaInfo
        task.TaskTitle = "[DELETED]";
        task.Status = (int)TaskStatusEnum.Done;
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto
        {
            EntityId = id,
            ProjectId = task.ProjectId
        });
    }

    // ========================================================================
    // HELPER: Get or create ContentMetaInfo from task title keywords
    // ========================================================================

    private Guid? GetOrCreateMetaInfoForTask(Guid projectId, string taskTitle)
    {
        var keywords = new[] { "character", "faction", "location", "scene", "chapter", "plot" };
        foreach (var keyword in keywords)
        {
            if (taskTitle.ToLowerInvariant().Contains(keyword))
                return null; // Task title suggests it's about an entity, let user create ContentMetaInfo separately
        }

        var ContentMetaInfo = _db.MetaInfos.FirstOrDefault(m => m.ProjectId == projectId && m.Title.ToLowerInvariant() == taskTitle.ToLowerInvariant());
        return ContentMetaInfo?.Id;
    }
}
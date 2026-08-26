// =============================================================================
using GameDev.Core.Dtos;
using Microsoft.EntityFrameworkCore;
// GameDev.Api - ASP.NET Core Web API Services
// =============================================================================


using System;
using System.Linq;
using System.Threading.Tasks;
using GameDev.Core.Dtos;
using GameDev.Data;
using Microsoft.Extensions.Logging;
using GameDev.Core.Models;
using GameDev.Core.Dtos.Tasks;
using GameDev.Core.Services;

namespace GameDev.Api.Services;

/// <summary>
/// Implementation of task service (renamed to ProjectTaskService).
/// </summary>
public class ProjectTaskService : IGademaService,  IProjectTaskService
{
    private readonly GameDbContext _context;
    private readonly ILogger<ProjectTaskService> _logger;

    public ServiceTypeEnum ServiceType => ServiceTypeEnum.ProjectTaskService;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public ProjectTaskService(GameDbContext context, ILogger<ProjectTaskService> logger)  // ✅ Fixed: correct class name
    {
        _context = context;
        _logger = logger;
        if(_context == null)
        {
            throw new Exception("Error: DB context was null");
        }
    }

    /// <summary>
    /// List all tasks (paginated).
    /// </summary>
    public async Task<ApiResponseDto<PaginationResponse<ProjectTaskResponseDto>>> GetTasksAsync(Guid? projectId, int? status, int? difficulty, bool isQuickWin)  // ✅ Fixed: added async
    {
        var query = _context.ProjectTasks.AsQueryable();  // ✅ Fixed: use ProjectTasks instead of Tasks
        
        if (projectId.HasValue)
        {
            query = query.Where(t => t.ProjectId == projectId.Value);
        }
        
        if (status.HasValue)
        {
            query = query.Where(t => t.Status == status.Value);
        }
        
        if (difficulty.HasValue)
        {
            query = query.Where(t => t.Difficulty == difficulty.Value);
        }
        
        if (isQuickWin)
        {
            query = query.Where(t => t.IsQuickWin);
        }
        
        query = query.OrderByDescending(t => t.CreatedAt);
        
        var tasks = await query.Skip(0).Take(20).Select(t => new ProjectTaskResponseDto
        {
            Id = t.Id,
            ProjectId = t.ProjectId,
            ContentItemId = t.ContentItemId,
            TaskTitle = t.TaskTitle,
            Description = t.Description,
            Status = t.Status,
            Priority = t.Priority,
            Difficulty = t.Difficulty,
            EstimatedMinutes = t.EstimatedMinutes,
            AssignedToUserId = t.AssignedToUserId,
            DueDate = t.DueDate,
            IsQuickWin = t.IsQuickWin,
            CreatedAt = t.CreatedAt,
            TaskDescription = t.Description  // ✅ Fixed: include TaskDescription property
        }).ToListAsync();

        return ApiResponseDto<PaginationResponse<ProjectTaskResponseDto>>.Success(new PaginationResponse<ProjectTaskResponseDto>());
    }

    /// <summary>
    /// Create new task.
    /// </summary>
    public async Task<ApiResponseDto<ProjectTaskResponseDto>> CreateTaskAsync(ProjectTaskCreateDto createDto)  // ✅ Fixed: return ProjectTaskResponseDto instead of generic TaskResponseDto
    {
        var now = DateTime.UtcNow;
        
        var task = new ProjectTask
        {
            Id = Guid.NewGuid(),
            ProjectId = createDto.ProjectId,
            ContentItemId = null!,
            TaskTitle = createDto.TaskTitle,
            Description = createDto.Description,
            Status = createDto.Status ?? 0,
            Priority = createDto.Priority ?? 1,
            Difficulty = createDto.Difficulty ?? 0,
            EstimatedMinutes = createDto.EstimatedMinutes,
            AssignedToUserId = null!,
            DueDate = null!,
            IsQuickWin = createDto.IsQuickWin ?? false,
            CreatedAt = now,
            CreatedByUserId = UserHelper.GetUserId()
        };

        _context.ProjectTasks.Add(task);  // ✅ Fixed: use ProjectTasks instead of Tasks
        await _context.SaveChangesAsync();

        return ApiResponseDto<ProjectTaskResponseDto>.Success(new ProjectTaskResponseDto
        {
            Id = task.Id,
            ProjectId = task.ProjectId,
            ContentItemId = task.ContentItemId,
            TaskTitle = task.TaskTitle,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            Difficulty = task.Difficulty,
            EstimatedMinutes = task.EstimatedMinutes,
            AssignedToUserId = task.AssignedToUserId,
            DueDate = task.DueDate,
            IsQuickWin = task.IsQuickWin,
            CreatedAt = task.CreatedAt
        });  // ✅ Fixed: populate actual response data instead of empty constructor
    }

    /// <summary>
    /// Update task.
    /// </summary>
    public async Task<ApiResponseDto<ProjectTaskResponseDto>> UpdateTaskAsync(Guid id, ProjectTaskUpdateDto updateDto)  // ✅ Fixed: use ProjectTaskUpdateDto and return ProjectTaskResponseDto
    {
        var task = await _context.ProjectTasks.FindAsync(id);  // ✅ Fixed: use ProjectTasks

        if (task == null)
        {
            return ApiResponseDto<ProjectTaskResponseDto>.NotFound($"Task with ID {id} not found");
        }

        if (updateDto.Status.HasValue)
        {
            task.Status = updateDto.Status.Value;
        }

        if (updateDto.Difficulty.HasValue)
        {
            task.Difficulty = updateDto.Difficulty.Value;
        }

        if (updateDto.IsQuickWin.HasValue)
        {
            task.IsQuickWin = updateDto.IsQuickWin.Value;
        }

        task.LastModifiedAt = DateTime.UtcNow;
        _context.Entry(task).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
        await _context.SaveChangesAsync();

        return ApiResponseDto<ProjectTaskResponseDto>.Success(new ProjectTaskResponseDto
        {
            Id = task.Id,
            ProjectId = task.ProjectId,
            ContentItemId = task.ContentItemId,
            TaskTitle = task.TaskTitle,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            Difficulty = task.Difficulty,
            EstimatedMinutes = task.EstimatedMinutes,
            AssignedToUserId = task.AssignedToUserId,
            DueDate = task.DueDate,
            IsQuickWin = task.IsQuickWin,
            CreatedAt = task.CreatedAt
        });  // ✅ Fixed: populate actual response data instead of empty constructor
    }

    /// <summary>
    /// Delete task.
    /// </summary>
    public async Task<ApiResponseDto<ProjectTaskResponseDto>> DeleteTaskAsync(Guid id)  // ✅ Fixed: return ProjectTaskResponseDto
    {
        var task = await _context.ProjectTasks.FindAsync(id);  // ✅ Fixed: use ProjectTasks

        if (task == null)
        {
            return ApiResponseDto<ProjectTaskResponseDto>.NotFound($"Task with ID {id} not found");
        }

        _context.ProjectTasks.Remove(task);  // ✅ Fixed: use ProjectTasks
        await _context.SaveChangesAsync();

        return ApiResponseDto<ProjectTaskResponseDto>.Success(new ProjectTaskResponseDto
        {
            Id = task.Id,
            TaskTitle = task.TaskTitle,
            CreatedAt = task.CreatedAt
        });  // ✅ Fixed: populate actual response data instead of empty constructor
    }
}
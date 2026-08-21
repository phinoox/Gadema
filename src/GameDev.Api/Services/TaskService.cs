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

namespace GameDev.Api.Services;

/// <summary>
/// Implementation of task service.
/// </summary>
public class TaskService : ITaskService
{
    private readonly GameDbContext _context;
    private readonly ILogger<TaskService> _logger;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public TaskService(GameDbContext context, ILogger<TaskService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// List all tasks (paginated).
    /// </summary>
    public async Task<ApiResponseDto<PaginationResponse<TaskResponseDto>>> GetTasksAsync(Guid? projectId, int? status, int? difficulty, bool isQuickWin)
    {
        var query = _context.Tasks.AsQueryable();
        
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
        
        var tasks = await query.Skip(0).Take(20).Select(t => new TaskResponseDto
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
            CreatedAt = t.CreatedAt
        }).ToListAsync();

        return ApiResponseDto<PaginationResponse<TaskResponseDto>>.Success(new PaginationResponse<TaskResponseDto>());
    }

    /// <summary>
    /// Create new task.
    /// </summary>
    public async Task<ApiResponseDto<TaskResponseDto>> CreateTaskAsync(CreateTaskDto createDto)
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
            IsQuickWin = createDto.IsQuickWin,
            CreatedAt = now,
            CreatedByUserId = UserHelper.GetUserId()
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return ApiResponseDto<TaskResponseDto>.Success(new TaskResponseDto());
    }

    /// <summary>
    /// Update task.
    /// </summary>
    public async Task<ApiResponseDto<TaskResponseDto>> UpdateTaskAsync(Guid id, UpdateTaskDto updateDto)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
        {
            return ApiResponseDto<TaskResponseDto>.NotFound($"Task with ID {id} not found");
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

        return ApiResponseDto<TaskResponseDto>.Success(new TaskResponseDto());
    }

    /// <summary>
    /// Delete task.
    /// </summary>
    public async Task<ApiResponseDto<TaskResponseDto>> DeleteTaskAsync(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
        {
            return ApiResponseDto<TaskResponseDto>.NotFound($"Task with ID {id} not found");
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return ApiResponseDto<TaskResponseDto>.Success(new TaskResponseDto());
    }

    Task<ApiResponseDto<TaskResponseDto>> ITaskService.DeleteTaskAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
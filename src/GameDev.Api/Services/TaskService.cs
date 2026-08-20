// =============================================================================
// GameDev.Api - ASP.NET Core Web API Services
// =============================================================================

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
    public async Task<ApiResponseDto<PaginationResponse<TaskResponse>>> GetTasksAsync(Guid? projectId, int? status, int? difficulty, bool isQuickWin)
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
        
        query = query.OrderBy(t => t.CreatedAt, Microsoft.EntityFrameworkCore.Sorting.Order.Descending);
        
        var tasks = await query.Skip(0).Take(20).Select(t => new TaskResponse
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

        return ApiResponseDto.Success<PaginationResponse<TaskResponse>>(new PaginationResponse<TaskResponse>());
    }

    /// <summary>
    /// Create new task.
    /// </summary>
    public async Task<ApiResponseDto<TaskResponse>> CreateTaskAsync(CreateTaskDto createDto)
    {
        var now = DateTime.UtcNow;
        
        var task = new Task
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

        return ApiResponseDto.Success<TaskResponse>(new TaskResponse());
    }

    /// <summary>
    /// Update task.
    /// </summary>
    public async Task<ApiResponseDto<TaskResponse>> UpdateTaskAsync(Guid id, UpdateTaskDto updateDto)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
        {
            return ApiResponseDto.NotFound($"Task with ID {id} not found");
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

        return ApiResponseDto.Success<TaskResponse>(new TaskResponse());
    }

    /// <summary>
    /// Delete task.
    /// </summary>
    public async Task<ApiResponseDto<object>> DeleteTaskAsync(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);

        if (task == null)
        {
            return ApiResponseDto.NotFound($"Task with ID {id} not found");
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();

        return ApiResponseDto.Success<object>(new { success = true, message = "Task has been deleted successfully" });
    }
}
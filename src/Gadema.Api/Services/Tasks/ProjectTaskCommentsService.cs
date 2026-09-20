using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Tasks;
using Gadema.Core.Dtos.Response;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Models.Tasks;
using Gadema.Core.Interfaces;

namespace Gadema.Api.Services.Tasks;

/// <summary>
/// Service for managing comments on tasks.
/// Comments are treated as leaf entities belonging to a ProjectTask.
/// </summary>
[ServiceLifetime(ServiceLifetime.Scoped)] public class ProjectTaskCommentService : CoreService
{
    public ProjectTaskCommentService(GameDbContext db, ILogger<ProjectTaskCommentService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    // ========================================================================
    // GET - List comments for a specific task
    // ========================================================================

    public async Task<ApiResponseDto<ListResponseDto<ProjectTaskCommentResponseDto>>> GetCommentsAsync(Guid taskId)
    {
        var comments = await _db.ProjectTaskComments
            .Where(c => c.ProjectTaskId == taskId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return ApiResponseDto<ListResponseDto<ProjectTaskCommentResponseDto>>.Success(new ListResponseDto<ProjectTaskCommentResponseDto>
        {
            Items = comments.Select(CreateResponseDto),
            TotalCount = comments.Count
        });
    }

    // ========================================================================
    // POST - Add a new comment
    // ========================================================================

    public async Task<ApiResponseDto<CreateResponseDto>> AddCommentAsync(Guid taskId, ProjectTaskCommentCreateDto createDto)
    {
        // 1. Verify the task exists and belongs to the user's project scope
        var task = await _db.ProjectTasks.FindAsync(taskId);
        if (task == null) 
            return ApiResponseDto<CreateResponseDto>.NotFound($"Task with ID {taskId} not found.");

        // Validate access to the project via the task
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(task.ProjectId);
        if (error != null) return error;

        // 2. Create the comment
        var comment = new ProjectTaskComment
        {
            Id = Guid.NewGuid(),
            ProjectTaskId = taskId,
            CommentedByUserId = createDto.CommentedByUserId,
            CommentText = createDto.CommentText,
            CreatedAt = DateTime.UtcNow
        };

        _db.ProjectTaskComments.Add(comment);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto 
        { 
            EntityId = comment.Id, 
            ProjectId = task.ProjectId 
        });
    }

    // ========================================================================
    // PUT - Update a comment's text
    // ========================================================================

    public async Task<ApiResponseDto<ProjectTaskCommentResponseDto>> UpdateCommentAsync(Guid id, ProjectTaskCommentUpdateDto updateDto)
    {
        var comment = await _db.ProjectTaskComments.FindAsync(id);
        if (comment is null)
            return ApiResponseDto<ProjectTaskCommentResponseDto>.NotFound($"Comment with ID {id} not found.");

        // Validate access via the task hierarchy
        var error = await ValidateProjectAccessAsync<ProjectTaskUpdateDto>(comment.ProjectTask.ProjectId);
        if (error != null) return ApiResponseDto<ProjectTaskCommentResponseDto>.Unauthorized("not authorized");

        // Update content
        comment.CommentText = updateDto.CommentText;
        await _db.SaveChangesAsync();

        return ApiResponseDto<ProjectTaskCommentResponseDto>.Success(CreateResponseDto(comment));
    }

    // ========================================================================
    // DELETE - Remove a comment
    // ========================================================================

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteCommentAsync(Guid id)
    {
        var comment = await _db.ProjectTaskComments.FindAsync(id);
        if (comment is null)
            return ApiResponseDto<DeleteResponseDto>.NotFound($"Comment with ID {id} not found.");

        // Validate access via the task hierarchy
        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(comment.ProjectTask.ProjectId);
        if (error != null) return error;

        _db.ProjectTaskComments.Remove(comment);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto 
        { 
            EntityId = id, 
            ProjectId = comment.ProjectTask.ProjectId 
        });
    }

    private ProjectTaskCommentResponseDto CreateResponseDto(ProjectTaskComment comment)
    {
        return new ProjectTaskCommentResponseDto
        {
            Id = comment.Id,
            ProjectTaskId = comment.ProjectTaskId,
            CommentedByUserId = comment.CommentedByUserId,
            CommentText = comment.CommentText,
            CreatedAt = comment.CreatedAt
        };
    }
}
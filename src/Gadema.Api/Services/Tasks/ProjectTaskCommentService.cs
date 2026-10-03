using Gadema.Api.CoreServices;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Tasks;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Core.Models.Tasks;
using Gadema.Data.Database.Core;
using Gadema.Data.Database.Writing;
using Microsoft.EntityFrameworkCore;
using Gadema.Data.Database.Tasks;

namespace Gadema.Api.Services.Tasks;

[ServiceLifetime(ServiceLifetime.Scoped)] public class ProjectTaskCommentService : DomainService
{
    private TaskDbContext _db;

    public ProjectTaskCommentService( TaskDbContext db,
        ILogger<ProjectTaskCommentService> logger,  
        ICoreServicesProvider coreServices) 
        : base(coreServices,logger) { _db = db; }

    public async Task<ApiResponseDto<ListResponseDto<ProjectTaskCommentResponseDto>>> GetCommentsAsync(Guid taskId)
    {
        var task = await _db.ProjectTasks
            .Include(t => t.MetaInfo)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null) return ApiResponseDto<ListResponseDto<ProjectTaskCommentResponseDto>>.NotFound($"Task {taskId} not found.");

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

    public async Task<ApiResponseDto<CreateResponseDto>> AddCommentAsync(Guid taskId, ProjectTaskCommentCreateDto createDto)
    {
        var task = await _db.ProjectTasks
            .Include(t => t.MetaInfo)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null) return ApiResponseDto<CreateResponseDto>.NotFound($"Task {taskId} not found.");

        var error = await CheckAccessAsync<CreateResponseDto>(task.ProjectId, Permission.CanEdit);
        if (error != null) return error;

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

    public async Task<ApiResponseDto<ProjectTaskCommentResponseDto>> UpdateCommentAsync(Guid id, ProjectTaskCommentUpdateDto updateDto)
    {
        var comment = await _db.ProjectTaskComments
            .Include(c => c.ProjectTask.MetaInfo)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment is null) return ApiResponseDto<ProjectTaskCommentResponseDto>.NotFound($"Comment {id} not found.");

        var error = await CheckAccessAsync<ProjectTaskCommentResponseDto>(comment.ProjectTask.ProjectId, Permission.CanEdit);
        if (error != null) return error;

        comment.CommentText = updateDto.CommentText;
        await _db.SaveChangesAsync();

        return ApiResponseDto<ProjectTaskCommentResponseDto>.Success(CreateResponseDto(comment));
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteCommentAsync(Guid id)
    {
        var comment = await _db.ProjectTaskComments
            .Include(c => c.ProjectTask.MetaInfo)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (comment is null) return ApiResponseDto<DeleteResponseDto>.NotFound($"Comment {id} not found.");

        var error = await CheckAccessAsync<DeleteResponseDto>(comment.ProjectTask.ProjectId, Permission.CanDelete);
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
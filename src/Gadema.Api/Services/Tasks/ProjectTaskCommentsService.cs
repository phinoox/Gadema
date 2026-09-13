// =============================================================================
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Dtos.Tasks;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Tasks;

/// <summary>
/// Service for managing ProjectTaskComments - comments on individual tasks.
/// </summary>
public class ProjectTaskCommentsService : CoreService
{
    public ProjectTaskCommentsService(GameDbContext db, ILogger<ProjectTaskCommentsService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    private TaskCommentResponseDto CreateResponseDto(ProjectTaskComment comment)
        => new()
        {
            Id = comment.Id,
            MetaInfoId = comment.MetaInfoId,
            ProjectTaskId = comment.ProjectTaskId, // FK as PK pattern
            Text = comment.Text,
            CreatedByUserId = comment.CreatedByUserId,
            CreatedAt = comment.CreatedAt,
        };

    private ListResponseDto<TaskCommentResponseDto> CreateListResponseDto(IEnumerable<ProjectTaskComment> comments)
        => new() { Items = comments.Select(CreateResponseDto).ToList(), TotalCount = comments.Count() };

    public async Task<ApiResponseDto<ListResponseDto<TaskCommentResponseDto>>> GetCommentsAsync(Guid projectId, Guid? taskId = null)
    {
        var error = await ValidateProjectAccessAsync<ListResponseDto<TaskCommentResponseDto>>(projectId);
        if (error != null) return error;

        var query = _db.ProjectTaskComments.Where(c => c.MetaInfo.ProjectId == projectId).OrderByDescending(c => c.CreatedAt);

        if (taskId.HasValue) query = query.Where(c => c.ProjectTaskId == taskId.Value || c.ProjectTaskId == Guid.Empty); // FK-as-PK pattern

        var comments = await query.ToListAsync();
        return ApiResponseDto<ListResponseDto<TaskCommentResponseDto>>.Success(CreateListResponseDto(comments));
    }

    public async Task<ApiResponseDto<TaskCommentResponseDto>> GetCommentByIdAsync(Guid id)
    {
        var comment = await _db.ProjectTaskComments.Include(c => c.CreatedByUser).FirstOrDefaultAsync(c => c.Id == id);
        if (comment is null) return ApiResponseDto<TaskCommentResponseDto>.NotFound($"Task comment with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<TaskCommentResponseDto>(comment.MetaInfo.ProjectId);
        if (error != null) return error;

        return ApiResponseDto<TaskCommentResponseDto>.Success(CreateResponseDto(comment));
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateCommentAsync(Guid projectId, Guid? taskId, string text)
    {
        var error = await ValidateProjectAccessAsync<CreateResponseDto>(projectId);
        if (error != null) return error;

        // Use FK-as-PK pattern: ProjectTaskId matches the comment's ID for junction table
        var comment = new ProjectTaskComment
        {
            Id = Guid.NewGuid(),
            MetaInfoId = Guid.Empty, // Not applicable for comments
            ProjectTaskId = taskId ?? Guid.Empty, // If taskId is null, use empty GUID
            Text = text,
            CreatedByUserId = _userContext.CurrentUser!.Id,
        };

        _db.ProjectTaskComments.Add(comment);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CreateResponseDto>.Success(new CreateResponseDto { EntityId = comment.Id, MetaInfoId = Guid.Empty, ProjectId = projectId });
    }

    public async Task<ApiResponseDto<TaskCommentResponseDto>> UpdateCommentAsync(Guid id, string text)
    {
        var comment = await _db.ProjectTaskComments.FirstOrDefaultAsync(c => c.Id == id);
        if (comment is null) return ApiResponseDto<TaskCommentResponseDto>.NotFound($"Task comment with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<TaskCommentResponseDto>(comment.MetaInfo.ProjectId);
        if (error != null) return error;

        comment.Text = text;
        await _db.SaveChangesAsync();
        return ApiResponseDto<TaskCommentResponseDto>.Success(CreateResponseDto(comment));
    }

    public async Task<ApiResponseDto<DeleteResponseDto>> DeleteCommentAsync(Guid id)
    {
        var comment = await _db.ProjectTaskComments.Include(c => c.CreatedByUser).FirstOrDefaultAsync(c => c.Id == id);
        if (comment is null) return ApiResponseDto<DeleteResponseDto>.NotFound($"Task comment with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<DeleteResponseDto>(comment.MetaInfo.ProjectId);
        if (error != null) return error;

        _db.ProjectTaskComments.Remove(comment);
        await _db.SaveChangesAsync();

        return ApiResponseDto<DeleteResponseDto>.Success(new DeleteResponseDto { EntityId = id, ProjectId = comment.MetaInfo.ProjectId });
    }
}
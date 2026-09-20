// ... imports ...
using Microsoft.EntityFrameworkCore;
using Gadema.Core.Dtos;
using Gadema.Data.Database;
using Gadema.Core.Interfaces;
using Gadema.Core.Dtos.Writing.Social;

namespace Gadema.Api.Services.Base.MetaInfo;

public class CommentService : CoreService
{
    public CommentService(GameDbContext db, ILogger<CommentService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    private CommentResponseDto MapToResponseDto(Comment comment)
        => new()
        {
            Id = comment.Id,
            TargetId = comment.TargetId,
            Text = comment.Text,
            AuthorUserId = comment.AuthorUserId,
            CreatedAt = comment.CreatedAt,
            ParentCommentId = comment.ParentCommentId
        };

    public async Task<ApiResponseDto<IEnumerable<CommentResponseDto>>> GetCommentsByTargetAsync(Guid projectId, Guid targetId)
    {
        var error = await ValidateProjectAccessAsync<IEnumerable<CommentResponseDto>>(projectId);
        if (error != null) return error;

        // We query comments directly. 
        // Note: To ensure the user isn't querying a target in another project, 
        // we check if that target exists in this project first.
        if (!await IsTargetInProjectAsync(projectId, targetId))
            return ApiResponseDto<IEnumerable<CommentResponseDto>>.BadRequest("Target content not found or access denied.");

        var comments = await _db.Comments
            .Where(c => c.TargetId == targetId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return ApiResponseDto<IEnumerable<CommentResponseDto>>.Success(comments.Select(MapToResponseDto));
    }


    public async Task<ApiResponseDto<CommentResponseDto>> CreateCommentAsync(Guid projectId, CreateCommentDto dto)
    {
        var error = await ValidateProjectAccessAsync<CommentResponseDto>(projectId);
        if (error != null) return error;

        // Use the new CoreService helper to verify ownership/existence in one step
        if (!await IsTargetInProjectAsync(projectId, dto.TargetId))
            return ApiResponseDto<CommentResponseDto>.BadRequest("Target content not found or access denied.");

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            TargetId = dto.TargetId,
            Text = dto.CommentText,
            AuthorUserId = _userContext.CurrentUser!.Id,
            ParentCommentId = dto.ParentCommentId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CommentResponseDto>.Success(MapToResponseDto(comment));
    }

     public async Task<ApiResponseDto<CommentResponseDto>> UpdateCommentAsync(Guid id, UpdateCommentDto dto)
    {
        var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == id);
        if (comment is null) return ApiResponseDto<CommentResponseDto>.NotFound("Comment not found.");

        // Verify access via the target's project ownership
        // We use the TargetId to check if the anchor belongs to this project
        var error = await ValidateProjectAccessAsync<CommentResponseDto>(comment.TargetId); 
        // Note: If your ValidateProjectAccessAsync expects a ProjectId, you might need 
        // an overload or to fetch the ProjectId from ContentMetaInfo first.
        if (error != null) return error;

        if (!string.IsNullOrWhiteSpace(dto.CommentText)) comment.Text = dto.CommentText;
        if (dto.ParentCommentId.HasValue) comment.ParentCommentId = dto.ParentCommentId;

        await _db.SaveChangesAsync();
        return ApiResponseDto<CommentResponseDto>.Success(MapToResponseDto(comment));
    }

    public async Task<ApiResponseDto<string>> DeleteCommentAsync(Guid id)
    {
        var comment = await _db.Comments.FirstOrDefaultAsync<Comment>(c => c.Id == id);
        if (comment is null) return ApiResponseDto<string>.NotFound("Comment not found.");

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();
        return ApiResponseDto<string>.Success("Deleted.");
    }
}
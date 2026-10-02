using Gadema.Api.CoreServices;
using Gadema.Api.CoreServices.Interfaces;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Writing.Social;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Access;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Data.Database.Core;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Base.MetaInfo;

public class CommentService : DomainService
{
    private readonly CoreDbContext _db;

    public CommentService(CoreDbContext db, ILogger<CommentService> logger, CoreServicesProvider coreServices) 
        : base(coreServices, logger)
    {
        _db = db;
    }

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
        var error = await CheckAccessAsync<IEnumerable<CommentResponseDto>>(projectId, Permission.CanView);
        if (error != null) return error;

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
        var error = await CheckAccessAsync<CommentResponseDto>(projectId, Permission.CanEdit);
        if (error != null) return error;

        if (!await IsTargetInProjectAsync(projectId, dto.TargetId))
            return ApiResponseDto<CommentResponseDto>.BadRequest("Target content not found or access denied.");

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            TargetId = dto.TargetId,
            Text = dto.CommentText,
            AuthorUserId = _userId,
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

        var meta = await _db.Set<BaseMetaInfo>().OfType<ContentMetaInfo>()
            .FirstOrDefaultAsync(m => m.Id == comment.TargetId);

        if (meta == null) return ApiResponseDto<CommentResponseDto>.BadRequest("Target content not found.");

        var error = await CheckAccessAsync<CommentResponseDto>(meta.ProjectId ?? Guid.Empty, Permission.CanEdit);
        if (error != null) return error;

        if (!string.IsNullOrWhiteSpace(dto.CommentText)) comment.Text = dto.CommentText;
        if (dto.ParentCommentId.HasValue) comment.ParentCommentId = dto.ParentCommentId;

        await _db.SaveChangesAsync();
        return ApiResponseDto<CommentResponseDto>.Success(MapToResponseDto(comment));
    }

    public async Task<ApiResponseDto<string>> DeleteCommentAsync(Guid id)
    {
        var comment = await _db.Comments.FirstOrDefaultAsync(c => c.Id == id);
        if (comment is null) return ApiResponseDto<string>.NotFound("Comment not found.");

        var meta = await _db.Set<BaseMetaInfo>().OfType<ContentMetaInfo>()
            .FirstOrDefaultAsync(m => m.Id == comment.TargetId);

        if (meta == null) return ApiResponseDto<string>.BadRequest("Target content not found.");

        var error = await CheckAccessAsync<string>(meta.ProjectId ?? Guid.Empty, Permission.CanDelete);
        if (error != null) return error;

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();
        return ApiResponseDto<string>.Success("Deleted.");
    }
}
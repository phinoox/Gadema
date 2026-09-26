using Gadema.Api.CoreServices;
using Gadema.Api.Services.Base.Projects;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Base.Infrastructure;
using Gadema.Core.Dtos.Writing.Social;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Base.Permissions;
using Gadema.Data.Database.Core;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Base.MetaInfo;

[ServiceLifetime(ServiceLifetime.Scoped)] public class CommentService : DomainService
{
    private CoreDbContext _db;

    public CommentService( CoreDbContext db,
        ILogger<ProjectService> logger,  
        CoreServicesProvider coreServices) // Injected via CoreService constructor
        : base(coreServices,logger)
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
        // 1. Check if user has permission to view the project scope
        var error = await CheckAccessAsync<IEnumerable<CommentResponseDto>>(projectId, Permission.CanView);
        if (error != null) return error;

        // 2. Verify target exists within this project
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
        // 1. Check if user has permission to edit the project scope
        var error = await CheckAccessAsync<CommentResponseDto>(projectId, Permission.CanEdit);
        if (error != null) return error;

        // 2. Verify target exists within this project
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

        // 1. Resolve the project from the target to check permissions
        // We need to find if this comment's target belongs to a project
        var meta = await _db.Set<BaseMetaInfo>().OfType<ContentMetaInfo>()
            .FirstOrDefaultAsync(m => m.Id == comment.TargetId);

        if (meta == null) return ApiResponseDto<CommentResponseDto>.BadRequest("Target content not found.");

        // 2. Check permission on the project scope
        var error = await CheckAccessAsync<CommentResponseDto>(meta.ProjectId.Value, Permission.CanEdit);
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

        // 1. Resolve project from target to check permissions
        var meta = await _db.Set<BaseMetaInfo>().OfType<ContentMetaInfo>()
            .FirstOrDefaultAsync(m => m.Id == comment.TargetId);

        if (meta == null) return ApiResponseDto<string>.BadRequest("Target content not found.");

        // 2. Check permission on the project scope
        var error = await CheckAccessAsync<string>(meta.ProjectId.Value, Permission.CanDelete);
        if (error != null) return error;

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();
        return ApiResponseDto<string>.Success("Deleted.");
    }
}
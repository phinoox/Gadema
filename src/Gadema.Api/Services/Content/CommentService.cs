// =============================================================================
using Gadema.Api.Controllers.Content;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Comments;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Content;

/// <summary>
/// Service for managing Comments on MetaInfo entities.
/// Supports pagination, author filtering, and nested replies (parentId).
/// </summary>
public class CommentService : CoreService
{
    public CommentService(GameDbContext db, ILogger<CommentService> logger, IUserContext userContext)
        : base(db, logger, userContext) { }

    private CommentResponseDto CreateResponseDto(Comment comment)
        => new()
        {
            Id = comment.Id,
            MetaInfoId = comment.MetaInfoId,
            Text = comment.Text,
            ParentId = comment.ParentId,
            CreatedByUserId = comment.CreatedByUserId,
            CreatedAt = comment.CreatedAt,
        };

    private ListResponseDto<CommentResponseDto> CreateListResponseDto(IEnumerable<Comment> comments)
        => new() { Items = comments.Select(CreateResponseDto).ToList(), TotalCount = comments.Count() };

    // ========================================================================
    // GET /api/v1/content/comments - Paginated list with filters
    // ========================================================================

    public async Task<ApiResponseDto<ListResponseDto<CommentResponseDto>>> GetCommentsAsync(
        Guid projectId,
        int? metaInfoId = null,
        string? authorEmail = null,
        int? parentId = null,
        int page = 1,
        int pageSize = 50)
    {
        var error = await ValidateProjectAccessAsync<ListResponseDto<CommentResponseDto>>(projectId);
        if (error != null) return error;

        IQueryable<Comment> query = _db.Comments
            .Include(c => c.CreatedByUser)
            .Where(c => c.MetaInfo.ProjectId == projectId)
            .OrderByDescending(c => c.CreatedAt);

        if (metaInfoId.HasValue) query = query.Where(c => c.MetaInfoId == metaInfoId.Value);
        if (!string.IsNullOrWhiteSpace(authorEmail))
            query = query.Where(c => c.CreatedByUser?.Email.Contains(authorEmail, StringComparison.OrdinalIgnoreCase) == true);
        if (parentId.HasValue) query = query.Where(c => c.ParentId == parentId.Value);

        var total = await query.CountAsync();
        var comments = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return ApiResponseDto<ListResponseDto<CommentResponseDto>>.Success(
            new ListResponseDto<CommentResponseDto> { Items = CreateListResponseDto(comments), TotalCount = total });
    }

    // ========================================================================
    // POST /api/v1/content/comments - Create a comment (root or reply)
    // ========================================================================
    public async Task<ApiResponseDto<CommentResponseDto>> CreateCommentAsync(
        Guid projectId,
        Guid metaInfoId,
        string text,
        Guid? parentId = null)
    {
        var error = await ValidateProjectAccessAsync<CommentResponseDto>(projectId);
        if (error != null) return error;

        // Validate parentId references a comment in the same project
        if (parentId.HasValue && !await _db.Comments.AnyAsync(c => c.Id == parentId.Value && c.MetaInfo.ProjectId == projectId))
            return ApiResponseDto<CommentResponseDto>.BadRequest("Parent comment not found or not in this project.");

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            MetaInfoId = metaInfoId,
            Text = text,
            ParentId = parentId,
            CreatedByUserId = _userContext.CurrentUser!.Id,
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();

        return ApiResponseDto<CommentResponseDto>.Success(CreateResponseDto(comment));
    }

    // ========================================================================
    // PUT /api/v1/content/comments/{id} - Update comment text
    // ========================================================================

    public async Task<ApiResponseDto<CommentResponseDto>> UpdateCommentAsync(Guid id, string text)
    {
        var comment = await _db.Comments.Include(c => c.CreatedByUser).FirstOrDefaultAsync(c => c.Id == id);
        if (comment is null) return ApiResponseDto<CommentResponseDto>.NotFound($"Comment with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<CommentUpdateDto>(comment.MetaInfo.ProjectId);
        if (error != null) return error;

        comment.Text = text;
        await _db.SaveChangesAsync();
        return ApiResponseDto<CommentResponseDto>.Success(CreateResponseDto(comment));
    }

    // ========================================================================
    // DELETE /api/v1/content/comments/{id} - Delete a comment
    // ========================================================================

    public async Task<ApiResponseDto<string>> DeleteCommentAsync(Guid id)
    {
        var comment = await _db.Comments.Include(c => c.CreatedByUser).FirstOrDefaultAsync(c => c.Id == id);
        if (comment is null) return ApiResponseDto<string>.NotFound($"Comment with ID {id} not found.");

        var error = await ValidateProjectAccessAsync<CommentUpdateDto>(comment.MetaInfo.ProjectId);
        if (error != null) return error;

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();
        return ApiResponseDto<string>.Success("Comment deleted successfully.");
    }
}
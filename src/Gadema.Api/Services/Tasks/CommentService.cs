// =============================================================================
using Gadema.Core.Dtos;
using Microsoft.EntityFrameworkCore;
// Gadema.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Gadema.Core.Dtos.Comments;
using Gadema.Core.Models;
using Gadema.Data.Database;
using Microsoft.Extensions.Logging;
using Gadema.Core.Services;

namespace Gadema.Api.Services;

/// <summary>
/// Implementation of comment service.
/// </summary>
public class CommentService : IGademaService,  ICommentService
{
    private readonly GameDbContext _context;
    private readonly ILogger<CommentService> _logger;

    public ServiceTypeEnum ServiceType => ServiceTypeEnum.CommentService;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public CommentService(GameDbContext context, ILogger<CommentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// List comments for content item.
    /// </summary>
    public async Task<ApiResponseDto<CommentListResponseDto>> GetCommentsAsync(Guid MetaInfoId, string? visibility = null)
    {
        var query = _context.Comments.Where(c => c.MetaInfoId == MetaInfoId);
        
        if (!string.IsNullOrWhiteSpace(visibility))
        {
            query = query.Where(c => c.Visibility == visibility);
        }

        var comments = await query
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return ApiResponseDto<CommentListResponseDto>.Success(new CommentListResponseDto());
    }

    /// <summary>
    /// Create comment on content item.
    /// </summary>
    public async Task<ApiResponseDto<CommentResponseDto>> CreateCommentAsync(Guid MetaInfoId, CreateCommentDto createDto)
    {
        var now = DateTime.UtcNow;

        // HTML escape to prevent XSS attacks
        var escapedCommentText = System.Net.WebUtility.HtmlEncode(createDto.CommentText);

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            MetaInfoId = MetaInfoId,
            CommentedByUserId = UserHelper.GetUserId(),
            CommentText = escapedCommentText,
            Visibility = createDto.Visibility ?? "private",
            CreatedAt = now
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return ApiResponseDto<CommentResponseDto>.Success(new CommentResponseDto());
    }
}
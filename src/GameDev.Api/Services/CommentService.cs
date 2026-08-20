// =============================================================================
// GameDev.Api - ASP.NET Core Web API Services
// =============================================================================

namespace GameDev.Api.Services;

/// <summary>
/// Implementation of comment service.
/// </summary>
public class CommentService : ICommentService
{
    private readonly GameDbContext _context;
    private readonly ILogger<CommentService> _logger;

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
    public async Task<ApiResponseDto<CommentListResponse>> GetCommentsAsync(Guid contentItemId, string? visibility = null)
    {
        var query = _context.Comments.Where(c => c.ContentItemId == contentItemId);
        
        if (!string.IsNullOrWhiteSpace(visibility))
        {
            query = query.Where(c => c.Visibility == visibility);
        }

        var comments = await query
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return ApiResponseDto.Success<CommentListResponse>(new CommentListResponse());
    }

    /// <summary>
    /// Create comment on content item.
    /// </summary>
    public async Task<ApiResponseDto<CommentResponse>> CreateCommentAsync(Guid contentItemId, CreateCommentDto createDto)
    {
        var now = DateTime.UtcNow;

        // HTML escape to prevent XSS attacks
        var escapedCommentText = System.Net.WebUtility.HtmlEncode(createDto.CommentText);

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            ContentItemId = contentItemId,
            CommentedByUserId = UserHelper.GetUserId(),
            CommentText = escapedCommentText,
            Visibility = createDto.Visibility ?? "private",
            CreatedAt = now
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return ApiResponseDto.Success<CommentResponse>(new CommentResponse());
    }
}
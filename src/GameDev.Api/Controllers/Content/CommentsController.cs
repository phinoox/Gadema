// =============================================================================
using Microsoft.AspNetCore.Http;
// GameDev.Api - ASP.NET Core Web API Controllers
// =============================================================================

using System;
using System.Threading.Tasks;
using GameDev.Api.Services;
using GameDev.Core.Dtos.Comments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameDev.Api.Controllers;

/// <summary>
/// Controller for comment endpoints.
/// </summary>
[ApiController]
[Route("api/v1/content-items/{id}/comments")]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly ILogger<CommentsController> _logger;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public CommentsController(ICommentService commentService, ILogger<CommentsController> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }

    /// <summary>
    /// List comments for content item.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetCommentsAsync(Guid id, [FromQuery] string? visibility = null)
    {
        return Ok(await _commentService.GetCommentsAsync(id, visibility));
    }

    /// <summary>
    /// Create comment on content item.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateCommentAsync(Guid id, [FromBody] CreateCommentDto createDto)
    {
        return Ok(await _commentService.CreateCommentAsync(id, createDto));
    }
}
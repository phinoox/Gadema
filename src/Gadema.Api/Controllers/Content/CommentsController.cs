using Gadema.Api.Services.Content;
using Gadema.Core.Dtos.Comments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gadema.Api.Controllers.Content;

[ApiController]
[Route("api/v1/projects/{projectId:guid}/comments")]
public class CommentsController : ControllerBase
{
    private readonly CommentService _commentService;
    private readonly ILogger<CommentsController> _logger;

    public CommentsController(CommentService commentService, ILogger<CommentsController> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetCommentsAsync(
        Guid projectId,
        [FromQuery] int? metaInfoId = null,
        [FromQuery] string? authorEmail = null,
        [FromQuery] int? parentId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        return Ok(await _commentService.GetCommentsAsync(projectId, metaInfoId, authorEmail, parentId, page, pageSize));
    }

    [HttpPost]
    public async Task<IActionResult> CreateCommentAsync(
        Guid projectId,
        [FromBody] CommentCreateDto createDto)
    {
        return Ok(await _commentService.CreateCommentAsync(projectId, createDto.MetaInfoId, createDto.Text, createDto.ParentId));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCommentAsync(Guid id, [FromBody] CommentUpdateDto updateDto)
    {
        return Ok(await _commentService.UpdateCommentAsync(id, updateDto.Text));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCommentAsync(Guid id)
    {
        return Ok(await _commentService.DeleteCommentAsync(id));
    }
}

public record CommentCreateDto(string Text, Guid MetaInfoId, Guid? ParentId = null);
public record CommentUpdateDto(string Text);
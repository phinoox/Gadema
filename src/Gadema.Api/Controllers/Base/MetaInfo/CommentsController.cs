namespace Gadema.Api.Controllers.Base.MetaInfo;

using Gadema.Api.Services.Base.MetaInfo;
using Gadema.Core.Dtos.Comments;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/projects/{projectId:guid}/content/{targetId:guid}/comments")]
public class CommentsController : ControllerBase
{
    private readonly CommentService _service;

    public CommentsController(CommentService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Get(Guid projectId, Guid targetId) 
        => Ok(await _service.GetCommentsByTargetAsync(projectId, targetId));

    [HttpPost]
    public async Task<IActionResult> Create(Guid projectId, Guid targetId, [FromBody] CreateCommentDto dto)
    {
        // Ensure the DTO's TargetId matches the route for integrity
        if (dto.TargetId != targetId) return BadRequest("Target ID mismatch.");
        return Ok(await _service.CreateCommentAsync(projectId, dto));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid projectId, Guid targetId, Guid id, [FromBody] UpdateCommentDto dto)
        => Ok(await _service.UpdateCommentAsync(id, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid projectId, Guid targetId, Guid id)
        => Ok(await _service.DeleteCommentAsync(id));
}
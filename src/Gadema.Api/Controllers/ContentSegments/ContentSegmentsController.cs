// =============================================================================
using Gadema.Api.Services;
using Gadema.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.ContentSegments;

/// <summary>
/// Controller for content segment management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/projects/{projectId}/content-segments")]
public class ContentSegmentsController : ControllerBase
{
    private readonly IContentSegmentService _contentSegmentService;
    private readonly ILogger<ContentSegmentsController> _logger;

    public ContentSegmentsController(IContentSegmentService contentSegmentService, ILogger<ContentSegmentsController> logger)
    {
        _contentSegmentService = contentSegmentService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetContentSegmentsAsync(Guid projectId)
    {
        return Ok(await _contentSegmentService.GetContentSegmentsAsync(projectId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContentSegmentAsync(Guid id)
    {
        return Ok(await _contentSegmentService.GetContentSegmentAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateContentSegmentAsync(Guid projectId, [FromBody] ContentSegmentCreateDto createDto)
    {
        return Ok(await _contentSegmentService.CreateContentSegmentAsync(projectId, createDto));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateContentSegmentAsync(Guid id, [FromBody] ContentSegmentUpdateDto updateDto)
    {
        return Ok(await _contentSegmentService.UpdateContentSegmentAsync(id, updateDto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteContentSegmentAsync(Guid id)
    {
        return Ok(await _contentSegmentService.DeleteContentSegmentAsync(id));
    }
}

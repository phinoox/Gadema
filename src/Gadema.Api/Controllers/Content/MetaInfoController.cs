// =============================================================================
using Microsoft.AspNetCore.Http;
// Gadema.Api - ASP.NET Core Web API Controllers
// =============================================================================

using System;
using System.Threading.Tasks;
using Gadema.Api.Services;
using Gadema.Core.Dtos.MetaInfos;
using Gadema.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Gadema.Api.Services.Content;

namespace Gadema.Api.Controllers;

/// <summary>
/// Controller for content item management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/content/items")]
public class MetaInfosController : ControllerBase
{
    private readonly ContentService _contentService;
    private readonly ILogger<MetaInfosController> _logger;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public MetaInfosController(ContentService contentService, ILogger<MetaInfosController> logger)
    {
        _contentService = contentService;
        _logger = logger;
    }

    /// <summary>
    /// List all content items (paginated).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMetaInfosAsync(
        [FromQuery] Guid? projectId = null,
        [FromQuery] ContentTypeEnum? contentType = null,
        [FromQuery] ContentStatusEnum? status = null,
        [FromQuery] bool published = true,
        [FromQuery] ViewModeEnum viewMode = ViewModeEnum.PrivateWriting)
    {
        return Ok(await _contentService.GetMetaInfosAsync(projectId, contentType, status, published, viewMode));
    }

    /// <summary>
    /// Get content item by ID.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetMetaInfoAsync(Guid id, [FromQuery] ViewModeEnum? viewMode = null)
    {
        return Ok(await _contentService.GetMetaInfoAsync(id, viewMode ?? ViewModeEnum.PrivateWriting));
    }

    /// <summary>
    /// Create/edit content item.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateMetaInfoAsync([FromBody] CreateMetaInfoDto createDto)
    {
        return Ok(await _contentService.CreateMetaInfoAsync(createDto));
    }

    /// <summary>
    /// Update content item.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMetaInfoAsync(Guid id, [FromBody] UpdateMetaInfoDto updateDto)
    {
        return Ok(await _contentService.UpdateMetaInfoAsync(id, updateDto));
    }

    /// <summary>
    /// Delete content item (admin only).
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMetaInfoAsync(Guid id)
    {
        return Ok(await _contentService.DeleteMetaInfoAsync(id));
    }

    /// <summary>
    /// Upload media file to content item.
    /// </summary>
    [HttpPost("{id}/upload")]
    public async Task<IActionResult> UploadMediaAsync(Guid id, [FromForm] IFormFile file)
    {
        return Ok(await _contentService.UploadMediaAsync(id, file));
    }

    /// <summary>
    /// Auto-save content item snapshot.
    /// </summary>
    [HttpPost("{id}/autosave")]
    public async Task<IActionResult> AutosaveAsync(Guid id)
    {
        return Ok(await _contentService.AutosaveAsync(id));
    }

    /// <summary>
    /// Rollback to previous version.
    /// </summary>
    [HttpPost("{id}/rollback")]
    public async Task<IActionResult> RollbackAsync(Guid id, [FromBody] RollbackDto rollbackDto)
    {
        return Ok(await _contentService.RollbackAsync(id, rollbackDto));
    }
}
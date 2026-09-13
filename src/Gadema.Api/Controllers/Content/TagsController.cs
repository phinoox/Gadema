// =============================================================================
using Microsoft.AspNetCore.Http;
// Gadema.Api - ASP.NET Core Web API Controllers
// =============================================================================

using System;
using System.Threading.Tasks;
using Gadema.Api.Services;
using Gadema.Core.Dtos.Tags;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gadema.Api.Controllers;

/// <summary>
/// Controller for tag management endpoints.
/// </summary>
[ApiController]
[Route("api/v1/content-items/{id}/tags")]
public class TagsController : ControllerBase
{
    private readonly TagsService _tagService;
    private readonly ILogger<TagsController> _logger;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public TagsController(TagService tagService, ILogger<TagsController> logger)
    {
        _tagService = tagService;
        _logger = logger;
    }

    /// <summary>
    /// List tags for content item.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetTagsAsync(Guid id)
    {
        return Ok(await _tagService.GetTagsAsync(id));
    }

    /// <summary>
    /// Add tags to content item.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddTagsAsync(Guid id, [FromBody] AddTagsDto addDto)
    {
        return Ok(await _tagService.AddTagsAsync(id, addDto));
    }
}
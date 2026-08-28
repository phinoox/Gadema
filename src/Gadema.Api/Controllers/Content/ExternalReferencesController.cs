// =============================================================================
using Microsoft.AspNetCore.Http;
// Gadema.Api - ASP.NET Core Web API Controllers
// =============================================================================

using System;
using System.Threading.Tasks;
using Gadema.Api.Services;
using Gadema.Core.Dtos.ExternalReferences;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gadema.Api.Controllers;

/// <summary>
/// Controller for external reference endpoints.
/// </summary>
[ApiController]
[Route("api/v1/content-items/{id}/references")]
public class ExternalReferencesController : ControllerBase
{
    private readonly IExternalReferenceService _referenceService;
    private readonly ILogger<ExternalReferencesController> _logger;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public ExternalReferencesController(IExternalReferenceService referenceService, ILogger<ExternalReferencesController> logger)
    {
        _referenceService = referenceService;
        _logger = logger;
    }

    /// <summary>
    /// List external references for content item.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetReferencesAsync(Guid id)
    {
        return Ok(await _referenceService.GetReferencesAsync(id));
    }

    /// <summary>
    /// Create external reference.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateReferenceAsync(Guid id, [FromBody] CreateExternalReferenceDto createDto)
    {
        return Ok(await _referenceService.CreateReferenceAsync(id, createDto));
    }
}
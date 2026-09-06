// =============================================================================
using Microsoft.AspNetCore.Http;
// Gadema.Api - ASP.NET Core Web API Controllers
// =============================================================================

using System.Threading.Tasks;
using Gadema.Api.Services;
using Gadema.Core.Dtos.Search;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gadema.Api.Controllers;

/// <summary>
/// Controller for search endpoints.
/// </summary>
[ApiController]
[Route("api/v1/search")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;
    private readonly ILogger<SearchController> _logger;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public SearchController(ISearchService searchService, ILogger<SearchController> logger)
    {
        _searchService = searchService;
        _logger = logger;
    }

    /// <summary>
    /// Search content items.
    /// </summary>
    [HttpPost("content-items")]
    public async Task<IActionResult> SearchMetaInfosAsync([FromBody] SearchMetaInfosDto searchDto)
    {
        return Ok(await _searchService.SearchMetaInfosAsync(searchDto));
    }
}
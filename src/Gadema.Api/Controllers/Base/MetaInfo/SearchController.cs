using Gadema.Api.Services.Search;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Base.MetaInfo;

[ApiController]
[Route("api/v1/search")]
internal class SearchController : ControllerBase
{
    private readonly SearchOrchestrator _orchestrator;

    internal SearchController(SearchOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    [HttpGet]
    public async Task<IActionResult> GlobalSearch(
        [FromQuery] string query, 
        [FromQuery] Guid? projectId, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Search query cannot be empty.");

        var results = await _orchestrator.GlobalSearchAsync(query, projectId, page, pageSize);
        return Ok(results);
    }
}
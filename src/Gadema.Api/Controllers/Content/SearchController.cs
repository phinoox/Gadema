using Gadema.Api.Services.Content;
using Gadema.Core.Dtos.Search;
using Gadema.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gadema.Api.Controllers.Content;

[ApiController]
[Route("api/v1/search")]
public class SearchController : ControllerBase
{
    private readonly ContentSearchService _searchService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(ContentSearchService searchService, ILogger<SearchController> logger)
    {
        _searchService = searchService;
        _logger = logger;
    }

    [HttpPost("content-items")]
    public async Task<IActionResult> SearchAsync([FromBody] SearchQueryDto searchQuery)
    {
        return Ok(await _searchService.SearchAsync(
            query: searchQuery.Query,
            contentType: searchQuery.ContentType,
            status: searchQuery.Status,
            tags: searchQuery.Tags?.ToArray(),
            page: searchQuery.Page ,
            pageSize: searchQuery.PageSize 
        ));
    }
}

public record SearchQueryDto(
    string Query,
    int? ContentType = null,
    ContentStatusEnum? Status = null,
    string[]? Tags = null,
    int Page = 1,
    int PageSize = 20);
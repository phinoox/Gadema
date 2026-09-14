using Gadema.Core.Dtos.Response;
using Gadema.Core.Dtos.Search;

using Microsoft.Extensions.Logging;

namespace Gadema.Api.Services.Search;

internal class SearchOrchestrator
{
    private readonly IEnumerable<ISearchableProvider> _providers;
    private readonly ILogger<SearchOrchestrator> _logger;

    public SearchOrchestrator(IEnumerable<ISearchableProvider> providers, ILogger<SearchOrchestrator> logger)
    {
        _providers = providers;
        _logger = logger;
    }

    /// <summary>
    /// Orchestrates a global search across all registered domain providers.
    /// </summary>
    public async Task<ListResponseDto<SearchHitDto>> GlobalSearchAsync(string query, Guid? projectId, int page, int pageSize)
    {
        try
        {
            var allHits = new List<SearchHitDto>();

            // 1. Gather results from all providers in parallel
            var tasks = _providers.Select(p => p.GetMatchesAsync(query, projectId));
            var results = await Task.WhenAll(tasks);

            foreach (var hitList in results)
            {
                allHits.AddRange(hitList);
            }

            // 2. Perform Global Pagination
            int totalCount = allHits.Count;
            var pagedResults = allHits
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new ListResponseDto<SearchHitDto>
            {
                Items = pagedResults,
                TotalCount = totalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during global search orchestration.");
            throw; // Let the controller handle the error response
        }
    }
}

internal interface ISearchableProvider
{
    Task<IEnumerable<SearchHitDto>> GetMatchesAsync(string query, Guid? projectId);
}
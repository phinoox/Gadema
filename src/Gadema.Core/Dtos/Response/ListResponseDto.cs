// =============================================================================
namespace Gadema.Core.Dtos.Response;

/// <summary>
/// Generic list response wrapper used for paginated list endpoints.
/// </summary>
public class ListResponseDto<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
}

// =============================================================================


/// <summary>
/// Generic paged response wrapper for list endpoints.
/// </summary>
public class PagedResponseDto<T> : ListResponseDto<T>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}

// =============================================================================


/// <summary>
/// Generic paginated list response wrapper.
/// </summary>
public class PaginatedListResponseDto<T> : PagedResponseDto<T>
{
    // No additional fields — just reuses PagedResponseDto behavior
}
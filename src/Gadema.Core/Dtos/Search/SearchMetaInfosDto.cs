using Gadema.Core.Dtos.Base.Infrastructure;

namespace Gadema.Core.Dtos.Search;

/// <summary>
/// Data transfer object used to define search criteria for content items (ContentMetaInfo).
/// </summary>
public class SearchMetaInfosDto
{
    /// <summary>
    /// The full-text query string. Supports AND/OR operators and wildcards.
    /// E.g., "dragon knight" OR "fire mage" dragon*
    /// </summary>
    [Required, MaxLength(512)] public string Query { get; set; } = "";

    /// <summary>
    /// Optional: filter by specific content type IDs (e.g., 0=Character, 4=Scene).
    /// If null, searches across all content types.
    /// </summary>
    public int[]? ContentTypeIds { get; set; }

    /// <summary>
    /// Optional: filter by workflow status IDs (e.g., Draft, InProgress, Published).
    /// If null, includes all statuses.
    /// </summary>
    public int[]? StatusIds { get; set; }

    /// <summary>
    /// Optional: limit search to specific projects. 
    /// If null or empty, searches across the user's entire library.
    /// </summary>
    public Guid[]? ProjectIds { get; set; }

    /// <summary>
    /// Optional: minimum relevance score threshold (0.0 - 1.0).
    /// Default is 0.0 (no filter).
    /// </summary>
    [Range(0.0, 1.0)] public double? MinScore { get; set; }

    /// <summary>
    /// Optional: whether to also search in description and long-text fields.
    /// Default is true for full-text relevance scoring.
    /// </summary>
    public bool IncludeDescription { get; set; } = true;

    /// <summary>
    /// Optional: minimum word count of the query term (filters out stop words).
    /// Useful to prevent over-matching on common words like "the" or "and".
    /// </summary>
    [Range(1, 50)] public int? MinWordLength { get; set; }

    /// <summary>
    /// Optional: exact phrase search. If true, "dragon knight" matches only as a specific sequence.
    /// If false (default), searches for individual terms.
    /// </summary>
    public bool ExactPhraseSearch { get; set; } = false;
}

/// <summary>
/// Represents an individual result from a search operation, including its relevance context.
/// Inherits basic meta-information state.
/// </summary>
public class SearchResultDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// The type of entity this search result refers to (e.g., "Character", "Scene").
    /// </summary>
    public string ContentType { get; set; } = "";

    /// <summary>
    /// Indicates whether the item is a direct match or part of a relationship chain.
    /// </summary>
    public bool IsDirectMatch { get; set; }

    /// <summary>
    /// The matched text snippet(s) from the source content, suitable for highlighting in UI.
    /// Null if no specific text was matched.
    /// </summary>
    [MaxLength(1024)] public string? MatchedSnippet { get; set; }

    /// <summary>
    /// The relevance score (0.0 - 1.0) indicating how well this result matches the query.
    /// Higher scores indicate higher relevance.
    /// </summary>
    [Range(0.0, 1.0)] public double RelevanceScore { get; set; }

    /// <summary>
    /// An array of field names where a match was found (e.g., ["Title", "Description"]).
    /// </summary>
    public string[] MatchedFields { get; set; } = Array.Empty<string>();
}

/// <summary>
/// A collection of search results, including query metadata and pagination.
/// </summary>
public class SearchResultsResponseDto
{
    /// <summary>
    /// The original query string that was processed.
    /// </summary>
    public string Query { get; set; } = "";

    /// <summary>
    /// Total number of matching items found across all projects.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Number of results returned in this page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Zero-based index of the first result shown (skip count).
    /// </summary>
    public int SkipCount { get; set; }

    /// <summary>
    /// The average relevance score across all returned results.
    /// Useful for ranking UI components.
    /// </summary>
    [Range(0.0, 1.0)] public double AverageScore { get; set; }

    /// <summary>
    /// Time taken to execute the search in milliseconds.
    /// </summary>
    [Display(Name = "Search Duration (ms)")]
    public int SearchDurationMs { get; set; }

    /// <summary>
    /// The actual list of results, sorted by relevance score descending.
    /// </summary>
    public IEnumerable<SearchResultDto> Results { get; set; } = Enumerable.Empty<SearchResultDto>();
}
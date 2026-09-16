// =============================================================================
using Gadema.Core.Dtos.Base.Infrastructure;

namespace Gadema.Core.Dtos.Search;

/// <summary>
/// DTO for searching content items (ContentMetaInfo search).
/// </summary>
public class SearchMetaInfosDto
{
    /// <summary>
    /// The full-text query string. Supports AND/OR operators and wildcards.
    /// E.g., "dragon knight" OR "fire mage" dragon*
    /// </summary>
    [Required, MaxLength(512)] public string Query { get; set; } = "";

    /// <summary>
    /// Optional: filter by specific content type(s).
    /// 0=Character, 1=StoryBeat, 2=LoreEntry, 3=PlotPoint, 4=Scene, etc.
    /// If null, search all types.
    /// </summary>
    public int[]? ContentTypeIds { get; set; }

    /// <summary>
    /// Optional: filter by status (Draft, InProgress, Published, Archived).
    /// If null, include all statuses.
    /// </summary>
    public int[]? StatusIds { get; set; }

    /// <summary>
    /// Optional: limit search to specific projects.
    /// If null or empty, search across user's entire library.
    /// </summary>
    public Guid[]? ProjectIds { get; set; }

    /// <summary>
    /// Optional: minimum relevance score threshold (0.0 - 1.0).
    /// Default is 0.0 (no filter).
    /// </summary>
    [Range(0.0, 1.0)] public double? MinScore { get; set; }

    /// <summary>
    /// Optional: whether to return full entity details or just metadata.
    /// "full" = include all fields including relationships.
    /// "metadata" = only basic info (Id, Title, Slug, Status).
    /// </summary>
    //public SearchDepthEnum SearchDepth { get; set; } = SearchDepthEnum.Metadata;

    /// <summary>
    /// Optional: whether to also search in description/long text fields.
    /// Default is true for full-text relevance scoring.
    /// </summary>
    public bool IncludeDescription { get; set; } = true;

    /// <summary>
    /// Optional: minimum word count of the query term (filters out stop words).
    /// Useful to prevent over-matching on common words like "the", "and".
    /// </summary>
    [Range(1, 50)] public int? MinWordLength { get; set; }

    /// <summary>
    /// Optional: exact phrase search. If true, "dragon knight" matches only as a phrase.
    /// If false (default), searches for individual terms.
    /// </summary>
    public bool ExactPhraseSearch { get; set; } = false;
}

/// <summary>
/// Response DTO for a single search result item. Inherits ContentMetaInfo state.
/// </summary>
public class SearchResultDto : MetaInfoResponseBaseDto
{
    /// <summary>
    /// The type of entity this search result refers to.
    /// </summary>
    public string ContentType { get; set; } = "";

    /// <summary>
    /// Whether the item is a direct match or part of a relationship chain.
    /// </summary>
    public bool IsDirectMatch { get; set; }

    /// <summary>
    /// The matched text snippet(s) from the search (highlighted).
    /// Null if no specific text was matched.
    /// </summary>
    [MaxLength(1024)] public string? MatchedSnippet { get; set; }

    /// <summary>
    /// The relevance score (0.0 - 1.0) indicating how well this result matches the query.
    /// Higher is more relevant.
    /// </summary>
    [Range(0.0, 1.0)] public double RelevanceScore { get; set; }

    /// <summary>
    /// Array of matched field names (e.g., ["Title", "Description"]).
    /// </summary>
    public string[] MatchedFields { get; set; } = Array.Empty<string>();
}

/// <summary>
/// List response for search results.
/// </summary>
public class SearchResultsResponseDto
{
    /// <summary>
    /// The original query that was searched.
    /// </summary>
    public string Query { get; set; } = "";

    /// <summary>
    /// Total number of matching items across all projects.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Number of results on this page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Zero-based index of the first result shown.
    /// </summary>
    public int SkipCount { get; set; }

    /// <summary>
    /// The average relevance score of returned results (useful for ranking UI).
    /// </summary>
    [Range(0.0, 1.0)] public double AverageScore { get; set; }

    /// <summary>
    /// Number of seconds the search took to execute.
    /// Useful for performance monitoring.
    /// </summary>
    [Display(Name = "Search Duration (ms)")]
    public int SearchDurationMs { get; set; }

    /// <summary>
    /// The actual results, sorted by relevance score descending.
    /// </summary>
    public IEnumerable<SearchResultDto> Results { get; set; } = Enumerable.Empty<SearchResultDto>();
}
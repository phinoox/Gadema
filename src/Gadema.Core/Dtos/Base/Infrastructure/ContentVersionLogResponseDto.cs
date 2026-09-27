namespace Gadema.Core.Dtos.Base.Infrastructure;

/// <summary>
/// Represents an entry in the version history of a content item.
/// </summary>
public class ContentVersionLogResponseDto
{
    /// <summary>
    /// The unique identifier for this log entry.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the meta-information record being tracked.
    /// </summary>
    public Guid MetaInfoId { get; set; }

    /// <summary>
    /// The unique identifier of the user who made this change.
    /// </summary>
    public Guid ChangedByUserId { get; set; }

    /// <summary>
    /// An optional description explaining what was changed in this version.
    /// </summary>
    public string? ChangeDescription { get; set; }

    /// <summary>
    /// The resulting version number after this change.
    /// </summary>
    public int VersionNumber { get; set; }

    /// <summary>
    /// The timestamp when the version was created in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// A paginated list response for content version history.
/// </summary>
public class ContentVersionLogListResponseDto
{
    /// <summary>
    /// The collection of version log entries.
    /// </summary>
    public IEnumerable<ContentVersionLogResponseDto> Items { get; set; } = Enumerable.Empty<ContentVersionLogResponseDto>();

    /// <summary>
    /// Total number of records available across all pages.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// The current page index (1-based).
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// The number of items per page.
    /// </summary>
    public int PageSize { get; set; }
}

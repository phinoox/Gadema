/// <summary>
/// Represents a single entry in the system's audit log.
/// </summary>
public class ActivityLogResponseDto
{
    /// <summary>
    /// The unique identifier for the activity log entry.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The action performed (e.g., "Created", "Updated", "Deleted").
    /// </summary>
    public string Action { get; set; } = "";

    /// <summary>
    /// The type of entity that was affected by this action.
    /// </summary>
    public string RelatedEntityType { get; set; } = "";

    /// <summary>
    /// The unique identifier of the related entity.
    /// </summary>
    public Guid? RelatedEntityId { get; set; }

    /// <summary>
    /// A human-readable description of the event.
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// The timestamp when the action occurred.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The project context to which this activity belongs.
    /// </summary>
    public Guid ProjectId { get; set; }
}

/// <summary>
/// A paginated list response for activity logs.
/// </summary>
public class ActivityLogListResponseDto
{
    /// <summary>
    /// The collection of activity log entries.
    /// </summary>
    public IEnumerable<ActivityLogResponseDto> Items { get; set; } = Enumerable.Empty<ActivityLogResponseDto>();

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

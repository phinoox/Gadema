// =============================================================================
namespace Gadema.Core.Dtos.Base.Infrastructure;

/// <summary>
/// Response DTO for an activity log entry.
/// </summary>
public class ActivityLogResponseDto
{
    public Guid Id { get; set; }
    public string Action { get; set; } = "";
    public string RelatedEntityType { get; set; } = "";
    public Guid? RelatedEntityId { get; set; }
    public string Description { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public Guid ProjectId { get; set; }
}

/// <summary>
/// List response for activity logs.
/// </summary>
public class ActivityLogListResponseDto
{
    public IEnumerable<ActivityLogResponseDto> Items { get; set; } = Enumerable.Empty<ActivityLogResponseDto>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

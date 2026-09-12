// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Activities;

/// <summary>
/// Response DTO for an activity log entry.
/// </summary>
public class ActivityLogResponseDto
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string? ProjectTitle { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public string EventType { get; set; } = "";
    public Guid? RelatedEntityId { get; set; }
    public int RelatedEntityType { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
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

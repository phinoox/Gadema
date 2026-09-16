// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Base.Infrastructure;

/// <summary>
/// Response DTO for a content version log entry.
/// </summary>
public class ContentVersionLogResponseDto
{
    public Guid Id { get; set; }
    public Guid MetaInfoId { get; set; }
    public Guid ChangedByUserId { get; set; }
    public string? ChangeDescription { get; set; }
    public int VersionNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// List response for content version logs.
/// </summary>
public class ContentVersionLogListResponseDto
{
    public IEnumerable<ContentVersionLogResponseDto> Items { get; set; } = Enumerable.Empty<ContentVersionLogResponseDto>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Activities;

/// <summary>
/// Response DTO for a token usage log entry.
/// </summary>
public class TokenUsageLogResponseDto
{
    public Guid Id { get; set; }
    public Guid TokenId { get; set; }
    public string? TokenName { get; set; }
    public Guid? ProjectId { get; set; }
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }
    public int Action { get; set; }
    public Guid? ContentId { get; set; }
    public DateTime UsedAt { get; set; }
}

/// <summary>
/// List response for token usage logs.
/// </summary>
public class TokenUsageLogListResponseDto
{
    public IEnumerable<TokenUsageLogResponseDto> Items { get; set; } = Enumerable.Empty<TokenUsageLogResponseDto>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

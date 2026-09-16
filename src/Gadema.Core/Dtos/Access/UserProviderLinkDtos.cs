// =============================================================================
using Gadema.Core.Enums;

namespace Gadema.Core.Dtos.Access;

/// <summary>
/// Response DTO for a user provider link.
/// </summary>
public class UserProviderLinkResponseDto
{
    public string UserId { get; set; } = "";
    public UserAuthProviderEnum Provider { get; set; }
    public DateTime LinkedAtUtc { get; set; }
    public string? ExternalSubjectId { get; set; }
}

/// <summary>
/// List response for user provider links.
/// </summary>
public class UserProviderLinkListResponseDto
{
    public IEnumerable<UserProviderLinkResponseDto> Items { get; set; } = Enumerable.Empty<UserProviderLinkResponseDto>();
}

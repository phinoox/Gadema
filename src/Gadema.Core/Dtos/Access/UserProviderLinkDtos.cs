using Gadema.Core.Models.Access.Enums;


/// <summary>
/// Represents a link between a user and an external authentication provider.
/// </summary>
public class UserProviderLinkResponseDto
{
    /// <summary>
    /// The unique identifier of the user.
    /// </summary>
    public string UserId { get; set; } = "";

    /// <summary>
    /// The type of external provider (e.g., Google).
    /// </summary>
    public UserAuthProviderEnum Provider { get; set; }

    /// <summary>
    /// The timestamp when the link was created in UTC.
    /// </summary>
    public DateTime LinkedAtUtc { get; set; }

    /// <summary>
    /// The unique identifier provided by the external service for this user.
    /// </summary>
    public string? ExternalSubjectId { get; set; }
}

/// <summary>
/// A collection of user provider links.
/// </summary>
public class UserProviderLinkListResponseDto
{
    /// <summary>
    /// The list of linked providers for the user.
    /// </summary>
    public IEnumerable<UserProviderLinkResponseDto> Items { get; set; } = Enumerable.Empty<UserProviderLinkResponseDto>();
}

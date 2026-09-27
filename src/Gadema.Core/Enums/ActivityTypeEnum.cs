namespace Gadema.Core.Enums;

/// <summary>
/// Represents the type of activity performed within the system.
/// </summary>
public enum ActivityTypeEnum
{
    /// <summary>
    /// Indicates a creation activity.
    /// </summary>
    Create = 0,
    Update = 1,
    Delete = 2,
    Login = 3,
    Logout = 4,
    TokenUsage = 5
}

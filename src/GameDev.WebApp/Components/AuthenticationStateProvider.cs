// =============================================================================
// GameDev.WebApp - Blazor Server Authentication State Provider
// =============================================================================

namespace GameDev.WebApp.Components;

using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

/// <summary>
/// Custom authentication state provider for Blazor Server app.
/// </summary>
public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    /// <summary>
    /// Current user claim identity.
    /// </summary>
    public ClaimsIdentity? User { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public CustomAuthenticationStateProvider()
    {
        // In production, authenticate against JWT tokens or session
    }

    /// <summary>
    /// Notify authentication state has changed.
    /// </summary>
    public override Task NotifyAuthenticationStateChangedAsync(Task<AuthenticationState> task)
    {
        return base.NotifyAuthenticationStateChangedAsync(task);
    }
}

/// <summary>
/// Custom user claims principal.
/// </summary>
public class CustomUserClaimsPrincipal : ClaimsPrincipal
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public CustomUserClaimsPrincipal()
        : base(new ClaimsIdentity()) { }
}
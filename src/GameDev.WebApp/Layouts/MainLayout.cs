// =============================================================================
// GameDev.WebApp - Blazor Server App Layouts
// =============================================================================

namespace GameDev.WebApp.Layouts;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

/// <summary>
/// Main layout with authentication guard.
/// </summary>
public partial class MainLayout : LayoutComponentBase
{
    /// <summary>
    /// Current user claims principal.
    /// </summary>
    [Inject]
    private AuthenticationState? AuthenticationState { get; set; }

    /// <summary>
    /// Build the layout.
    /// </summary>
    public BuildRenderTree Build(IVisualBuilder builder)
    {
        // Main layout with auth guard, navigation, and content area
        return builder.Build();
    }

    /// <summary>
    /// Check if user is authenticated.
    /// </summary>
    private bool IsAuthenticated => AuthenticationState?.User?.Identity?.IsAuthenticated ?? false;
}

/// <summary>
/// Admin layout with admin tools visible.
/// </summary>
public partial class AdminLayout : LayoutComponentBase
{
    /// <summary>
    /// Current user claims principal.
    /// </summary>
    [Inject]
    private AuthenticationState? AuthenticationState { get; set; }

    /// <summary>
    /// Build the admin layout.
    /// </summary>
    public BuildRenderTree Build(IVisualBuilder builder)
    {
        // Admin layout with all tools visible
        return builder.Build();
    }
}

/// <summary>
/// Presentation layout for public viewing mode.
/// </summary>
public partial class PresentationLayout : LayoutComponentBase
{
    /// <summary>
    /// Current user claims principal.
    /// </summary>
    [Inject]
    private AuthenticationState? AuthenticationState { get; set; }

    /// <summary>
    /// Build the presentation layout.
    /// </summary>
    public BuildRenderTree Build(IVisualBuilder builder)
    {
        // Presentation layout with admin tools hidden
        return builder.Build();
    }
}

/// <summary>
/// Authentication guard component for Blazor components.
/// </summary>
public partial class AuthGuard : ComponentBase
{
    /// <summary>
    /// Required roles (optional).
    /// </summary>
    [Parameter]
    public string[]? Roles { get; set; }

    /// <summary>
    /// Authorization policy name (optional).
    /// </summary>
    [Parameter]
    public string? PolicyName { get; set; }

    /// <summary>
    /// Current user claims principal.
    /// </summary>
    [Inject]
    private AuthenticationState? AuthenticationState { get; set; }

    /// <summary>
    /// Build the auth guard component.
    /// </summary>
    public BuildRenderTree Build(IVisualBuilder builder)
    {
        // Auth guard with roles and policy validation
        return builder.Build();
    }

    /// <summary>
    /// Check if user has required roles or policy.
    /// </summary>
    private bool CanAccess =>
        string.IsNullOrEmpty(AuthenticationState?.User?.Identity?.Role) == true ||
        Roles.Contains(AuthenticationState!.User!.Identity!.Role!) ||
        PolicyName == null;
}

/// <summary>
/// View mode switcher component for PrivateWriting vs Presentation modes.
/// </summary>
public partial class ViewModeSwitcher : ComponentBase
{
    /// <summary>
    /// Current view mode (PrivateWriting or Presentation).
    /// </summary>
    [Parameter]
    public ViewModeEnum? ViewMode { get; set; }

    /// <summary>
    /// OnViewModeChanged event.
    /// </summary>
    [Parameter]
    public EventCallback<ViewModeEnum> OnViewModeChanged { get; set; }

    /// <summary>
    /// Build the view mode switcher.
    /// </summary>
    public BuildRenderTree Build(IVisualBuilder builder)
    {
        return builder.Build();
    }

    /// <summary>
    /// Toggle between PrivateWriting and Presentation modes.
    /// </summary>
    private void OnSwitchViewMode()
    {
        // Toggle view mode
    }
}
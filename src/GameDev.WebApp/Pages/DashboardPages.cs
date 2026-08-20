// =============================================================================
// GameDev.WebApp - Blazor Server App Pages
// =============================================================================

namespace GameDev.WebApp.Pages;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// Main overview page (dashboard).
/// </summary>
[Authorize]
public class Overview : PageComponent
{
    /// <summary>
    /// Constructor.
    /// </summary>
    [Inject]
    private NavigationManager? NavigationManager { get; set; }

    /// <summary>
    /// Render the page.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        // Load user projects and display dashboard
    }

    /// <summary>
    /// Build the component.
    /// </summary>
    public BuildRenderTree Build(IVisualBuilder builder)
    {
        // Overview page content
        return builder.Build();
    }
}

/// <summary>
/// Projects list page.
/// </summary>
[Authorize]
public class Projects : PageComponent
{
    /// <summary>
    /// Constructor.
    /// </summary>
    [Inject]
    private NavigationManager? NavigationManager { get; set; }

    /// <summary>
    /// Render the page.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        // Load and display project list
    }

    /// <summary>
    /// Build the component.
    /// </summary>
    public BuildRenderTree Build(IVisualBuilder builder)
    {
        return builder.Build();
    }
}

/// <summary>
/// Content items page.
/// </summary>
[Authorize]
public class ContentItems : PageComponent
{
    /// <summary>
    /// Constructor.
    /// </summary>
    [Inject]
    private NavigationManager? NavigationManager { get; set; }

    /// <summary>
    /// Render the page.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        // Load and display content items
    }

    /// <summary>
    /// Build the component.
    /// </summary>
    public BuildRenderTree Build(IVisualBuilder builder)
    {
        return builder.Build();
    }
}

/// <summary>
/// Tasks page (ADHD-friendly flat task structure).
/// </summary>
[Authorize]
public class Tasks : PageComponent
{
    /// <summary>
    /// Constructor.
    /// </summary>
    [Inject]
    private NavigationManager? NavigationManager { get; set; }

    /// <summary>
    /// Render the page.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        // Load and display flat task structure
    }

    /// <summary>
    /// Build the component.
    /// </summary>
    public BuildRenderTree Build(IVisualBuilder builder)
    {
        return builder.Build();
    }
}

/// <summary>
/// Story outlining page (chapter/sequence management).
/// </summary>
[Authorize]
public class StoryOutlining : PageComponent
{
    /// <summary>
    /// Constructor.
    /// </summary>
    [Inject]
    private NavigationManager? NavigationManager { get; set; }

    /// <summary>
    /// Render the page.
    /// </summary>
    protected override async Task OnInitializedAsync()
    {
        // Load and display story outlines
    }

    /// <summary>
    /// Build the component.
    /// </summary>
    public BuildRenderTree Build(IVisualBuilder builder)
    {
        return builder.Build();
    }
}
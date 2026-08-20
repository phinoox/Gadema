// =============================================================================
// GameDev.WebApp - Blazor Server App Components
// =============================================================================

namespace GameDev.WebApp.Components;

using Microsoft.AspNetCore.Components;

/// <summary>
/// Navigation menu component.
/// </summary>
public partial class NavigationMenu : ComponentBase
{
    /// <summary>
    /// Current user claims principal.
    /// </summary>
    [Inject]
    private AuthenticationState? AuthenticationState { get; set; }

    /// <summary>
    /// Current page route.
    /// </summary>
    [Inject]
    private NavigationManager? NavigationManager { get; set; }

    /// <summary>
    /// Build the navigation menu.
    /// </summary>
    public BuildRenderTree Build(IVisualBuilder builder)
    {
        return builder.Build();
    }
}

/// <summary>
/// User profile dropdown component.
/// </summary>
public partial class UserProfileDropdown : ComponentBase
{
    /// <summary>
    /// Current user claims principal.
    /// </summary>
    [Inject]
    private AuthenticationState? AuthenticationState { get; set; }

    /// <summary>
    /// Build the user profile dropdown.
    /// </summary>
    public BuildRenderTree Build(IVisualBuilder builder)
    {
        return builder.Build();
    }
}

/// <summary>
/// Task list component (ADHD-friendly flat structure).
/// </summary>
public partial class TaskList : ComponentBase
{
    /// <summary>
    /// List of tasks.
    /// </summary>
    [Parameter]
    public List<Task> Tasks { get; set; } = new();

    /// <summary>
    /// Quick win filter toggle.
    /// </summary>
    [Parameter]
    public bool IsQuickWinFilterEnabled { get; set; }

    /// <summary>
    /// Build the task list component.
    /// </summary>
    public BuildRenderTree Build(IVisualBuilder builder)
    {
        return builder.Build();
    }

    /// <summary>
    /// Filter tasks by difficulty for ADHD-friendly focus mode.
    /// </summary>
    private List<Task> GetQuickWinTasks =>
        IsQuickWinFilterEnabled ? Tasks.Where(t => t.IsQuickWin).ToList() : Tasks;
}

/// <summary>
/// Content item card component.
/// </summary>
public partial class ContentItemCard : ComponentBase
{
    /// <summary>
    /// Content item data.
    /// </summary>
    [Parameter]
    public ContentItemResponseDto? ContentItem { get; set; }

    /// <summary>
    /// Build the content item card.
    /// </summary>
    public BuildRenderTree Build(IVisualBuilder builder)
    {
        return builder.Build();
    }
}

/// <summary>
/// Story sequence outline component.
/// </summary>
public partial class StorySequenceOutline : ComponentBase
{
    /// <summary>
    /// List of story sequences.
    /// </summary>
    [Parameter]
    public List<StorySequence> Sequences { get; set; } = new();

    /// <summary>
    /// Build the story sequence outline.
    /// </summary>
    public BuildRenderTree Build(IVisualBuilder builder)
    {
        return builder.Build();
    }
}

/// <summary>
/// External reference link component.
/// </summary>
public partial class ExternalReferenceLink : ComponentBase
{
    /// <summary>
    /// External reference data.
    /// </summary>
    [Parameter]
    public ExternalReference Response { get; set; } = null!;

    /// <summary>
    /// Build the external reference link.
    /// </summary>
    public BuildRenderTree Build(IVisualBuilder builder)
    {
        return builder.Build();
    }
}

/// <summary>
/// Activity feed component.
/// </summary>
public partial class ActivityFeed : ComponentBase
{
    /// <summary>
    /// List of activity logs.
    /// </summary>
    [Parameter]
    public List<ActivityLog> Activities { get; set; } = new();

    /// <summary>
    /// Number of days to show.
    /// </summary>
    [Parameter]
    public int DaysToShow { get; set; } = 7;

    /// <summary>
    /// Build the activity feed.
    /// </summary>
    public BuildRenderTree Build(IVisualBuilder builder)
    {
        return builder.Build();
    }
}

/// <summary>
/// Loading spinner component.
/// </summary>
public partial class LoadingSpinner : ComponentBase
{
    /// <summary>
    /// Loading text (optional).
    /// </summary>
    [Parameter]
    public string? LoadingText { get; set; }

    /// <summary>
    /// Build the loading spinner.
    /// </summary>
    public BuildRenderTree Build(IVisualBuilder builder)
    {
        return builder.Build();
    }
}

/// <summary>
/// Modal dialog component.
/// </summary>
public partial class ModalDialog : ComponentBase
{
    /// <summary>
    /// Is modal visible.
    /// </summary>
    [Parameter]
    public bool IsVisible { get; set; }

    /// <summary>
    /// OnIsVisibleChanged event.
    /// </summary>
    [Parameter]
    public EventCallback<bool> OnIsVisibleChanged { get; set; }

    /// <summary>
    /// Build the modal dialog.
    /// </summary>
    public BuildRenderTree Build(IVisualBuilder builder)
    {
        return builder.Build();
    }

    /// <summary>
    /// Close the modal dialog.
    /// </summary>
    private void OnClose()
    {
        IsVisible = false;
    }
}

/// <summary>
/// Empty state component for when no data is available.
/// </summary>
public partial class EmptyState : ComponentBase
{
    /// <summary>
    /// Title for empty state.
    /// </summary>
    [Parameter]
    public string? Title { get; set; } = "No Data Available";

    /// <summary>
    /// Description for empty state.
    /// </summary>
    [Parameter]
    public string? Description { get; set; } = "There is no data to display.";

    /// <summary>
    /// Build the empty state.
    /// </summary>
    public BuildRenderTree Build(IVisualBuilder builder)
    {
        return builder.Build();
    }
}

/// <summary>
/// Pagination component.
/// </summary>
public partial class Pagination : ComponentBase
{
    /// <summary>
    /// Current page number.
    /// </summary>
    [Parameter]
    public int CurrentPage { get; set; } = 1;

    /// <summary>
    /// Total number of pages.
    /// </summary>
    [Parameter]
    public int TotalPages { get; set; } = 0;

    /// <summary>
    /// OnPageChanged event.
    /// </summary>
    [Parameter]
    public EventCallback<int> OnPageChanged { get; set; }

    /// <summary>
    /// Build the pagination component.
    /// </summary>
    public BuildRenderTree Build(IVisualBuilder builder)
    {
        return builder.Build();
    }

    /// <summary>
    /// Navigate to next page.
    /// </summary>
    private void OnNextPage()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Navigate to previous page.
    /// </summary>
    private void OnPreviousPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            StateHasChanged();
        }
    }
}
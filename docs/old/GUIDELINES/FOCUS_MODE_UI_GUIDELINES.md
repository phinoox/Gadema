# 🧠 Focus Mode UI Implementation Guidelines for GaDeMa v0.1

This document provides detailed guidance on implementing ADHD-friendly focus mode features including single-task views, quick win badges, and difficulty filtering.

## Single-Task View Implementation

### Hide Sidebars in Focus Mode

```csharp
// ✅ CORRECT - Single-task view hides all sidebars and navigation
public class FocusModeService : IGademaService,  IFocusModeService
{
    private readonly GameDbContext _context;
    
    public async Task<List<ProjectTask>> GetFocusModeTasksAsync(Guid projectId)
    {
        // Return only active task, hide project sidebar
        var activeTask = await _context.ProjectTasks
            .Where(t => t.ProjectId == projectId && 
                       t.Status == (int)TaskStatusEnum.InProgress)
            .Include(t => t.Comments)
            .FirstOrDefaultAsync();
        
        if (activeTask != null)
        {
            return new List<ProjectTask> { activeTask };  // Only one task visible
        }
        
        return [];  // No active task
    }
    
    public void HideProjectSidebar()
    {
        // Implementation to hide project sidebar in focus mode
        // Blazor: <div class="sidebar-hidden" style="visibility: hidden;">
        return;
    }
}

// ✅ CORRECT - Blazor component with single-task view
@code {
    private ProjectTask ActiveTask { get; set; }
    
    [Parameter] public Guid ProjectId { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        ActiveTask = await _focusModeService.GetFocusModeTasksAsync(ProjectId).FirstOrDefault();
    }
}

<div class="focus-mode-view">
    @if (ActiveTask != null)
    {
        <div class="single-task-container">
            <h1>@ActiveTask.TaskTitle</h1>
            
            <div class="task-details">
                <p><strong>Status:</strong> @(GetTaskStatusName(ActiveTask.Status))</p>
                <p><strong>Difficulty:</strong> @(GetDifficultyName(ActiveTask.Difficulty))</p>
                @if (ActiveTask.IsQuickWin)
                {
                    <span class="quick-win-badge">⭐ Quick Win!</span>
                }
                
                <div class="estimated-time">@ActiveTask.EstimatedMinutes minutes</div>
            </div>
            
            <textarea 
                class="task-editor"
                placeholder="Working on..."
                rows="10"
                @bind-Value="ActiveTask.Description" />
            
            <button class="complete-task-btn" onclick="@() => CompleteTask(ActiveTask.Id)">
                ✅ Complete Task
            </button>
        </div>
    }
    else
    {
        <p>No active task. Click "Create New Task" to start working.</p>
    }
</div>

<div class="sidebar-hidden">
    <!-- Sidebar is hidden in focus mode -->
</div>
```

---

## Quick Win Badge Implementation

### Visual Badge Component

```razor
@* Blazor component for quick win badge *@
<QuickWinBadge Task="@task" OnToggle="@OnToggleQuickWin" />

@code {
    [Parameter] public ProjectTask Task { get; set; }
    
    [Parameter] public EventCallback<bool> OnToggleQuickWin { get; set; }
}

<div class="quick-win-badge @(task.IsQuickWin ? "visible" : "hidden")">
    <span>⭐ Quick Win</span>
    <button 
        class="toggle-quick-win-btn" 
        onclick="@(() => ToggleQuickWin(task.Id))"
        @onclick:preventDefault>
        @(task.IsQuickWin ? "✕" : "+")
    </button>
</div>

@functions {
    private async Task ToggleQuickWin(Guid taskId)
    {
        var updatedTask = await _projectTaskService.ToggleQuickWin(taskId);
        await OnToggleQuickWin.InvokeAsync(updatedTask.IsQuickWin);
    }
}
```

---

## Difficulty Filtering UI Controls

### Filter Dropdown Component

```razor
@* Blazor dropdown for difficulty filtering *@
<DifficultyFilter 
    CurrentDifficulty="@currentDifficulty" 
    OnFilterChange="@OnFilterChange" />

@code {
    [Parameter] public int CurrentDifficulty { get; set; }  // Easy(0), Medium(1), Hard(2)
    
    [Parameter] public EventCallback<int> OnFilterChange { get; set; }
}
```

---

## Estimated Minutes Display

### Time Estimation Component

```razor
@* Display estimated time with visual indicator *@
<TaskEstimatedTime @task="@task" />

@code {
    [Parameter] public ProjectTask Task { get; set; }
}

<div class="estimated-time-indicator">
    <svg class="time-icon" viewBox="0 0 24 24">
        <circle cx="12" cy="12" r="10" />
        <path d="M12 6v6l4 2" />
    </svg>
    <span>@(Task.EstimatedMinutes.HasValue ? Math.Round(Task.EstimatedMinutes.Value) : "N/A") minutes</span>
    
    @if (Task.EstimatedMinutes.HasValue && Task.EstimatedMinutes.Value <= 15)
    {
        <span class="quick-win-indicator">⚡ Short task!</span>
    }
    else if (Task.EstimatedMinutes.HasValue && Task.EstimatedMinutes.Value > 60)
    {
        <span class="long-task-indicator">🕐 Long task</span>
    }
</div>

<style>
    .quick-win-indicator { color: #2ecc71; font-size: 0.9rem; margin-left: 8px; }
    .long-task-indicator { color: #e74c3c; font-size: 0.9rem; margin-left: 8px; }
</style>
```

---

## Task List Sorting by Quick Wins and Difficulty

### Sort Algorithm Implementation

```csharp
public class TaskSortService : IGademaService,  ITaskSortService
{
    private readonly GameDbContext _context;
    
    public async Task<List<ProjectTask>> GetSortedTasksAsync(Guid projectId)
    {
        // Priority: 1. Quick wins, 2. Easy tasks, 3. Medium tasks, 4. Hard tasks
        var sortedTasks = await _context.ProjectTasks
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.IsQuickWin ? 0 : 1)  // Quick wins first
            .ThenBy(t => t.Difficulty)  // Easy tasks next (Easy=0, Medium=1, Hard=2)
            .Include(t => t.Comments)
            .ToListAsync();
        
        return sortedTasks;
    }
    
    public async Task<List<ProjectTask>> GetQuickWinFirstAsync(Guid projectId)
    {
        // Separate quick wins and non-quick wins
        var quickWins = await _context.ProjectTasks
            .Where(t => t.ProjectId == projectId && t.IsQuickWin)
            .Include(t => t.Comments)
            .ToListAsync();
        
        var otherTasks = await _context.ProjectTasks
            .Where(t => t.ProjectId == projectId && !t.IsQuickWin)
            .Include(t => t.Comments)
            .ToListAsync();
        
        // Combine: quick wins + sorted by difficulty, then non-quick wins
        return [.. quickWins.OrderBy(t => t.Difficulty), .. otherTasks.OrderBy(t => t.Difficulty)];
    }
}
```

---

## "Getting Started" Mode for Overwhelmed Users

### Simplified Task View

```csharp
public class GettingStartedService : IGademaService,  IGettingStartedService
{
    private readonly GameDbContext _context;
    
    public async Task<GettingStartedTask> GetFirstQuickWinAsync(Guid projectId)
    {
        // Find the easiest quick win task to get started
        var firstTask = await _context.ProjectTasks
            .Where(t => t.ProjectId == projectId && 
                       t.IsQuickWin && 
                       t.Difficulty == 0)  // Easy tasks only
            .Include(t => t.Comments)
            .OrderBy(t => t.EstimatedMinutes ?? 15)  // Shortest first
            .FirstOrDefaultAsync();
        
        return new GettingStartedTask 
        {
            Task = firstTask,
            Message = "Why not start with this quick win? ⚡",
            EncouragingText = "Quick wins build momentum! Take a bite-sized step."
        };
    }
    
    public async Task<List<ProjectTask>> GetEasyTasksAsync(Guid projectId)
    {
        // Return only easy tasks for overwhelmed users
        return await _context.ProjectTasks
            .Where(t => t.ProjectId == projectId && t.Difficulty == 0)  // Easy tasks only
            .Include(t => t.Comments)
            .OrderBy(t => t.EstimatedMinutes ?? 15)
            .ToListAsync();
    }
}

// ✅ CORRECT - Blazor "Getting Started" UI component
<div class="getting-started-panel">
    <div class="panel-header">
        <h2>🚀 Ready to Start?</h2>
        <p class="encouraging-text">Don't feel overwhelmed!</p>
    </div>
    
    <div class="quick-win-suggestion">
        <svg viewBox="0 0 24 24" width="24" height="24" fill="#f1c40f">
            <path d="M12 2l3.09 6.26L22 9.27l-5 4.87 1.18 6.88L12 17.77l-6.18 3.25L7 14.14 2 9.27l6.91-1.01L12 2z"/>
        </svg>
        
        <h3>Suggested Quick Win:</h3>
        @if (firstTask != null)
        {
            <div class="suggested-task-card">
                <h4>@firstTask.TaskTitle</h4>
                <p>@(firstTask.EstimatedMinutes?.ToString("0")) minutes</p>
                @(firstTask.IsQuickWin ? "<span>⭐ Quick Win Badge</span>" : "")
            </div>
        }
        else
        {
            <p class="no-quick-wins">No quick wins available. Let's start with something easy!</p>
        }
        
        <button class="start-task-btn" onclick="@() => StartTask(firstTask?.Id)">
            👉 Let's Go!
        </button>
    </div>
</div>
```

---

## Keyboard Shortcuts for ADHD-Friendly Navigation

### Shortcut Implementation

```csharp
public interface IFocusModeShortcutsService : IGademaService,  ISnapshot
{
    Task FocusOnActiveTaskAsync();  // Ctrl+Shift+F
    Task NextQuickWinAsync();        // Ctrl+Shift+1
    Task PreviousQuickWinAsync();    // Ctrl+Shift+0
    Task ToggleFocusModeAsync();     // F11
}

@code {
    [CascadingParameter] private NavigationManager NavManager { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        // Setup keyboard shortcuts for focus mode
        Window.SetTimeout(1000, () => 
        {
            window.addEventListener('keydown', (e) => 
            {
                if (e.ctrlKey && e.shiftKey)
                {
                    switch(e.key)
                    {
                        case '1':
                            // Navigate to next quick win task
                            _focusModeService.NextQuickWin().then(task => {
                                document.getElementById('task-list').scrollIntoView();
                            });
                            break;
                        case '0':
                            // Navigate to previous quick win task
                            _focusModeService.PreviousQuickWin();
                            break;
                    }
                }
                
                if (e.ctrlKey && e.shiftKey && e.key === 'F')
                {
                    // Focus on active task
                    await _focusModeService.FocusOnActiveTaskAsync();
                }
            });
        });
    }
}
```

---

## Summary: Focus Mode Implementation Checklist

| Feature | Must Implement? | Priority | Notes |
| :--- | :--- | :--- | :--- |
| **Single-Task View** | ✅ Yes | High | Hide sidebars, show only active task |
| **Quick Win Badges** | ✅ Yes | High | Visual indicators with toggle functionality |
| **Difficulty Filtering UI** | ✅ Yes | High | Dropdown controls for Easy/Medium/Hard |
| **Estimated Minutes Display** | ✅ Yes | Medium | Show time estimates with visual indicators |
| **"Getting Started" Mode** | ✅ Yes | Medium | Simplified view for overwhelmed users |
| **Keyboard Shortcuts** | ⚠️ Optional | Low | Advanced navigation shortcuts (F11, Ctrl+Shift+) |

---

## Anti-Patterns to Avoid

| Anti-Pattern | Example | ✅ Correct Approach |
| :--- | :--- | :--- |
| **Showing All Tasks in Focus Mode** | Displaying entire task list in focus mode | ❌ Don't do this! Show only active/first task |
| **Hidden Time Estimates** | Not displaying estimated minutes | ✅ Always show time estimates with visual indicators |
| **Missing Quick Win Toggle** | Can't toggle quick win badge | ✅ Include toggle button for each task |
| **No "Getting Started" Option** | No simplified view available | ✅ Provide "Getting Started" mode for overwhelmed users |

---

## Final Reminder

**Focus mode should reduce cognitive load and help ADHD-friendly users maintain momentum!**

- ✅ Always hide sidebars when in focus mode
- ✅ Show quick win badges prominently with toggle functionality
- ✅ Display estimated minutes for time management
- ✅ Provide simplified "Getting Started" view when needed
- ✅ Encourage keyboard shortcuts for efficient navigation
```

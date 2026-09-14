// =============================================================================
// Gadema.Core.Dtos.Tasks — Task Management DTOs
// 
// Conventions:
// - No nested CreateData/UpdateData (flat structure)
// - ProjectId always derived from route, NEVER in body
// - Partial update via nullable fields (null = unchanged)
// - Enums serialized as integers for API simplicity
// =============================================================================

using System;
using Gadema.Core.Dtos.Response;


namespace Gadema.Core.Dtos.Tasks;


// ========================================================================
// TASK — CREATE DTO (flat, no nesting)
// ========================================================================

/// <summary>Request body for creating a new project task.</summary>
public class ProjectTaskCreateDto : UpdateRequestDto
{
    /// <summary>Short title of the task. Required and non-empty.</summary>
    public string TaskTitle { get; set; } = "";

    /// <summary>Detailed description or context for the task.</summary>
    public string? Description { get; set; }

    /// <summary>Current status: 0=Backlog, 1=InProgress, 2=Review, 3=Done</summary>
    public int? Status { get; set; } = (int)TaskStatusEnum.Backlog;

    /// <summary>Priority level: 0=High, 1=Medium, 2=Low</summary>
    public int? Priority { get; set; } = (int)TaskPriorityEnum.Medium;

    /// <summary>Difficulty/Effort estimate: 0=Easy, 1=Medium, 2=Hard (ADHD-friendly)</summary>
    public int? Difficulty { get; set; } = (int)TaskDifficultyEnum.Medium;

    /// <summary>Estimated time in minutes.</summary>
    public decimal? EstimatedMinutes { get; set; }

    /// <summary>ID of the user assigned to this task. Null means unassigned.</summary>
    public Guid? AssignedToUserId { get; set; }

    /// <summary>Due date for completion (null = no deadline).</summary>
    public DateTime? DueDate { get; set; }

    /// <summary>ADHD-friendly quick win flag. If true, the task should be completed quickly.</summary>
    public bool IsQuickWin { get; set; } = false;

    // Inherited from UpdateRequestDto: Guid Id (for upsert patterns if needed)
}

// ========================================================================
// TASK — UPDATE DTO (partial update via nullable fields)
// ========================================================================

/// <summary>Request body for updating an existing project task. Only provided fields change.</summary>
public class ProjectTaskUpdateDto : UpdateRequestDto
{
    /// <summary>New title. If null, the current title is preserved.</summary>
    public string? TaskTitle { get; set; }

    /// <summary>New description. If null, the current description is preserved.</summary>
    public string? Description { get; set; }

    /// <summary>New status. If null, the current status is preserved.</summary>
    public int? Status { get; set; }

    /// <summary>New priority. If null, the current priority is preserved.</summary>
    public int? Priority { get; set; }

    /// <summary>New difficulty level. If null, the current difficulty is preserved.</summary>
    public int? Difficulty { get; set; }

    /// <summary>New estimated time in minutes. If null, the current estimate is preserved.</summary>
    public decimal? EstimatedMinutes { get; set; }

    /// <summary>New assigned user ID. Null means unassigned (remove assignment).</summary>
    public Guid? AssignedToUserId { get; set; }

    /// <summary>New due date. If null, the current due date is preserved.</summary>
    public DateTime? DueDate { get; set; }

    /// <summary>New quick win flag. If null, the current flag is preserved.</summary>
    public bool? IsQuickWin { get; set; }
}

// ========================================================================
// TASK — RESPONSE DTO (flat structure with denormalized fields)
// ========================================================================

/// <summary>Response DTO for a single task detail response.</summary>
public class TaskResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>Title of the task.</summary>
    public string TaskTitle { get; set; } = "";

    /// <summary>Detailed description or context.</summary>
    public string? Description { get; set; }

    /// <summary>Status: 0=Backlog, 1=InProgress, 2=Review, 3=Done</summary>
    public int Status { get; set; } = (int)TaskStatusEnum.Backlog;

    /// <summary>Priority level: 0=High, 1=Medium, 2=Low</summary>
    public int Priority { get; set; } = (int)TaskPriorityEnum.Medium;

    /// <summary>Difficulty/Effort: 0=Easy, 1=Medium, 2=Hard</summary>
    public int Difficulty { get; set; } = (int)TaskDifficultyEnum.Medium;

    /// <summary>Estimated time in minutes.</summary>
    public decimal? EstimatedMinutes { get; set; }

    /// <summary>ID of the assigned user. Null means unassigned.</summary>
    public Guid? AssignedToUserId { get; set; }

    /// <summary>Due date for completion (null = no deadline).</summary>
    public DateTime? DueDate { get; set; }

    /// <summary>ADHD-friendly quick win flag. If true, the task should be completed quickly.</summary>
    public bool IsQuickWin { get; set; } = false;

    // Inherited from MetaInfoResponseBaseDto:
    //   Guid Id, Guid ProjectId, string Title, ContentStatusEnum Status, bool IsPublic,
    //   DateTime CreatedAt, DateTime LastModifiedAt
}

// ========================================================================
// TASK LIST RESPONSE DTO — Paginated list of all project tasks
// ========================================================================

/// <summary>Response DTO for paginated task listing with optional filters.</summary>
public class TaskListResponseDto : ListResponseDto<TaskResponseDto>
{
    /// <summary>Filter: specific status to filter by (null = all statuses).</summary>
    public int? Status { get; set; }

    /// <summary>Filter: assigned user ID. If provided, only tasks for that user.</summary>
    public Guid? AssignedToUserId { get; set; }

    /// <summary>Filter: quick win flag (null = all values).</summary>
    public bool? IsQuickWin { get; set; }

    /// <summary>Search query to filter by task title or description.</summary>
    public string? SearchQuery { get; set; }

    /// <summary>Saved sort order: 0=DueDate ASC, 1=Priority DESC, 2=Status ASC, etc.</summary>
    public int SortOrder { get; set; } = 0; // Default: due date ascending
}

// ========================================================================
// TASK COMMENT — CREATE DTO (flat structure)
// ========================================================================

/// <summary>Request body for creating a new comment on a task.</summary>
public class ProjectTaskCommentCreateDto : UpdateRequestDto
{
    /// <summary>The comment text. Required and non-empty.</summary>
    public string Text { get; set; } = "";

    // Optional: support parent-child comments (threaded replies)
    public Guid? ParentId { get; set; }

    // Inherited from UpdateRequestDto: Guid Id (for upsert patterns if needed)
}

// ========================================================================
// TASK COMMENT — UPDATE DTO
// ========================================================================

/// <summary>Request body for updating an existing task comment.</summary>
public class ProjectTaskCommentUpdateDto : UpdateRequestDto
{
    /// <summary>New comment text. If null, the current text is preserved.</summary>
    public string? Text { get; set; }

    // Inherited from UpdateRequestDto: Guid Id (for upsert patterns if needed)
}

// ========================================================================
// TASK COMMENT — RESPONSE DTO
// ========================================================================

/// <summary>Response DTO for a single task comment detail.</summary>
public class TaskCommentResponseDto : MetaInfoResponseBaseDto
{
    /// <summary>The full text of the comment.</summary>
    public string Text { get; set; } = "";

    /// <summary>ID of the parent comment (null if this is a root-level comment).</summary>
    public Guid? ParentId { get; set; }

    // Inherited from MetaInfoResponseBaseDto:
    //   Guid Id, Guid ProjectId, string Title, ContentStatusEnum Status, bool IsPublic,
    //   DateTime CreatedAt, DateTime LastModifiedAt
}

// ========================================================================
// TASK COMMENT LIST RESPONSE DTO — Paginated comments for a specific task
// ========================================================================

/// <summary>Response DTO for paginated task comment listing.</summary>
public class TaskCommentListResponseDto : ListResponseDto<TaskCommentResponseDto>
{
    // Inherits: Items, TotalCount from base list response pattern
}

// ========================================================================
// USAGE EXAMPLES — How to use these DTOs in controllers/services
// ========================================================================

/*
 * ┌─────────────────────────────────────────────────────────────────┐
 * │  POST /api/v1/projects/{projectId:guid}/tasks                   │
 * └─────────────────────────────────────────────────────────────────┘
 * Request Body: ProjectTaskCreateDto
 * {
 *   "taskTitle": "Design login screen",
 *   "description": "Create a modern, responsive login UI component...",
 *   "status": 0,                  // Backlog (default)
 *   "priority": 1,                // Medium priority (default)
 *   "difficulty": 2,              // Hard effort estimate
 *   "estimatedMinutes": 180,      // ~3 hours
 *   "assignedToUserId": "abc-...",
 *   "dueDate": "2025-06-01T00:00:00Z",
 *   "isQuickWin": false
 * }
 * 
 * Response: ApiResponseDto<TaskResponseDto>
 * {
 *   "success": true,
 *   "data": {
 *     "id": "...",
 *     "projectId": "{projectId}",
 *     "title": "Design login screen",
 *     "status": 0,
 *     "priority": 1,
 *     "difficulty": 2,
 *     "estimatedMinutes": 180,
 *     "assignedToUserId": "abc-...",
 *     "dueDate": "2025-06-01T00:00:00Z",
 *     "isQuickWin": false,
 *     "createdAt": "...",
 *     "lastModifiedAt": "..."
 *   }
 * }
 * 
 * ┌─────────────────────────────────────────────────────────────────┐
 * │  GET /api/v1/projects/{projectId:guid}/tasks                    │
 * └─────────────────────────────────────────────────────────────────┘
 * Query Parameters (optional filters):
 *   ?status=2&assignedToUserId=xyz...&isQuickWin=true&page=1&pageSize=50
 * 
 * Response: ApiResponseDto<TaskListResponseDto>
 * {
 *   "success": true,
 *   "data": {
 *     "items": [ ... ],       // array of TaskResponseDto
 *     "totalCount": 47,
 *     "page": 1,
 *     "pageSize": 50,
 *     "totalPages": 1
 *   }
 * }
 * 
 * ┌─────────────────────────────────────────────────────────────────┐
 * │  PUT /api/v1/projects/{projectId:guid}/tasks/{taskId:guid}     │
 * └─────────────────────────────────────────────────────────────────┘
 * Request Body (partial update — only provided fields change):
 * {
 *   "taskTitle": "Design login screen v2",        // changed
 *   "status": 1,                                   // → InProgress
 *   "priority": 0,                                 // → High
 *   "isQuickWin": true                             // NEW flag
 * }
 * 
 * Response: ApiResponseDto<TaskResponseDto> (with updated fields)
 * 
 * ┌─────────────────────────────────────────────────────────────────┐
 * │  GET /api/v1/projects/{projectId:guid}/tasks/{taskId:guid}/comments
 * └─────────────────────────────────────────────────────────────────┘
 * Query Parameters:
 *   ?page=1&pageSize=20&parentId=null           // all comments or replies to one comment
 * 
 * Response: ApiResponseDto<TaskCommentListResponseDto>
 * {
 *   "success": true,
 *   "data": {
 *     "items": [ ... ],       // array of TaskCommentResponseDto
 *     "totalCount": 12,
 *     "page": 1,
 *     "pageSize": 20,
 *     "totalPages": 1
 *   }
 * }
 * 
 * ┌─────────────────────────────────────────────────────────────────┐
 * │  POST /api/v1/projects/{projectId:guid}/tasks/{taskId:guid}/comments
 * └─────────────────────────────────────────────────────────────────┘
 * Request Body: ProjectTaskCommentCreateDto
 * {
 *   "text": "This looks good but we need to add error handling for..."
 * }
 * 
 * Response: ApiResponseDto<TaskCommentResponseDto>
 */

// ========================================================================
// HELPER ENUM EXTENSIONS (for frontend type safety / validation)
// ========================================================================

public static class TaskStatusEnumExtensions
{
    public static string ToDescription(this TaskStatusEnum status) => status switch
    {
        TaskStatusEnum.Backlog => "Backlog",
        TaskStatusEnum.InProgress => "In Progress",
        TaskStatusEnum.Review => "Under Review",
        TaskStatusEnum.Done => "Done ✅",
        _ => $"Unknown {(int)status}"
    };

    public static string ToDisplayName(this TaskStatusEnum status) => status switch
    {
        TaskStatusEnum.Backlog => "Backlog (Waiting)",
        TaskStatusEnum.InProgress => "In Progress",
        TaskStatusEnum.Review => "Review Pending",
        TaskStatusEnum.Done => "Completed ✅",
        _ => $"Unknown {(int)status}"
    };
}

public static class PriorityEnumExtensions
{
    public static string ToDescription(this TaskPriorityEnum priority) => priority switch
    {
        TaskPriorityEnum.High => "High 🔥",
        TaskPriorityEnum.Medium => "Medium ⚡",
        TaskPriorityEnum.Low => "Low 🐢",
        _ => $"Unknown {(int)priority}"
    };

    public static int ToSortOrder(this TaskPriorityEnum priority) => (int)(3 - priority); // High=0 sorts first, Low=2 sorts last
}

public static class DifficultyEnumExtensions
{
    public static string ToDescription(this TaskDifficultyEnum difficulty) => difficulty switch
    {
        TaskDifficultyEnum.Easy => "Easy 🟢",
        TaskDifficultyEnum.Medium => "Medium 🟡",
        TaskDifficultyEnum.Hard => "Hard 🔴",
        _ => $"Unknown {(int)difficulty}"
    };

    public static decimal ToEstimateMultiplier(this TaskDifficultyEnum difficulty) => difficulty switch
    {
        TaskDifficultyEnum.Easy => 0.5m,   // Reduce estimate by half for easy tasks (ADHD-friendly optimism)
        TaskDifficultyEnum.Medium => 1.0m,  // No adjustment
        TaskDifficultyEnum.Hard => 1.5m,   // Increase estimate by 50% for hard tasks (buffer)
        _ => 1.0m
    };

    public static int ToSortOrder(this TaskDifficultyEnum difficulty) => (int)(2 - difficulty); // Easy=0 sorts first
}
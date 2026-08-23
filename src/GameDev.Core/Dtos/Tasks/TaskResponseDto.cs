// =============================================================================
// Response DTOs for API Services - Collection of response types referenced in GameDev.Api
// =============================================================================

using System.ComponentModel.DataAnnotations;

namespace GameDev.Core.Dtos.Tasks;

/// <summary>
/// Task list response.
/// </summary>
public class ProjectTaskListResponseDto
{
    public IEnumerable<ProjectTaskResponseDto> Items { get; set; } = Enumerable.Empty<ProjectTaskResponseDto>();
    
    public int TotalCount { get; set; }
    
    public int PageNumber { get; set; }
    
    public int PageSize { get; set; }
}

/// <summary>
/// Single task response.
/// </summary>
public class ProjectTaskResponseDto
{
    public Guid Id { get; set; }
    
    [Required, MaxLength(256)]
    public string TaskTitle { get; set; } = "";
    
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    public int Status { get; set; }
    
    public int Priority { get; set; }
    
    public int Difficulty { get; set; }
    
    public decimal? EstimatedMinutes { get; set; }
    
    public Guid? AssignedToUserId { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public bool IsQuickWin { get; set; }
    
    public DateTime CreatedAt { get; set; }

    // NEW: Add missing properties from ProjectTask model
    public Guid ProjectId { get; set; }
    [MaxLength(256)]
    public string? TaskDescription { get; set; }  // For ContentItemId compatibility
/// <summary>
    /// ID of the content item this task is associated with.
/// </summary>
    public Guid? ContentItemId { get; set; }
    
    public int? DifficultyValue { get; set; }  // For nullable difficulty
    [MaxLength(512)]
    public string? DifficultyName { get; set; }  // Enum name
}

/// <summary>
/// Create task DTO.
/// </summary>
public class ProjectTaskCreateDto
{
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }
    
    [MaxLength(256), Required]
    public string TaskTitle { get; set; } = "";
    
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    public int? Status { get; set; }  // Backlog, InProgress, Review, Done
    public int? Priority { get; set; }  // High, Medium, Low
    
    public int? Difficulty { get; set; }  // Easy, Medium, Hard
    
    [MaxLength(512)]
    public decimal? EstimatedMinutes { get; set; }
    
    public Guid? AssignedToUserId { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public bool? IsQuickWin { get; set; } = false;
}

/// <summary>
/// Update task DTO.
/// </summary>
public class ProjectTaskUpdateDto
{
    [MaxLength(256)]
    public string? TaskTitle { get; set; }
    
    [MaxLength(4096)]
    public string? Description { get; set; }
    
    public int? Status { get; set; }
    
    public int? Priority { get; set; }
    
    public int? Difficulty { get; set; }
    
    [MaxLength(512)]
    public decimal? EstimatedMinutes { get; set; }
    
    public Guid? AssignedToUserId { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public bool? IsQuickWin { get; set; } = false;
}

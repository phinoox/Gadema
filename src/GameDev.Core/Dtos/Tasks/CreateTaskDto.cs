// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace GameDev.Core.Dtos.Tasks;

/// <summary>
/// DTO for creating a new task.
/// </summary>
public class CreateTaskDto
{
    /// <summary>
    /// ID of the project this task belongs to (required).
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }
    
    /// <summary>
    /// Task title (required).
    /// </summary>
    [MaxLength(256), Required, Display(Name = "Task Title")]
    public string TaskTitle { get; set; } = "";
    
    /// <summary>
    /// Task description (optional).
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; } = null!;
    
    /// <summary>
    /// Status: 0=Backlog, 1=InProgress, 2=Review, 3=Done.
    /// </summary>
    [Range(0, 3)]
    public int Status { get; set; } = 0;
    
    /// <summary>
    /// Priority: 0=High, 1=Medium, 2=Low.
    /// </summary>
    [Range(0, 2)]
    public int Priority { get; set; } = 1;
    
    /// <summary>
    /// Difficulty: 0=Easy, 1=Medium, 2=Hard (ADHD-friendly).
    /// </summary>
    [Range(0, 2)]
    public int Difficulty { get; set; } = 0;
    
    /// <summary>
    /// Estimated time in minutes (optional).
    /// </summary>
    [MaxLength(512)]
    public decimal? EstimatedMinutes { get; set; } = null!;
    
    /// <summary>
    /// ADHD-friendly quick win flag.
    /// </summary>
    public bool IsQuickWin { get; set; } = false;
}
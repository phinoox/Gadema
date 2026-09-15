using System.ComponentModel.DataAnnotations;


namespace Gadema.Core.Dtos.Tasks;

/// <summary>
/// DTO for creating a project task.
/// </summary>
public class ProjectTaskCreateDto
{
    /// <summary>
    /// The nested identity payload (Title, Description, Status, Priority, etc.).
    /// </summary>
    [Required]
    public BaseMetaInfoCreateData MetaInfo { get; set; } = new();

    /// <summary>
    /// The project this task belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// User assigned to this task.
    /// </summary>
    public Guid? AssignedToUserId { get; set; }

    /// <summary>
    /// The deadline for the task.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// User who created this task.
    /// </summary>
    [Required]
    public Guid CreatedByUserId { get; set; }
}

/// <summary>
/// DTO for updating an existing project task.
/// </summary>
public class ProjectTaskUpdateDto
{
    /// <summary>
    /// The nested identity payload for the sync strategy.
    /// </summary>
    public BaseMetaInfoUpdateData? MetaInfo { get; set; }

    [MaxLength(128)] public string? AssignedToUserIdString { get; set; } // For easier mapping if needed
    public Guid? AssignedToUserId { get; set; }
    public DateTime? DueDate { get; set; }
}

/// <summary>
/// Response DTO for a project task.
/// Denormalizes properties from both the MetaInfo anchor and the Task component.
/// </summary>
public class ProjectTaskResponseDto
{
    public Guid Id { get; set; }
    public Guid MetaInfoId { get; set; }

    // Denormalized Identity Properties (from Anchor)
    public string Title { get; set; } = "";
    public string Slug { get; set; } = "";
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }

    // Domain Properties (from Component)
    public Guid ProjectId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid CreatedByUserId { get; set; }
}

/// <summary>
/// List response for project tasks.
/// </summary>
public class ProjectTaskListResponseDto
{
    public IEnumerable<ProjectTaskResponseDto> Items { get; set; } = Enumerable.Empty<ProjectTaskResponseDto>();
    public int TotalCount { get; set; }
}
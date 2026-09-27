using Gadema.Core.Dtos.Base.Infrastructure;

namespace Gadema.Core.Dtos.Tasks;

/// <summary>
/// Data transfer object for creating a new project task.
/// </summary>
public class ProjectTaskCreateDto
{
    /// <summary>
    /// The nested identity payload (e.g., Title, Slug, ShortDesc) used to anchor the task.
    /// </summary>
    [Required]
    public BaseMetaInfoCreateData MetaInfo { get; set; } = new();

    /// <summary>
    /// The unique identifier of the project this task belongs to.
    /// </summary>
    [Required]
    public Guid ProjectId { get; set; }

    /// <summary>
    /// The user assigned to work on this task.
    /// </summary>
    public Guid? AssignedToUserId { get; set; }

    /// <summary>
    /// The deadline by which the task should be completed.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// The unique identifier of the user who created this task.
    /// </summary>
    [Required]
    public Guid CreatedByUserId { get; set; }
}

/// <summary>
/// Data transfer object for updating an existing project task's properties and metadata.
/// </summary>
public class ProjectTaskUpdateDto
{
    /// <summary>
    /// The nested identity payload used by the sync strategy to update meta-information (e.g., Title, Slug).
    /// </summary>
    public BaseMetaInfoUpdateData? MetaInfo { get; set; }

    [MaxLength(128)] public string? AssignedToUserIdString { get; set; } // For easier mapping if needed
    
    /// <summary>
    /// The updated user assigned to this task.
    /// </summary>
    public Guid? AssignedToUserId { get; set; }

    /// <summary>
    /// The updated deadline for the task.
    /// </summary>
    public DateTime? DueDate { get; set; }
}

/// <summary>
/// Represents a project task, denormalizing properties from both its MetaInfo anchor and core domain component.
/// </summary>
public class ProjectTaskResponseDto
{
    /// <summary>
    /// The unique identifier of the task entity.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The unique identifier of the associated meta-information record.
    /// </summary>
    public Guid MetaInfoId { get; set; }

    // Denormalized Identity Properties (from Anchor)
    /// <summary>
    /// The title of the task.
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// The URL-friendly slug for the task.
    /// </summary>
    public string Slug { get; set; } = "";

    /// <summary>
    /// Indicates if this task is public within the project.
    /// </summary>
    public bool IsPublic { get; set; }

    /// <summary>
    /// The timestamp when the task was created in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    // Domain Properties (from Component)
    /// <summary>
    /// The unique identifier of the project this task belongs to.
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// The user currently assigned to this task.
    /// </summary>
    public Guid? AssignedToUserId { get; set; }

    /// <summary>
    /// The deadline for completing the task.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// The user who created this task.
    /// </summary>
    public Guid CreatedByUserId { get; set; }
}

/// <summary>
/// A collection of project tasks, typically used for paginated lists.
/// </summary>
public class ProjectTaskListResponseDto
{
    /// <summary>
    /// The list of retrieved project tasks.
    /// </summary>
    public IEnumerable<ProjectTaskResponseDto> Items { get; set; } = Enumerable.Empty<ProjectTaskResponseDto>();

    /// <summary>
    /// Total number of tasks matching the query/filters across all pages.
    /// </summary>
    public int TotalCount { get; set; }
}
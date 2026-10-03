
using Gadema.Core.Dtos.Base.Infrastructure;

// 1. Identity Data for creation
using Gadema.Core.Models.Base.Projects.Enums;

namespace Gadema.Core.Dtos.Base.Projects;
/// <summary>
/// Metadata required to initialize a new project.
/// </summary>
public class ProjectMetaInfoCreateData : BaseMetaInfoCreateData
{
    /// <summary>
    /// The initial status of the project.
    /// </summary>
    public ProjectStatusEnum Status { get; set; }
    
    /// <summary>
    /// The default view mode for this project.
    /// </summary>
    public ViewModeEnum ViewMode { get; set; }
    
}

// 2. Create DTO (The entry point)
/// <summary>
/// Data transfer object used to create a new project.
/// </summary>
public class ProjectCreateDto
{
    /// <summary>
    /// The identity and metadata for the new project.
    /// </summary>
    public ProjectMetaInfoCreateData MetaInfo { get; set; } = null!;

    /// <summary>
    /// A high-level description of the project.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether users can register themselves for this project.
    /// </summary>
    public bool EnableUserRegistration { get; set; }

    /// <summary>
    /// Whether administrators can invite users manually.
    /// </summary>
    public bool AllowManualInvites { get; set; }

    /// <summary>
    /// The primary format of the project (e.g., Novel, Manga).
    /// </summary>
    public PrimaryFormatEnum PrimaryFormat { get; set; }

    /// <summary>
    /// The genre of the project content.
    /// </summary>
    public string? Genre { get; set; }

    /// <summary>
    /// The central theme or setting of the project.
    /// </summary>
    public string? Theme { get; set; }

    /// <summary>
    /// The narrative tone of the project.
    /// </summary>
    public ToneEnum Tone { get; set; }

    /// <summary>
    /// The intended target audience for the project.
    /// </summary>
    public AudienceEnum Audience { get; set; }

    /// <summary>
    /// An optional initial status override.
    /// </summary>
    public ContentStatusEnum? ProjectStatus { get; set; }
}

// 3. Update DTO (The sync payload)
/// <summary>
/// Data transfer object for updating an existing project's settings and metadata.
/// </summary>
public class ProjectUpdateDto
{
    /// <summary>
    /// The updated description of the project.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Whether to enable or disable user registration.
    /// </summary>
    public bool? EnableUserRegistration { get; set; }

    /// <summary>
    /// Whether to enable or disable manual invites.
    /// </summary>
    public bool? AllowManualInvites { get; set; }

    /// <summary>
    /// The updated primary format.
    /// </summary>
    public PrimaryFormatEnum? PrimaryFormat { get; set; }

    /// <summary>
    /// The updated genre.
    /// </summary>
    public string? Genre { get; set; }

    /// <summary>
    /// The updated theme.
    /// </summary>
    public string? Theme { get; set; }

    /// <summary>
    /// The updated narrative tone.
    /// </summary>
    public ToneEnum? Tone { get; set; }

    /// <summary>
    /// The updated target audience.
    /// </summary>
    public AudienceEnum? Audience { get; set; }
    
    /// <summary>
    /// The identity payload that includes the tags to sync.
    /// </summary>
    public ProjectMetaInfoUpdateData? ContentMetaInfo { get; set; }
}



// 5. Response DTO (The flattened "Contract")
/// <summary>
/// Represents a project's core details for client consumption.
/// </summary>
public class ProjectResponseDto 
{
    /// <summary>
    /// The unique identifier of the project.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The title of the project.
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// The URL-friendly slug of the project.
    /// </summary>
    public string Slug { get; set; } = "";

    /// <summary>
    /// The current status of the project lifecycle.
    /// </summary>
    public ProjectStatusEnum Status { get; set; }

    /// <summary>
    /// The visibility level of the project.
    /// </summary>
    public ProjectVisibilityEnum Visibility { get; set; }

    /// <summary>
    /// The default view mode for this project.
    /// </summary>
    public ViewModeEnum ViewMode { get; set; }

    /// <summary>
    /// The timestamp when the project was created in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// A high-level description of the project.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indicates if the project is currently active and accessible.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// The primary format of the project content.
    /// </summary>
    public PrimaryFormatEnum PrimaryFormat { get; set; }

    /// <summary>
    /// The genre associated with this project.
    /// </summary>
    public string? Genre { get; set; }

    /// <summary>
    /// The central theme of the project.
    /// </summary>
    public string? Theme { get; set; }

    /// <summary>
    /// The narrative tone of the project.
    /// </summary>
    public ToneEnum Tone { get; set; }

    /// <summary>
    /// The target audience for this project.
    /// </summary>
    public AudienceEnum Audience { get; set; }
}
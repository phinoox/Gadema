namespace Gadema.Core.Dtos.Base.Infrastructure;

using Gadema.Core.Models.Base.Projects.Enums;


/// <summary>
/// Shared content metadata required for creating any content entity.
/// </summary>
public class BaseMetaInfoCreateData
{
    /// <summary>
    /// The title of the content item.
    /// </summary>
    [Required]
    public string Title { get; set; } = "";

    /// <summary>
    /// A URL-friendly unique identifier (slug).
    /// </summary>
    [MaxLength(128)]
    public string? Slug { get; set; }

    /// <summary>
    /// A short description of the content item.
    /// </summary>
    [MaxLength(4096)]
    public string? ShortDesc { get; set; }

    /// <summary>
    /// Indicates if this content is visible to users who are not project members.
    /// </summary>
    public bool IsPublic { get; set; } = false;

    /// <summary>
    /// The list of tag identifiers to associate with this entity.
    /// </summary>
    public List<Guid>? TagIds { get; set; } 
}

/// <summary>
/// Data required for creating content meta-information, including workflow status.
/// </summary>
public class ContentMetaInfoCreateData : BaseMetaInfoCreateData
{
    /// <summary>
    /// The initial workflow status of the content (e.g., Draft).
    /// </summary>
    public ContentStatusEnum Status { get; set; } = ContentStatusEnum.Draft;

}

/// <summary>
/// Unified response for successful creation operations, containing navigation identifiers.
/// </summary>
public class CreateResponseDto
{
    /// <summary>
    /// The unique identifier of the newly created entity.
    /// </summary>
    [Required, Display(Name = "Entity ID")]
    public Guid EntityId { get; set; }
    
    /// <summary>
    /// The unique identifier of the associated meta-information record.
    /// </summary>
    [Required, Display(Name = "ContentMetaInfo ID")]
    public Guid MetaInfoId { get; set; }
    
    /// <summary>
    /// The unique identifier of the project this entity belongs to.
    /// </summary>
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }
}

/// <summary>
/// Data required for updating meta-information, allowing partial updates via the sync pattern.
/// </summary>
public class BaseMetaInfoUpdateData
{
    /// <summary>
    /// The updated title of the content item.
    /// </summary>
    [MaxLength(128)] public string? Title { get; set; }

    /// <summary>
    /// The updated slug for the content item.
    /// </summary>
    [MaxLength(128)] public string? Slug { get; set; }

    /// <summary>
    /// The updated short description of the content item.
    /// </summary>
    [MaxLength(4096)] public string? ShortDesc { get; set; }

    /// <summary>
    /// The updated visibility status of the content.
    /// </summary>
    public bool? IsPublic { get; set; } = false;

    /// <summary>
    /// The full list of tags that should exist on this entity after the update.
    /// Used for the "Sync Pattern".
    /// </summary>
    public List<Guid>? TagIds { get; set; } 
}

/// <summary>
/// Data required for updating content meta-information, including workflow status.
/// </summary>
public class ContentMetaInfoUpdateData : BaseMetaInfoUpdateData
{
    /// <summary>
    /// The updated workflow status of the content.
    /// </summary>
    public ContentStatusEnum? Status { get; set; } = ContentStatusEnum.Draft;

}

/// <summary>
/// Data required for updating project-specific meta-information.
/// </summary>
public class ProjectMetaInfoUpdateData : BaseMetaInfoUpdateData
{
    /// <summary>
    /// The updated status of the project.
    /// </summary>
    public ProjectStatusEnum? ProjectStatus { get; set; }

    /// <summary>
    /// The updated visibility level for the project.
    /// </summary>
    public ProjectVisibilityEnum? Visibility { get; set; }

    /// <summary>
    /// The updated view mode setting for the project.
    /// </summary>
    public ViewModeEnum? ViewMode { get; set; }
}

/// <summary>
/// Unified response for successful delete operations.
/// </summary>
public class DeleteResponseDto
{
    /// <summary>
    /// The unique identifier of the deleted entity.
    /// </summary>
    [Required, Display(Name = "Entity ID")]
    public Guid EntityId { get; set; }

    /// <summary>
    /// The unique identifier of the project the entity belonged to.
    /// </summary>
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }
}

/// <summary>
/// Base class for all update requests, providing the EntityId property.
/// </summary>
public abstract class UpdateRequestDto
{
    /// <summary>
    /// The unique identifier of the entity being updated.
    /// </summary>
    [Required, Display(Name = "Entity ID")]
    public Guid EntityId { get; set; }
}

/// <summary>
/// Base response DTO for meta-information queries.
/// </summary>
public abstract class MetaInfoResponseBaseDto
{
    /// <summary>
    /// The unique identifier of the meta-info record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The ID of the associated content entity.
    /// </summary>
    public Guid MetaInfoId { get; set; }

    /// <summary>
    /// The title of the associated content item.
    /// </summary>
    public string? MetaInfoTitle { get; set; }

    /// <summary>
    /// Indicates if this content is public.
    /// </summary>
    public bool IsPublic { get; set; }  // ✅ Visibility of ContentMetaInfo wrapper

    /// <summary>
    /// The timestamp when the record was created in UTC.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The timestamp of the last modification in UTC.
    /// </summary>
    public DateTime LastModifiedAt { get; set; }

    /// <summary>
    /// The current workflow status of the content.
    /// </summary>
    public ContentStatusEnum Status { get; set; } = ContentStatusEnum.Draft;

}

/// <summary>
/// Response data for an automatic save operation.
/// </summary>
public class AutosaveResponseDto
{
    /// <summary>
    /// The unique identifier of the created version log entry.
    /// </summary>
    public Guid VersionLogId { get; set; }

    /// <summary>
    /// The resulting version number after the save operation.
    /// </summary>
    public int VersionNumber { get; set; }
}
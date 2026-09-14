// =============================================================================
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Enums;

namespace Gadema.Core.Dtos;

/// <summary>
/// Shared MetaInfo data for creation of any content entity.
/// ContentType is resolved from the endpoint, not the body.
/// </summary>
public class MetaInfoCreateData
{
    [Required]
    public string Title { get; set; } = "";

    [MaxLength(128)]
    public string? Slug { get; set; }

    [MaxLength(4096)]
    public string? ShortDesc { get; set; }

     /// <summary>
    /// Workflow status of the content (Draft, InProgress, Published, Archived).
    /// </summary>
    public ContentStatusEnum Status { get; set; } = ContentStatusEnum.Draft;

    /// <summary>
    /// Indicates if this content is visible to users who are not project members.
    /// </summary>
    public bool IsPublic { get; set; } = false;
}

/// <summary>
/// Unified response for create operations.
/// Contains only the identifiers needed to navigate to the entity's edit UI.
/// Client already knows Status + IsPublic from the request body.
/// </summary>
public class CreateResponseDto
{
    [Required, Display(Name = "Entity ID")]
    public Guid EntityId { get; set; }
    
    [Required, Display(Name = "MetaInfo ID")]
    public Guid MetaInfoId { get; set; }
    
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }
}

public class MetaInfoUpdateData
{
    [MaxLength(128)] public string? Title { get; set; }
    [MaxLength(128)] public string? Slug { get; set; }
    [MaxLength(4096)] public string? ShortDesc { get; set; }

     /// <summary>
    /// Workflow status of the content (Draft, InProgress, Published, Archived).
    /// </summary>
    public ContentStatusEnum? Status { get; set; } = ContentStatusEnum.Draft;

    public bool? IsPublic { get; set; } = false;

    /// <summary>
    /// The full list of tags that should exist on this entity after the update.
    /// Used for the "Sync Pattern".
    /// </summary>
    public List<Guid>? TagIds { get; set; } 
}

public class ProjectMetaInfoUpdateData : MetaInfoUpdateData
{
    // Project-specific identity fields
    public ProjectStatusEnum? ProjectStatus { get; set; }
    public ProjectVisibilityEnum? Visibility { get; set; }
    public ViewModeEnum? ViewMode { get; set; }
}

/// <summary>
/// Unified response for delete operations.
/// </summary>
public class DeleteResponseDto
{
    [Required, Display(Name = "Entity ID")]
    public Guid EntityId { get; set; }

    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }
}


/// <summary>
/// Base class for update DTOs. Provides the EntityId property shared by all updates.
/// Each entity-specific DTO inherits this and adds its own optional fields.
/// </summary>
public abstract class UpdateRequestDto
{
    /// <summary>
    /// ID of the entity being updated (required).
    /// </summary>
    [Required, Display(Name = "Entity ID")]
    public Guid EntityId { get; set; }
}

public abstract class MetaInfoResponseBaseDto
{
    public Guid Id { get; set; }
    public Guid MetaInfoId { get; set; }
    public string? MetaInfoTitle { get; set; }
    public bool IsPublic { get; set; }  // ✅ Visibility of MetaInfo wrapper
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }

     /// <summary>
    /// Workflow status of the content (Draft, InProgress, Published, Archived).
    /// </summary>
    public ContentStatusEnum Status { get; set; } = ContentStatusEnum.Draft;

}

public class AutosaveResponseDto
{
    public Guid VersionLogId { get; set; }
    public int VersionNumber { get; set; }
}
// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos;

/// <summary>
/// Shared MetaInfo data for creation of any content entity.
/// ContentType is resolved from the endpoint, not the body.
/// </summary>
public class MetaInfoCreateData
{
   // [Required]
 //   public Guid ProjectId { get; set; }

    [Required]
    public string Title { get; set; } = "";

    [MaxLength(128)]
    public string? Slug { get; set; }

    [MaxLength(4096)]
    public string? ShortDesc { get; set; }
}

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
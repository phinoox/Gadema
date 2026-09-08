// =============================================================================
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Enums;

namespace Gadema.Core.Dtos.Narrative;


public class SceneCreateDto
{
    [Required] public MetaInfoCreateData CreateData { get; set; } = new();
    
    /// <summary>
    /// ID of the StoryOutline this scene belongs to.
    /// </summary>
    [Required] public Guid StoryOutlineId { get; set; }

    /// <summary>
    /// Order index for sorting within the outline. Defaults to last if not provided.
    /// </summary>
    public int? OrderIndex { get; set; }
}

public class SceneCreateResponseDto
{
    public CreateResponseDto Data { get; set; } = new();
}

/// <summary>
/// DTO for creating a scene.
/// </summary>
public class SceneUpdateDto : UpdateRequestDto
{
    [MaxLength(4096)] public string? RawText { get; set; }
    public Guid? StoryOutlineId { get; set; }
    public int? OrderIndex { get; set; }
    public bool? HasGameLogic { get; set; }
    public ContentStatusEnum? Status { get; set; }
    
    public MetaInfoUpdateData? MetaInfo {get;set;}
}

/// <summary>
/// Response DTO for a scene.
/// </summary>
public class SceneResponseDto
{
    public Guid Id { get; set; }
    public Guid MetaInfoId { get; set; }
    public string? MetaInfoTitle { get; set; }
    public string RawText { get; set; } = "";
    public Guid StoryOutlineId { get; set; }
    public int OrderIndex { get; set; }
    public bool HasGameLogic { get; set; }
    public ContentStatusEnum Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastModifiedAt { get; set; }
}

/// <summary>
/// List response for scenes.
/// </summary>
public class SceneListResponseDto
{
    public IEnumerable<SceneResponseDto> Items { get; set; } = Enumerable.Empty<SceneResponseDto>();
    public int TotalCount { get; set; }
}

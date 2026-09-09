// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Narrative;

/// <summary>
/// DTO for creating a story beat.
/// </summary>
public class StoryBeatCreateDto
{
    [Required] public MetaInfoCreateData CreateData { get; set; } = new();
    
    /// <summary>
    /// ID of the StoryOutline this beat belongs to.
    /// </summary>
    [Required] public Guid StoryOutlineId { get; set; }

    /// <summary>
    /// Description of the beat's narrative intent.
    /// </summary>
    [MaxLength(4096)] public string? Description { get; set; }

    /// <summary>
    /// Order index for sorting beats in the Timeline/Map views.
    /// Defaults to last if not provided.
    /// </summary>
    public int? OrderIndex { get; set; }
}

/// <summary>
/// DTO for updating a story beat.
/// </summary>
public class StoryBeatUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// MetaInfo fields (nullable — omit to keep current).
    /// </summary>
    public MetaInfoUpdateData? MetaInfo { get; set; }

    /// <summary>
    /// Description of the beat's narrative intent.
    /// </summary>
    [MaxLength(4096)] public string? Description { get; set; }

    /// <summary>
    /// Order index for sorting beats in the Timeline/Map views.
    /// </summary>
    public int? OrderIndex { get; set; }
}

/// <summary>
/// Response DTO for a story beat. Inherits MetaInfo state (Id, MetaInfoId, MetaInfoTitle, Status, IsPublic, CreatedAt, LastModifiedAt).
/// </summary>
public class StoryBeatResponseDto : MetaInfoResponseBaseDto
{
    public Guid StoryOutlineId { get; set; }
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
}

/// <summary>
/// List response for story beats.
/// </summary>
public class StoryBeatListResponseDto
{
    public IEnumerable<StoryBeatResponseDto> Items { get; set; } = Enumerable.Empty<StoryBeatResponseDto>();
    public int TotalCount { get; set; }
}
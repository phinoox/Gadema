// =============================================================================
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Narrative;

/// <summary>
/// DTO for creating a story beat.
/// </summary>
// StoryBeatCreateDto — change StoryOutlineId to StoryId
public class StoryBeatCreateDto
{
    [Required] public MetaInfoCreateData CreateData { get; set; } = new();

    [Required] public Guid StoryId { get; set; }   // ← CHANGED from StoryOutlineId

    [MaxLength(4096)] public string? Description { get; set; }
    public int? OrderIndex { get; set; }
}

// ... StoryBeatUpdateDto stays the same ...

// StoryBeatResponseDto — change StoryOutlineId to StoryId
public class StoryBeatResponseDto : MetaInfoResponseBaseDto
{
    public Guid StoryId { get; set; }   // ← CHANGED from StoryOutlineId

    public string? Description { get; set; }
    public int OrderIndex { get; set; }
}

/// <summary>
/// DTO for updating a story beat.
/// </summary>
public class StoryBeatUpdateDto : UpdateRequestDto
{
    /// <summary>
    /// ContentMetaInfo fields (nullable — omit to keep current).
    /// </summary>
    public MetaInfoUpdateData? ContentMetaInfo { get; set; }

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
/// List response for story beats.
/// </summary>
public class StoryBeatListResponseDto
{
    public IEnumerable<StoryBeatResponseDto> Items { get; set; } = Enumerable.Empty<StoryBeatResponseDto>();
    public int TotalCount { get; set; }
}
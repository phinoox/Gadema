// src/Gadema.Core/Dtos/Characters/StoryEventDtos.cs
using Gadema.Core.Models.Characters;

namespace Gadema.Core.Dtos.Characters;

/// <summary>
/// DTO for creating a story event.
/// </summary>
public class StoryEventCreateDto
{
    /// <summary>
    /// ID of the scene where this event occurred.
    /// </summary>
    [Required] public Guid SceneId { get; set; }
    
    /// <summary>
    /// ID of the MetaInfo representing the actor/entity that was affected.
    /// </summary>
    [Required] public Guid ActorMetaInfoId { get; set; }
    
    /// <summary>
    /// Type of event (CharacterStateChange, CharacterRelationChange, FactionShift, etc.).
    /// </summary>
    public StoryEventType EventType { get; set; } = StoryEventType.Custom;
    
    /// <summary>
    /// The entity type that changed (e.g., "CharacterState", "CharacterRelation").
    /// </summary>
    [Required] public string TargetEntityTypeId { get; set; } = "";
    
    /// <summary>
    /// The ID of the specific entity that changed.
    /// </summary>
    [Required] public Guid TargetEntityId { get; set; }
    
    /// <summary>
    /// Optional description of what happened.
    /// </summary>
    [MaxLength(2048)] public string? Description { get; set; }
}

/// <summary>
/// DTO for updating a story event.
/// </summary>
public class StoryEventUpdateDto : UpdateRequestDto
{
    public Guid? SceneId { get; set; }
    public Guid? ActorMetaInfoId { get; set; }
    public StoryEventType? EventType { get; set; }
    [MaxLength(2048)] public string? Description { get; set; }
}

/// <summary>
/// Response DTO for a story event.
/// </summary>
public class StoryEventResponseDto
{
    public Guid Id { get; set; }
    public Guid SceneId { get; set; }
    public string? SceneTitle { get; set; }
    public Guid ActorMetaInfoId { get; set; }
    public string? ActorName { get; set; }  // Denormalized from MetaInfo.Title
    public StoryEventType EventType { get; set; }
    public string TargetEntityTypeId { get; set; } = "";
    public Guid TargetEntityId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// List response for story events.
/// </summary>
public class StoryEventListResponseDto
{
    public IEnumerable<StoryEventResponseDto> Items { get; set; } = Enumerable.Empty<StoryEventResponseDto>();
    public int TotalCount { get; set; }
}

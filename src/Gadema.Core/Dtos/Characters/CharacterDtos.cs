// src/Gadema.Core/Dtos/Characters/CharacterDtos.cs
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Models.Characters;

namespace Gadema.Core.Dtos.Characters;

/// <summary>
/// DTO for creating a character (glue entity).
/// </summary>
public class CharacterCreateDto
{
    [Required] public ContentMetaInfoCreateData CreateData { get; set; } = new();

    [Required, MaxLength(128)] public string Name { get; set; } = "";

    [MaxLength(256)] public string? NickName { get; set; }

    public Guid? CurrentStateId { get; set; }
    public Guid? StoryProfileId { get; set; }
}

/// <summary>
/// DTO for updating a character's linkage (not ContentMetaInfo or details themselves).
/// </summary>
public class CharacterUpdateDto : UpdateRequestDto
{
    [MaxLength(128)] public string? Name { get; set; }

    [MaxLength(256)] public string? NickName { get; set; }

    public Guid? CurrentStateId { get; set; }
    public Guid? StoryProfileId { get; set; }
    public MetaInfoUpdateData? ContentMetaInfo { get; set; }
}

/// <summary>
/// Response DTO for a character, including linked story profile and current state.
/// </summary>
public class CharacterResponseDto : MetaInfoResponseBaseDto
{
    [Required, MaxLength(128)] public string Name { get; set; } = "";

    [MaxLength(256)] public string? NickName { get; set; }

    public Guid? CurrentStateId { get; set; }
    public Guid? StoryProfileId { get; set; }
}

/// <summary>
/// List response for characters.
/// </summary>
public class CharacterListResponseDto
{
    public IEnumerable<CharacterResponseDto> Items { get; set; } = Enumerable.Empty<CharacterResponseDto>();
    public int TotalCount { get; set; }
}

using Gadema.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Base.Projects;

/// <summary>
/// DTO for creating a new project.
/// </summary>
public class ProjectCreateDto
{
    [Required, MaxLength(256)]
    public string Title { get; set; } = "";

    [MaxLength(128)]
    public string? Slug { get; set; }

    [MaxLength(4096)]
    public string? Description { get; set; }

    [Required]
    public ProjectStatusEnum Status { get; set; } = ProjectStatusEnum.Draft;

    [Required]
    public ProjectVisibilityEnum Visibility { get; set; } = ProjectVisibilityEnum.Private;

    [Required]
    public PrimaryFormatEnum PrimaryFormat { get; set; } = PrimaryFormatEnum.Book;

    [MaxLength(128)]
    public string? Genre { get; set; }

    [MaxLength(128)]
    public string? Theme { get; set; }

    public ToneEnum Tone { get; set; } = ToneEnum.Neutral;

    public AudienceEnum Audience { get; set; } = AudienceEnum.AllAges;
}

/// <summary>
/// DTO for updating an existing project.
/// All properties are nullable to support partial updates.
/// </summary>
public class ProjectUpdateDto
{
    [MaxLength(256)]
    public string? Title { get; set; }

    [MaxLength(128)]
    public string? Slug { get; set; }

    [MaxLength(4096)]
    public string? Description { get; set; }

    public ProjectStatusEnum? Status { get; set; }

    public ProjectVisibilityEnum? Visibility { get; set; }

    public PrimaryFormatEnum? PrimaryFormat { get; set; }

    [MaxLength(128)]
    public string? Genre { get; set; }

    [MaxLength(128)]
    public string? Theme { get; set; }

    public ToneEnum? Tone { get; set; }

    public AudienceEnum? Audience { get; set; }
}
// =============================================================================
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using Gadema.Core.Enums;
// =============================================================================

namespace Gadema.Core.Dtos.Projects;

/// <summary>
/// DTO for creating a new project.
/// </summary>
public class CreateProjectDto
{
    /// <summary>
    /// Project title (required).
    /// </summary>
    [Required, MaxLength(256), Display(Name = "Project Title")]
    public string Title { get; set; } = "";
    
    /// <summary>
    /// URL-friendly slug for the project (optional, auto-generated if not provided).
    /// </summary>
    [MaxLength(128)]
    public string? Slug { get; set; } = null!;
    
    /// <summary>
    /// ID of the project template to use (optional).
    /// </summary>
    public Guid? TemplateId { get; set; } = null!;
    
    public string? Description {get;set;}

    public ProjectVisibilityEnum Visibility {get;set;}
}
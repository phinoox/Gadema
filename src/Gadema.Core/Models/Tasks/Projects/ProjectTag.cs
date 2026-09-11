using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Gadema.Core.Models.Projects;

/// <summary>
/// Represents a tag specifically for Projects.
/// </summary>
[ModelDependency(typeof(RootMarker))]
public class ProjectTag
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(128)]
    public string Name { get; set; } = "";

    [MaxLength(128)]
    public string Slug { get; set; } = "";

    [MaxLength(36)]
    public string? ColorHex { get; set; }

    // Navigation
    public virtual ICollection<ProjectTagRelation> ProjectTags { get; set; } = new List<ProjectTagRelation>();
}
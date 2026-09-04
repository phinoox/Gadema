using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Dtos.Projects;

/// <summary>
/// Data Transfer Object for Project Tags.
/// Represents a tag definition used across multiple projects.
/// </summary>
public class ProjectTagDto
{
    /// <summary>
    /// Unique identifier for the tag.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Display name of the tag.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// URL-friendly version of the name.
    /// </summary>
    public string Slug { get; set; } = "";

    /// <summary>
    /// Hex color code for UI visualization.
    /// </summary>
    public string? ColorHex { get; set; }

    /// <summary>
    /// Number of projects currently using this tag.
    /// Useful for determining if a tag can be safely deleted.
    /// </summary>
    public int UsageCount { get; set; }
}

public class CreateProjectTagDto
{
    [Required]
    public string Name { get; set; } = "";

    public string? Slug { get; set; }

    public string? ColorHex { get; set; }
}


public class AddProjectTagsDto
{
    [Required]
    public List<Guid> TagIds { get; set; } = new();
}


public class RemoveProjectTagsDto
{
    [Required]
    public Guid TagId { get; set; } = new();
}
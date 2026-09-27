namespace Gadema.Core.Dtos.Base.Projects;

/// <summary>
/// Data transfer object for creating a new tag within a project context.
/// </summary>
public class ProjectTagCreateDto
{
    /// <summary>
    /// The display name of the tag.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// A URL-friendly unique identifier (slug) for the tag.
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// An optional hexadecimal color code (e.g., "#FF5733") for visual representation.
    /// </summary>
    public string? ColorHex { get; set; }
}

/// <summary>
/// Represents a project-specific tag for client consumption.
/// </summary>
public class ProjectTagResponseDto
{
    /// <summary>
    /// The unique identifier of the tag.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The display name of the tag.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// The URL-friendly slug of the tag.
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// The hexadecimal color code for the tag.
    /// </summary>
    public string? ColorHex { get; set; }
}

/// <summary>
/// Data transfer object used to associate multiple existing tags with a project.
/// </summary>
public class AddTagsToProjectDto
{
    /// <summary>
    /// The list of tag identifiers to be linked to the project.
    /// </summary>
    public List<Guid> TagIds { get; set; } = new();
}
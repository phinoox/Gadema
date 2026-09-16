namespace Gadema.Core.Dtos.Base.Projects;

public class ProjectTagCreateDto
{
    public string Name { get; set; } = "";
    public string? Slug { get; set; }
    public string? ColorHex { get; set; }
}

public class ProjectTagResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string? Slug { get; set; }
    public string? ColorHex { get; set; }
}

public class AddTagsToProjectDto
{
    public List<Guid> TagIds { get; set; } = new();
}
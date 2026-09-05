using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Gadema.Core.DependencyResolver;

namespace Gadema.Core.Models.Content;

/// <summary>
/// Represents a tag specifically for ContentItems (Characters, World, Mechanics, etc.).
/// </summary>
[DependencyResolver.ModelDependency(typeof(RootMarker))]
public class ContentTag
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(128)]
    public string Name { get; set; } = "";

    [MaxLength(128)]
    public string Slug { get; set; } = "";

    [MaxLength(36)]
    public string? ColorHex { get; set; }

    // Navigation
    public virtual ICollection<ContentItemTag> ContentItemTags { get; set; } = new List<ContentItemTag>();
}
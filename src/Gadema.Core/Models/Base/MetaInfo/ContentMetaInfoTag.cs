using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


namespace Gadema.Core.Models.Base.MetaInfo;

/// <summary>
/// Represents a tag specifically for MetaInfos (Characters, World, Mechanics, etc.).
/// </summary>
[ModelDependency(typeof(RootMarker))]
public class ContentMetaInfoTag
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(128)]
    public string Name { get; set; } = "";

    [MaxLength(128)]
    public string Slug { get; set; } = "";

    [MaxLength(36)]
    public string? ColorHex { get; set; }

    // Navigation
    public virtual ICollection<ContentMetaInfoTagRelation> MetaInfoTagRelations { get; set; } = new List<ContentMetaInfoTagRelation>();
}
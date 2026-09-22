namespace Gadema.Core.Models.Base.MetaInfo;

/// <summary>
/// A universal tag used for categorization and discovery across the entire system.
/// </summary>
[ModelDependency(typeof(RootMarker))]
public class MetaTag
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(128)]
    public string Name { get; set; } = "";

    [Required, MaxLength(128), Column("slug")]
    public string Slug { get; set; } = "";
}
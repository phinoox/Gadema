using Gadema.Core.Models.Base.Projects;

namespace Gadema.Core.Models.Base.MetaInfo;

/// <summary>
/// Holds the identity and discovery metadata for a Project Series.
/// Acts as the "Identity Card" for the top-level universe anchor.
/// </summary>
[ModelDependency(typeof(RootMarker))]
public class ProjectSeriesMetaInfo : BaseMetaInfo
{
    // Note: Title, Slug, IsPublic, CreatedAt, LastModifiedAt are inherited from BaseMetaInfo

    /// <summary>
    /// Series-wide description.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }

    public virtual ProjectSeries ProjectSeries { get; set; } = null!;
}

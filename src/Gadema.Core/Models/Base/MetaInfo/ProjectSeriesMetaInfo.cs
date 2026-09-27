using Gadema.Core.Models.Base.Projects;

namespace Gadema.Core.Models.Base.MetaInfo;

/// <summary>
/// Serves as the identity anchor for a Project Series, holding its metadata and high-level description.
/// This entity acts as the root metadata provider for all projects within this series.
/// </summary>
[ModelDependency(typeof(RootMarker))]
public class ProjectSeriesMetaInfo : BaseMetaInfo
{
    // Note: Title, Slug, IsPublic, CreatedAt, LastModifiedAt are inherited from BaseMetaInfo

    /// <summary>
    /// A detailed description of the series and its overarching themes/universe.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; }

    /// <summary>
    /// Navigation property for the parent project series.
    /// </summary>
    public virtual ProjectSeries ProjectSeries { get; set; } = null!;
}


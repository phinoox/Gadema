// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Base.Projects;

/// <summary>
/// Represents a collection of related projects (e.g., a book series or game franchise).
/// A series acts as the highest-level container in the hierarchy, anchoring multiple projects.
/// </summary>
[ModelDependency(typeof(ProjectSeriesMetaInfo))]
public class ProjectSeries
{
    /// <summary>
    /// Unique identifier for the series.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The ID of the associated identity anchor for the series.
    /// </summary>
    public Guid ProjectSeriesMetaInfoId { get; set; }
    
    /// <summary>
    /// Navigation property for the series' identity anchor.
    /// </summary>
    [ForeignKey("ProjectSeriesMetaInfoId")]
    public virtual ProjectSeriesMetaInfo ProjectSeriesMetaInfo { get; set; } = null!;

    /// <summary>
    /// Collection of projects that belong to this series.
    /// </summary>
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
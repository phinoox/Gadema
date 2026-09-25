// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Base.Projects;

/// <summary>
/// Represents a collection of related projects (e.g., a book series or game franchise).
/// </summary>
[ModelDependency(typeof(ProjectSeriesMetaInfo))] // Added MetaInfo dependency
public class ProjectSeries
{
    /// <summary>
    /// Unique identifier for the series.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Collection of projects belonging to this series.
    /// </summary>
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public Guid ProjectSeriesMetaInfoId {get;set;}
    
    // New Relationship to the identity anchor
    [ForeignKey("ProjectSeriesMetaInfoId")]
    public virtual ProjectSeriesMetaInfo ProjectSeriesMetaInfo { get; set; } = null!;
}
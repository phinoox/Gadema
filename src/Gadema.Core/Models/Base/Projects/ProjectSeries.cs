// =============================================================================

using Gadema.Core.Enums;
using Gadema.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gadema.Core.Models.Base.MetaInfo; // Added for MetaInfo access
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Base.Projects;

/// <summary>
/// Represents a collection of related projects (e.g., a book series or game franchise).
/// </summary>
[ModelDependency(typeof(RootMarker), typeof(ProjectSeriesMetaInfo))] // Added MetaInfo dependency
public class ProjectSeries
{
    /// <summary>
    /// Unique identifier for the series.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    // --- Identity (Moved to ProjectSeriesMetaInfo) ---
    // Title, Slug, Description removed

    /// <summary>
    /// Collection of projects belonging to this series.
    /// </summary>
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    // New Relationship to the identity anchor
    public virtual ProjectSeriesMetaInfo MetaInfo { get; set; } = null!;
}
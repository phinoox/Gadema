// =============================================================================

using Gadema.Core.Enums;
using Gadema.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Projects;

/// <summary>
/// Represents a collection of related projects (e.g., a book series or game franchise).
/// </summary>
[ModelDependency(typeof(RootMarker))]
public class ProjectSeries
{
    /// <summary>
    /// Unique identifier for the series.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Title of the series.
    /// </summary>
    [Required, Display(Name = "Series Title")]
    public string Title { get; set; } = "";

    /// <summary>
    /// URL-friendly slug for the series (unique).
    /// </summary>
    [MaxLength(128), Column("slug"), Required, Display(Name = "URL Slug")]
    public string Slug { get; set; } = "";

    /// <summary>
    /// Series-wide description.
    /// </summary>
    [MaxLength(4096)]
    public string? Description { get; set; } = null!;

    /// <summary>
    /// Collection of projects belonging to this series.
    /// </summary>
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gadema.Core.Models.Projects;

/// <summary>
/// Junction table linking Project to ProjectTag.
/// </summary>
public class ProjectTagRelation
{
    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    public Guid ProjectTagId { get; set; }

    // Navigation
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; } = null!;

    [ForeignKey("ProjectTagId")]
    public virtual ProjectTag ProjectTag { get; set; } = null!;
}
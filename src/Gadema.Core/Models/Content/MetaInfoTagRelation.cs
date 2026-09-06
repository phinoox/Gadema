using Gadema.Core.Models.Content;
using Gadema.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gadema.Core.Models.Content;

/// <summary>
/// Junction table linking MetaInfo to MetaInfoTag.
/// </summary>
[DependencyResolver.ModelDependency(typeof(MetaInfo),typeof(MetaInfoTag))]
public class MetaInfoTagRelation
{
    [Required]
    public Guid MetaInfoId { get; set; }

    [Required]
    public Guid MetaInfoTagId { get; set; }

    // Navigation
    [ForeignKey("MetaInfoId")]
    public virtual MetaInfo MetaInfo { get; set; } = null!;

    [ForeignKey("MetaInfoTagId")]
    public virtual MetaInfoTag MetaInfoTag { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
using Gadema.Core.Models.Base.MetaInfo;
using Gadema.Core.Models.Content;
using Gadema.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gadema.Core.Models.Base.MetaInfo;

/// <summary>
/// Junction table linking ContentMetaInfo to MetaInfoTag.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo),typeof(ContentMetaInfoTag))]
public class ContentMetaInfoTagRelation
{
    [Required]
    public Guid MetaInfoId { get; set; }

    [Required]
    public Guid MetaInfoTagId { get; set; }

    // Navigation
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; } = null!;

    [ForeignKey("MetaInfoTagId")]
    public virtual ContentMetaInfoTag MetaInfoTag { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
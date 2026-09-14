using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gadema.Core.Models;
using Gadema.Core.Models.Tags;

namespace Gadema.Core.Models.Base;

/// <summary>
/// Junction table linking a MetaInfo (Content Anchor) to a MetaTag.
/// </summary>
public class ContentTagRelation
{
    [Required]
    public Guid MetaInfoId { get; set; }

    [Required]
    public Guid TagId { get; set; }

    // Navigation properties
    [ForeignKey("MetaInfoId")]
    public virtual MetaInfo MetaInfo { get; set; } = null!;

    [ForeignKey("TagId")]
    public virtual MetaTag Tag { get; set; } = null!;
}
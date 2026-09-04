using Gadema.Core.Models.Content;
using Gadema.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gadema.Core.Models.Content;

/// <summary>
/// Junction table linking ContentItem to ContentTag.
/// </summary>
public class ContentItemTag
{
    [Required]
    public Guid ContentItemId { get; set; }

    [Required]
    public Guid ContentTagId { get; set; }

    // Navigation
    [ForeignKey("ContentItemId")]
    public virtual ContentItem ContentItem { get; set; } = null!;

    [ForeignKey("ContentTagId")]
    public virtual ContentTag ContentTag { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
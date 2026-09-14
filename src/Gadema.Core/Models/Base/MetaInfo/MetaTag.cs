using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gadema.Core.Models.Tags;

/// <summary>
/// A universal tag used for categorization and discovery across the entire system.
/// </summary>
public class MetaTag
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(128)]
    public string Name { get; set; } = "";

    [Required, MaxLength(128), Column("slug")]
    public string Slug { get; set; } = "";
}
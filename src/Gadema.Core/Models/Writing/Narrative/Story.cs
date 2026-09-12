using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gadema.Core.Models.Projects;

namespace Gadema.Core.Models.Writing;

[ModelDependency(typeof(MetaInfo),typeof(Project))]
public class Story
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Name { get; set; } = "New Story";

    [Required] public Guid MetaInfoId { get; set; }
    [ForeignKey("MetaInfoId")]
    public virtual MetaInfo MetaInfo { get; set; } = null!;

    public string? Description { get; set; }
    
    // Narrative structure (Siblings under Story)
    public virtual StoryOutline Outline { get; set; }
    public virtual ICollection<StoryBeat> Beats { get; set; } = new List<StoryBeat>();
    public virtual ICollection<StoryChapter> Chapters { get; set; } = new List<StoryChapter>();

    
    
}
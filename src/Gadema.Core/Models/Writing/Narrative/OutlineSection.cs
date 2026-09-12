using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gadema.Core.Models.Writing;

[ModelDependency(typeof(StoryOutline))]
public class OutlineSection
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Title { get; set; } = "New Section";

    public string RawText { get; set; } = "";

    public int SortOrder { get; set; } = 0;

    // Links this section to structural landmarks
    public List<StoryBeat> LinkedBeats { get; set; } = new();

    public Guid StoryOutlineId { get; set; }
    [ForeignKey("StoryOutlineId")]
    public virtual StoryOutline StoryOutline { get; set; }

}
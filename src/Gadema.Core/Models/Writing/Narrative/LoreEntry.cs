using Gadema.Core.Models.Base.Projects;
using Gadema.Core.Models.Writing.Enums;

namespace Gadema.Core.Models.Writing.Narrative;

/// <summary>
/// Represents a piece of world-building information (lore) within a project.
/// Used to document setting details, history, geography, and mythology.
/// </summary>
[ModelDependency(typeof(Project), typeof(ContentMetaInfo))]
public class LoreEntry
{
    /// <summary>
    /// Unique identifier for the lore entry.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
        /// The ID of the associated ContentMetaInfo entity, used for identity anchoring and searching.
    /// </summary>
    [Required]
    public Guid MetaInfoId { get; set; }  
    
    /// <summary>
    /// Navigation property for the lore entry's identity anchor.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public  virtual ContentMetaInfo ContentMetaInfo { get; set;} = null!;

    /// <summary>
    /// The category of this lore entry (e.g., History, Mythology, Geography).
    /// </summary>
    [EnumDataType(typeof(LoreTypeEnum)), Required]
    public LoreTypeEnum LoreType { get; set; }
    
    /// <summary>
    /// The detailed narrative content or description of the lore entry.
    /// </summary>
    [MaxLength(4096)]
    public string? RawText { get; set; }
}
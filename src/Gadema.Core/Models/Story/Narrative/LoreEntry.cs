// =============================================================================
using Gadema.Core.Enums;
using Gadema.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models;

/// <summary>
/// Represents a lore entry (world-building information).
/// Used for documenting setting details, history, and mythology.
/// </summary>
[ModelDependency(typeof(Project), typeof(MetaInfo))]
public class LoreEntry
{
    /// <summary>
    /// Unique identifier for the lore entry.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid MetaInfoId { get; set; }  
    
    [ForeignKey("MetaInfoId")]
    public  virtual MetaInfo MetaInfo { get; set; } = null!;

        
    /// <summary>
    /// Type of lore (History, Mythology, Geography, etc.).
    /// </summary>
    [EnumDataType(typeof(LoreTypeEnum)), Required]
    public LoreTypeEnum LoreType { get; set; }
    
    /// <summary>
    /// RawText of the lore entry.
    /// </summary>
    [MaxLength(4096)]
    public string? RawText { get; set; }
    
   
}
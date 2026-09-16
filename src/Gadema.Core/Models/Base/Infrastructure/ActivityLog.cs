using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Gadema.Core.Models.Base.Projects;

namespace Gadema.Core.Models.Base.Infrastructure;

/// <summary>
/// Represents a business-level audit event within a project.
/// This is ancillary data used for accountability and history tracking.
/// </summary>
public class ActivityLog
{
    public Guid Id { get; set; } = Guid.NewGuid();

   [Required] 
    public Guid ProjectId { get; set; }

    // Necessary for EF Core to perform Cascade Delete
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; } = null!;
    // Who did it
    [Required] public Guid UserId { get; set; }

    // What happened (e.g., "Created", "Updated", "Deleted", "Revoked")
    [Required, MaxLength(64)] public string Action { get; set; } = "";

    // The type of entity involved (e.g., "Character", "DialogueBranch", "Token")
    [Required, MaxLength(64)] public string RelatedEntityType { get; set; } = "";

    // The ID of the specific object being acted upon
    public Guid? RelatedEntityId { get; set; }

    [MaxLength(1024)] public string Description { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
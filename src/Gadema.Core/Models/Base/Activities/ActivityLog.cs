using System.ComponentModel.DataAnnotations;

namespace Gadema.Core.Models;

/// <summary>
/// Represents a business-level audit event within a project.
/// This is ancillary data used for accountability and history tracking.
/// </summary>
public class ActivityLog
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // The "Anchor" this log belongs to (The Project)
    [Required] public Guid ProjectId { get; set; }

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
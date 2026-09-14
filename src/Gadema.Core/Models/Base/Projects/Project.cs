// =============================================================================
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Models.Projects;

/// <summary>
/// Represents a project in the game development management system.
/// Owned by a single User (CreatedBy).
/// </summary>
// ... existing code ...
[ModelDependency(typeof(User), typeof(ProjectMetaInfo))] // Added ProjectMetaInfo dependency
public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // --- Identity (Moved to ProjectMetaInfo) ---
    // Title, Slug, Status, Visibility, ViewMode, Timestamps removed

    // --- Domain Data ---
    [MaxLength(4096)]
    public string? Description { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    // --- Relationships ---
    [Fixture(FixtureHintEnum.Omit)]
    public Guid UserId { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    public Guid? ProjectSeriesId { get; set; }

    [ForeignKey("ProjectSeriesId")]
    public virtual ProjectSeries? ProjectSeries { get; set; }

    // New Relationship to the identity anchor
    public virtual ProjectMetaInfo MetaInfo { get; set; } = null!;

    // ... remaining relationships (Members, Tokens, Tasks, etc.) remain unchanged ...

    // --- Domain Specific Metadata ---
    public bool EnableUserRegistration { get; set; } = false;
    public bool AllowManualInvites { get; set; } = true;

    [EnumDataType(typeof(PrimaryFormatEnum)), Required]
    public PrimaryFormatEnum PrimaryFormat { get; set; } = PrimaryFormatEnum.Book;

    [MaxLength(128)]
    public string? Genre { get; set; }

    [MaxLength(128)]
    public string? Theme { get; set; }

    [EnumDataType(typeof(ToneEnum))]
    public ToneEnum Tone { get; set; } = ToneEnum.Neutral;

    [EnumDataType(typeof(AudienceEnum))]
    public AudienceEnum Audience { get; set; } = AudienceEnum.AllAges;
    public ICollection<ProjectMember> Members { get; set; }
}
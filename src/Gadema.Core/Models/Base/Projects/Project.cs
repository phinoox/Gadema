// =============================================================================
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Access;
using Gadema.Core.Models.Base.Projects.Enums;

namespace Gadema.Core.Models.Base.Projects;

/// <summary>
/// Represents a project in the game development management system.
/// Owned by a single User (CreatedBy).
/// </summary>
[ModelDependency(typeof(User), typeof(ProjectMetaInfo))] 
public class Project : ISoftDeletable
{
    [Key]
    public Guid Id { get; set; }

    // --- Domain Data ---
    [MaxLength(4096)]
    public string? Description { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    // --- Relationships ---
    public Guid UserId { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    public Guid? ProjectSeriesId { get; set; }

    [ForeignKey("ProjectSeriesId")]
    public virtual ProjectSeries? ProjectSeries { get; set; }

    // The Id of the project is now also the FK to its MetaInfo (Vertical Unification)
    [ForeignKey("Id")]
    public virtual ProjectMetaInfo ProjectMetaInfo { get; set; } = null!;

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

    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}

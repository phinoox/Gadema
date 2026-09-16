using Gadema.Core.Enums;
using Gadema.Core.Dtos.Response;

namespace Gadema.Core.Dtos.Base.Projects;

// 1. Identity Data for creation
public class ProjectMetaInfoCreateData : BaseMetaInfoCreateData
{
    public ProjectStatusEnum Status { get; set; }
    
    public ViewModeEnum ViewMode { get; set; }
    
}

// 2. Create DTO (The entry point)
public class ProjectCreateDto
{
    public ProjectMetaInfoCreateData MetaInfo { get; set; } = null!;
    public string? Description { get; set; }
    public bool EnableUserRegistration { get; set; }
    public bool AllowManualInvites { get; set; }
    public PrimaryFormatEnum PrimaryFormat { get; set; }
    public string? Genre { get; set; }
    public string? Theme { get; set; }
    public ToneEnum Tone { get; set; }
    public AudienceEnum Audience { get; set; }
    public ContentStatusEnum? ProjectStatus { get; set; }
}

// 3. Update DTO (The sync payload)
public class ProjectUpdateDto
{
    public string? Description { get; set; }
    public bool? EnableUserRegistration { get; set; }
    public bool? AllowManualInvites { get; set; }
    public PrimaryFormatEnum? PrimaryFormat { get; set; }
    public string? Genre { get; set; }
    public string? Theme { get; set; }
    public ToneEnum? Tone { get; set; }
    public AudienceEnum? Audience { get; set; }
    
    // The identity payload that includes the tags to sync
    public ProjectMetaInfoUpdateData? ContentMetaInfo { get; set; }
}



// 5. Response DTO (The flattened "Contract")
public class ProjectResponseDto 
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Slug { get; set; } = "";
    public ProjectStatusEnum Status { get; set; }
    public ProjectVisibilityEnum Visibility { get; set; }
    public ViewModeEnum ViewMode { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public PrimaryFormatEnum PrimaryFormat { get; set; }
    public string? Genre { get; set; }
    public string? Theme { get; set; }
    public ToneEnum Tone { get; set; }
    public AudienceEnum Audience { get; set; }
}
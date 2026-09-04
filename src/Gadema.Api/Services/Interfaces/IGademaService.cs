// =============================================================================
// Gadema.Core.Services - Clean Interface (No Redundancy)
// =============================================================================

namespace Gadema.Core.Services;

/// <summary>
/// Enumeration of all service types in the application.
/// </summary>
public enum ServiceTypeEnum
{
    ContentService,
    ProjectService,
    ProjectTagService,
    ProjectTaskService,
    AuthService,
    DialogueService,
    CommentService,
    ExternalReferenceService,
    StoryOutlineService,
    ExportService,
    ExportContentService,
    TagService,
    ReviewService,
    SearchService
}

/// <summary>
/// Base interface for all services - identifies service type only.
/// </summary>
public interface IGademaService
{
    /// <summary>Gets the type of this service (unique identifier).</summary>
    ServiceTypeEnum ServiceType { get; }
}
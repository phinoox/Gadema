// =============================================================================
// =============================================================================

namespace Gadema.Core.Dtos.Base.Infrastructure;

/// <summary>
/// Data transfer object used to upload a media file to a content item.
/// </summary>
public class UploadMediaDto
{
    // Note: The actual file stream is typically handled via multipart/form-data in the controller,
    // not directly as a property of this DTO in many API implementations.
}

// =============================================================================
// Export response DTOs - Collection of export-related response types
// =============================================================================

namespace Gadema.Core.Dtos.Export;

/// <summary>
/// Generic API result for exports.
/// </summary>
public class ExportApiResponseDto<T>
{
    public T Content { get; set; } = default!;
    
    public bool Success { get; set; }
    
    public string? Error { get; set; }
}

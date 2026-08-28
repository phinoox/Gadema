// =============================================================================
using System.ComponentModel.DataAnnotations;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Gadema.Core.Dtos.Response;

/// <summary>
/// Base class for simple API responses with success status and message.
/// </summary>
public class SimpleResponseDto
{
    /// <summary>
    /// Whether the operation was successful.
    /// </summary>
    [Display(Name = "Success")]
    public bool Success { get; set; } = true;

    /// <summary>
    /// Message describing the result.
    /// </summary>
    [MaxLength(1024)]
    public string? Message { get; set; }
}

/// <summary>
/// Simple success response (no message).
/// </summary>
public class SuccessResponseDto : SimpleResponseDto
{
    /// <summary>
    /// Indicates successful operation.
    /// </summary>
    public bool Success => true;
}

/// <summary>
/// Simple error response.
/// </summary>
public class ErrorResponseDto : SimpleResponseDto
{
    /// <summary>
    /// Error message.
    /// </summary>
    [Display(Name = "Message")]
    public new string? Message { get; set; }

    /// <summary>
    /// HTTP status code.
    /// </summary>
    public int StatusCode { get; set; }
}
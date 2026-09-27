namespace Gadema.Core.Dtos.Response;

/// <summary>
/// Base class for simple API responses containing a success flag and an optional message.
/// </summary>
public class SimpleResponseDto
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    [Display(Name = "Success")]
    public bool Success { get; set; } = true;

    /// <summary>
    /// A message describing the outcome of the operation.
    /// </summary>
    [MaxLength(1024)]
    public string? Message { get; set; }
}

/// <summary>
/// Represents a successful operation with no additional descriptive message required.
/// </summary>
public class SuccessResponseDto : SimpleResponseDto
{
    /// <summary>
    /// Indicates that the operation was successful.
    /// </summary>
    public new bool Success => true;
}

/// <summary>
/// Represents an error response containing a descriptive message and an HTTP status code.
/// </summary>
public class ErrorResponseDto : SimpleResponseDto
{
    /// <summary>
    /// The specific error message describing what went wrong.
    /// </summary>
    [Display(Name = "Message")]
    public new string? Message { get; set; }

    /// <summary>
    /// The HTTP status code associated with the error.
    /// </summary>
    public int StatusCode { get; set; }
}
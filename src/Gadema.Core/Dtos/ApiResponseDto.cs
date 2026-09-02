// =============================================================================
using System.Net;

namespace Gadema.Core.Dtos;

/// <summary>
/// Base API response wrapper for all endpoints.
/// Provides type-safe, consistent responses across the application.
/// </summary>
public class ApiResponseDto<T> where T : class
{
    /// <summary>
    /// Whether the request was successful.
    /// </summary>
    public bool Successful { get; set; } = true;

    /// <summary>
    /// HTTP status code.
    /// </summary>
    public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;

    /// <summary>
    /// Error message (if failed).
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// List of error messages.
    /// </summary>
    public List<string>? Errors { get; set; } = null!;

    /// <summary>
    /// Data payload (if successful).
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Create a success response.
    /// </summary>
    public static ApiResponseDto<T> Success(T data) => new()
    {
        Successful = true,
        StatusCode = HttpStatusCode.OK,
        Message = null!,
        Errors = null!,
        Data = data
    };

    /// <summary>
    /// Create a not found response.
    /// </summary>
    public static ApiResponseDto<T> NotFound(string message) => new()
    {
        Successful = false,
        StatusCode = HttpStatusCode.NotFound,
        Message = message,
        Errors = null!,
        Data = default!
    };

    /// <summary>
    /// Create a bad request response.
    /// </summary>
    public static ApiResponseDto<T> BadRequest(string message) => new()
    {
        Successful = false,
        StatusCode = HttpStatusCode.BadRequest,
        Message = message,
        Errors = null!,
        Data = default!
    };

    /// <summary>
    /// Create an unauthorized response.
    /// </summary>
    public static ApiResponseDto<T> Unauthorized(string message) => new()
    {
        Successful = false,
        StatusCode = HttpStatusCode.Unauthorized,
        Message = message,
        Errors = null!,
        Data = default!
    };

    /// <summary>
    /// Create a conflict (duplicate resource) response.
    /// </summary>
    public static ApiResponseDto<T> Conflict(string message) => new()
    {
        Successful = false,
        StatusCode = HttpStatusCode.Conflict,
        Message = message,
        Errors = null!,
        Data = default!
    };

    /// <summary>
    /// Create a forbidden response.
    /// </summary>
    public static ApiResponseDto<T> Forbidden(string message) => new()
    {
        Successful = false,
        StatusCode = HttpStatusCode.Forbidden,
        Message = message,
        Errors = null!,
        Data = default!
    };

    /// <summary>
    /// Create a server error response.
    /// </summary>
    public static ApiResponseDto<T> ServerError(string message) => new()
    {
        Successful = false,
        StatusCode = HttpStatusCode.InternalServerError,
        Message = message,
        Errors = null!,
        Data = default!
    };

    /// <summary>
    /// Create a response with multiple errors.
    /// </summary>
    public static ApiResponseDto<T> WithErrors(List<string> messages) => new()
    {
        Successful = false,
        StatusCode = HttpStatusCode.BadRequest,
        Message = null!,
        Errors = messages ?? new List<string>(),
        Data = default!
    };
}
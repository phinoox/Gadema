// =============================================================================
using GameDev.Core.Dtos;
// GameDev.Api - ASP.NET Core Web API Configuration Classes
// =============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GameDev.Data;
using Microsoft.Extensions.Logging;

namespace GameDev.Api.Services;

/// <summary>
/// Pagination response wrapper.
/// </summary>
public class PaginationResponse<T> where T : class
{
    /// <summary>
    /// Current page number.
    /// </summary>
    public int CurrentPage { get; set; } = 1;

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Total number of items.
    /// </summary>
    public int TotalItems { get; set; } = 0;

    /// <summary>
    /// Total number of pages.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

    /// <summary>
    /// List of items.
    /// </summary>
    public List<T> Items { get; set; } = new();
}

/// <summary>
/// API response DTO wrapper.
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
    public HttpStatusCode StatusCode { get; set; } = System.Net.HttpStatusCode.OK;

    /// <summary>
    /// Error message (if failed).
    /// </summary>
    public string? Message { get; set; } = null!;

    /// <summary>
    /// Errors list.
    /// </summary>
    public List<string>? Errors { get; set; } = null!;

    /// <summary>
    /// Data (if successful).
    /// </summary>
    public T? Data { get; set; } = null!;

    /// <summary>
    /// Create success response.
    /// </summary>
    public static ApiResponseDto<T> Success(T data) => new()
    {
        Successful = true,
        StatusCode = System.Net.HttpStatusCode.OK,
        Message = null!,
        Errors = null!,
        Data = data
    };

    /// <summary>
    /// Create not found response.
    /// </summary>
    public static ApiResponseDto<T> NotFound(string message) => new()
    {
        Successful = false,
        StatusCode = System.Net.HttpStatusCode.NotFound,
        Message = message,
        Errors = null!,
        Data = default!
    };

    /// <summary>
    /// Create bad request response.
    /// </summary>
    public static ApiResponseDto<T> BadRequest(string message) => new()
    {
        Successful = false,
        StatusCode = System.Net.HttpStatusCode.BadRequest,
        Message = message,
        Errors = null!,
        Data = default!
    };
}



/// <summary>
/// Exception for validation errors.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Error messages.
    /// </summary>
    public string[] Messages { get; }

    /// <summary>
    /// Constructor with error messages.
    /// </summary>
    public ValidationException(string[] messages)
    {
        Messages = messages ?? new string[0];
    }
}

/// <summary>
/// Exception for not found errors.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Constructor with entity name and ID.
    /// </summary>
    public NotFoundException(string entity, Guid id)
        : base($"{entity} with ID {id} not found") { }
}

/// <summary>
/// Global exception handler configuration.
/// </summary>
public class ExceptionHandlerConfiguration
{
    /// <summary>
    /// Unhandled exception handler.
    /// </summary>
    public static async Task HandleExceptionAsync(Exception ex)
    {
        // Log error and return appropriate response
        _ = ex;
    }

    /// <summary>
    /// Validation exception handler.
    /// </summary>
    public static void HandleValidationException(ValidationException ex)
    {
        // Handle validation errors
        _ = ex;
    }

    /// <summary>
    /// Not found exception handler.
    /// </summary>
    public static void HandleNotFoundException(NotFoundException ex)
    {
        // Handle not found errors
        _ = ex;
    }
}

/// <summary>
/// Global feature flags configuration.
/// </summary>
public class FeatureFlagsConfiguration
{
    /// <summary>
    /// Enable user registration globally.
    /// </summary>
    public bool EnableUserRegistration { get; set; } = false;

    /// <summary>
    /// Allow Google OAuth authentication.
    /// </summary>
    public bool AllowGoogleOAuth { get; set; } = true;

    /// <summary>
    /// Enable two-factor authentication.
    /// </summary>
    public bool Enable2FA { get; set; } = true;
}

/// <summary>
/// Global configuration constants.
/// </summary>
public static class GlobalConfigurationConstants
{
    /// <summary>
    /// Default page size for list endpoints.
    /// </summary>
    public const int DefaultPageSize = 20;

    /// <summary>
    /// Maximum page size allowed.
    /// </summary>
    public const int MaxPageSize = 100;

    /// <summary>
    /// JWT token expiration in minutes (default).
    /// </summary>
    public const int DefaultJwtExpiryMinutes = 60;

    /// <summary>
    /// Refresh token expiration in days.
    /// </summary>
    public const int RefreshTokenExpiryDays = 30;
}
// =============================================================================
using Microsoft.AspNetCore.Http;
// GameDev.Api - ASP.NET Core Web API Controllers
// =============================================================================

using System.Threading.Tasks;
using GameDev.Api.Services;
using GameDev.Core.Dtos.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameDev.Api.Controllers;

/// <summary>
/// Controller for user authentication endpoints.
/// </summary>
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IApiAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Constructor with dependency injection.
    /// </summary>
    public AuthController(IApiAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Traditional login (email/password).
    /// </summary>
    [HttpPost("signin")]
    public async Task<IActionResult> SignInAsync([FromBody] SignInDto signInDto)
    {
        return Ok(await _authService.SignInAsync(signInDto));
    }

    /// <summary>
    /// Google OAuth callback.
    /// </summary>
    [HttpPost("callback/google")]
    public async Task<IActionResult> GoogleCallbackAsync([FromQuery] GoogleCallbackDto callbackDto)
    {
        return Ok(await _authService.GoogleCallbackAsync(callbackDto.Code));
    }

    /// <summary>
    /// Two-factor authentication login.
    /// </summary>
    [HttpPost("signin-with-2fa")]
    public async Task<IActionResult> SignInWith2FAAsync([FromBody] SignInWith2FADto signInDto)
    {
        return Ok(await _authService.SignInWith2FAAsync(signInDto));
    }

    /// <summary>
    /// View recovery codes.
    /// </summary>
    [HttpGet("recovery-codes")]
    public async Task<IActionResult> GetRecoveryCodesAsync([FromQuery] RecoveryCodesDto recoveryDto)
    {
        return Ok(await _authService.GetRecoveryCodesAsync(recoveryDto));
    }

    /// <summary>
    /// Disable two-factor authentication.
    /// </summary>
    [HttpDelete("2fa/disable")]
    public async Task<IActionResult> Disable2FAAsync([FromBody] Disable2FADto disableDto)
    {
        return Ok(await _authService.Disable2FAAsync(disableDto));
    }
}
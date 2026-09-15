using Microsoft.AspNetCore.Mvc;
using Gadema.Api.Services.Authentication;
using Gadema.Core.Dtos.Authentication;

namespace Gadema.Api.Controllers.Authentication;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly EmailPasswordAuthService _authService;

    public AuthController(EmailPasswordAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return result.Successful ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Signs in a user and returns an access token.
    /// </summary>
    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInDto dto)
    {
        var result = await _authService.SignInAsync(dto);
        return result.Successful ? Ok(result) : Unauthorized(result);
    }

    // Note: If you implement 2FA/Setup, you would add endpoints here 
    // like [HttpPost("verify-pending")] using the JwtTokenService pending tokens.
}
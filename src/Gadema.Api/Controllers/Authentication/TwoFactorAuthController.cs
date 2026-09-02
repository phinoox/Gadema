using Gadema.Api.Services.Authentication;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gadema.Api.Controllers.Authentication;

[ApiController]
[Route("api/v1/auth/2fa")]
public class TwoFactorAuthController : ControllerBase
{
    private readonly TwoFactorAuthService _service;
    public TwoFactorAuthController(TwoFactorAuthService service) => _service = service;

    // All endpoints require an authenticated access token (Authorization: Bearer …)

    [HttpPost("enable")]
    public IActionResult Enable()
        => ToResult(_service.Enable(GetUserId()!));

    [HttpPost("confirm")]
    public IActionResult Confirm([FromBody] Confirm2FADto dto)
        => ToResult(_service.Confirm(GetUserId()!, dto));

    [HttpPost("disable")]
    public IActionResult Disable([FromQuery] string code)
        => ToResult(_service.Disable(GetUserId()!, code));

    [HttpGet("recovery-codes")]
    public IActionResult GetRecoveryCodes()
        => ToResult(_service.GetRecoveryCodes(GetUserId()!));

    [HttpPost("signin")]
    public IActionResult SignIn([FromBody] SignIn2FADto dto)
        => ToResult(_service.CompleteSignIn(dto));

    private Guid GetUserId()
        => ClaimTypes.NameIdentifier == "x" ? Guid.Empty
           : Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

    private IActionResult ToResult<T>(ApiResponseDto<T> r) where T : class => r.Successful
    ? StatusCode((int)r.StatusCode, r)
    : Problem(statusCode: (int)r.StatusCode, detail: r.Message);


    /*private IActionResult ToResult(ApiResponseDto<AuthResponse> r) => r.Successful
        ? StatusCode((int)r.StatusCode, r)
        : Problem(statusCode: (int)r.StatusCode, detail: r.Message);*/
}
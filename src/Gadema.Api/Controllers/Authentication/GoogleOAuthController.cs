using Gadema.Api.Services.Authentication;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Authentication;

[ApiController]
[Route("api/v1/auth/google")]
public class GoogleOAuthController : ControllerBase
{
    private readonly GoogleOAuthService _service;
    public GoogleOAuthController(GoogleOAuthService service) => _service = service;

    [HttpPost("signin")]
    public Task<IActionResult> SignInAsync([FromBody] GoogleSignInDto dto)
        => Task.FromResult(ToResult(_service.SignIn(dto)));

    [HttpPost("link")]
    public IActionResult Link()
        => ToResult(_service.LinkToCurrentUser(GetUserId()!, dto: null!));

    private Guid GetUserId()
        => Guid.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

    private IActionResult ToResult(ApiResponseDto<AuthResponse> r) => r.Successful
        ? StatusCode((int)r.StatusCode, r)
        : Problem(statusCode: (int)r.StatusCode, detail: r.Message);
}
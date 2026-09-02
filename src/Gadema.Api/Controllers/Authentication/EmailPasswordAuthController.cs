using Gadema.Api.Services;
using Gadema.Api.Services.Authentication;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Gadema.Api.Controllers.Authentication;

[ApiController]
[Route("api/v1/auth/email")]
public class EmailPasswordAuthController : ControllerBase
{
    private readonly EmailPasswordAuthService _service;
    public EmailPasswordAuthController(EmailPasswordAuthService service) => _service = service;

    [HttpPost("register")]
    public Task<IActionResult> RegisterAsync([FromBody] RegisterDto dto)
        => Task.FromResult(ToResult(_service.Register(dto)));

    [HttpPost("signin")]
    public Task<IActionResult> SignInAsync([FromBody] SignInDto dto)
        => Task.FromResult(ToResult(_service.SignIn(dto)));

    [HttpPost("link")]
    public Task<IActionResult> LinkProviderAsync([FromBody] LinkProviderDto dto)
        => Task.FromResult(ToResult(_service.LinkProvider(dto)));

    private IActionResult ToResult(ApiResponseDto<AuthResponse> r) => r.Successful
        ? StatusCode((int)r.StatusCode, r)
        : Problem(statusCode: (int)r.StatusCode, detail: r.Message);
}
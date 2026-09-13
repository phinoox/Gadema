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
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto dto){
       return Ok(await _service.RegisterAsync(dto));
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignInAsync([FromBody] SignInDto dto){
       return Ok(await _service.SignInAsync(dto));
    }

  
    private IActionResult ToResult(ApiResponseDto<AuthResponse> r) => r.Successful
        ? StatusCode((int)r.StatusCode, r)
        : Problem(statusCode: (int)r.StatusCode, detail: r.Message);
}
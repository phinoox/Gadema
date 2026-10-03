using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gadema.Api.Services.Core;
using Gadema.Core.Dtos.Access;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Response;
using Gadema.Api.Services.Users;

namespace Gadema.Api.Controllers.Users;

[Authorize]
[ApiController]
[Route("api/v1/userprofiles")]
public class UserProfileController : ControllerBase
{
    private readonly ProfileService _profileService;

    public UserProfileController(ProfileService profileService)
    {
        _profileService = profileService;
    }

    /// <summary>
    /// Gets the current user's profile.
    /// </summary>
    [HttpGet]
    public async Task<ApiResponseDto<UserProfileResponseDto>> Get()
    {
        return await _profileService.GetProfileAsync();
    }

    /// <summary>
    /// Updates the current user's profile.
    /// </summary>
    [HttpPatch]
    public async Task<ApiResponseDto<SimpleResponseDto>> Update([FromBody] UserProfileUpdateDto updateDto)
    {
        return await _profileService.UpdateProfileAsync(updateDto);
    }
}

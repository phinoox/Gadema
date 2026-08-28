// =============================================================================
using Gadema.Core.Dtos;
// Gadema.Api - ASP.NET Core Web API Services
// =============================================================================

using System;
using System.Linq;
using System.Threading.Tasks;
using Gadema.Core.Dtos.Authentication;
using Gadema.Data;
using Microsoft.Extensions.Logging;

namespace Gadema.Api.Services;

/// <summary>
/// Interface for authentication service.
/// </summary>
public interface IApiAuthService
{
    Task<ApiResponseDto<AuthResponse>> SignInAsync(SignInDto signInDto);
    Task<ApiResponseDto<AuthResponse>> GoogleCallbackAsync(string code);
    Task<ApiResponseDto<AuthResponse>> SignInWith2FAAsync(SignInWith2FADto signInDto);
    Task<ApiResponseDto<RecoveryCodesResponseDto>> GetRecoveryCodesAsync(RecoveryCodesDto recoveryDto);
    Task<ApiResponseDto<AuthResponse>> Disable2FAAsync(Disable2FADto disableDto);
}
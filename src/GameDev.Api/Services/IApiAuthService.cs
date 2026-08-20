// =============================================================================
// GameDev.Api - ASP.NET Core Web API Services
// =============================================================================

namespace GameDev.Api.Services;

/// <summary>
/// Interface for authentication service.
/// </summary>
public interface IApiAuthService
{
    Task<ApiResponseDto<AuthResponse>> SignInAsync(SignInDto signInDto);
    Task<ApiResponseDto<AuthResponse>> GoogleCallbackAsync(string code);
    Task<ApiResponseDto<AuthResponse>> SignInWith2FAAsync(SignInWith2FADto signInDto);
    Task<ApiResponseDto<RecoveryCodesResponse>> GetRecoveryCodesAsync(RecoveryCodesDto recoveryDto);
    Task<ApiResponseDto<object>> Disable2FAAsync(Disable2FADto disableDto);
}
using Gadema.Api.CoreServices;
using Gadema.Api.CoreServices.Strategies;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Access;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Access;
using Gadema.Data.Database.Core;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Users;

/// <summary>
/// Service for managing user profile (UserMetaInfo) operations.
/// </summary>
[ServiceLifetime(ServiceLifetime.Scoped)] 
public class ProfileService : DomainService
{
    private readonly CoreDbContext _db;

    public ProfileService(CoreDbContext db, ICoreServicesProvider coreServices, ILogger<ProfileService> logger) 
        : base(coreServices, logger)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves the profile information for the currently authenticated user.
    /// </summary>
    public async Task<ApiResponseDto<UserProfileResponseDto>> GetProfileAsync()
    {
        if (_userId == Guid.Empty)
            return ApiResponseDto<UserProfileResponseDto>.Unauthorized("User not authenticated.");

        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == _userId);
        if (user == null)
            return ApiResponseDto<UserProfileResponseDto>.NotFound("User not found.");

        var profile = new UserProfileResponseDto
        {
            UserId = user.Id,
            Title = user.MetaInfo.Title,
            ShortDesc = user.MetaInfo.ShortDesc,
            Slug = user.MetaInfo.Slug,
            AvatarUrl = user.MetaInfo.AvatarUrl,
            CreatedAt = user.MetaInfo.CreatedAt
        };

        return ApiResponseDto<UserProfileResponseDto>.Success(profile);
    }

    /// <summary>
    /// Updates the profile information for the currently authenticated user using the sync pattern.
    /// </summary>
    public async Task<ApiResponseDto<SimpleResponseDto>> UpdateProfileAsync(UserProfileUpdateDto updateDto)
    {
        if (_userId == Guid.Empty)
            return ApiResponseDto<SimpleResponseDto>.Unauthorized("User not authenticated.");

        var success = await SyncIdentityAsync<UserIdentityStrategy>(_userId, updateDto);
        if (!success) return ApiResponseDto<SimpleResponseDto>.ServerError("Sync failed.");


        return ApiResponseDto<SimpleResponseDto>.Success(new SimpleResponseDto() { Success = true });
    }
}


using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Authentication;
using Gadema.Core.Enums;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Gadema.Api.Services.Authentication;

public class EmailPasswordAuthService
{
    private readonly GameDbContext _db;
    private readonly JwtTokenService _jwt;

    private readonly IUserContext _userContext;

    public EmailPasswordAuthService(GameDbContext db, JwtTokenService jwt,IUserContext userContext)
    {
        _db = db;
        _jwt = jwt;
        _userContext = userContext;
    }

    public ApiResponseDto<AuthResponse> Register(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();
        var existing = _db.Users.FirstOrDefault(u => u.Email == email);
        if (existing != null)
            return ApiResponseDto<AuthResponse>.Conflict(
                $"A user with email '{email}' already exists. Link this provider to the existing account.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            UserName = email,
            FullName = dto.Name.Trim(),
            Provider = UserAuthProviderEnum.Password,
            PasswordHash = PasswordHasher.Hash(dto.Password),
            TwoFactorEnabled = false,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            LastLogin = DateTime.UtcNow
        };
        _db.Users.Add(user);
        _db.UserProviderLinks.Add(new UserProviderLink
        {
            UserId = user.Id,
            Provider = UserAuthProviderEnum.Password
        });
        _db.SaveChanges();

        return Ok(user, "password");
    }

    public ApiResponseDto<AuthResponse> SignIn(SignInDto dto)
    {
        var user = _db.Users.FirstOrDefault(u => u.Email == dto.Email.Trim().ToLowerInvariant() && u.IsActive);
        if (user == null || !PasswordHasher.Verify(dto.Password, user.PasswordHash))
            return ApiResponseDto<AuthResponse>.Unauthorized("Invalid email or password.");

        user.LastLogin = DateTime.UtcNow;
        _db.SaveChanges();
        // 2FA enabled → issue only a short-lived pending token, client goes to /2fa/signin
        if (user.TwoFactorEnabled)
            return ApiResponseDto<AuthResponse>.Success(new AuthResponse
            {
                TwoFactorRequired = true,
                TwoFactorToken = _jwt.IssuePendingToken(user.Id, "2fa_signin"),
                User = UserDto(user),
                Message = "Two-factor code required."
            });

        return Ok(user, "password");
    }

    /// <summary>
    /// Links the Password provider to an existing account (email already used by another provider).
    /// Sets a password on the account and records the provider link.
    /// </summary>
       public ApiResponseDto<AuthResponse> LinkProvider(LinkProviderDto dto)
    {
        if (dto.Provider != UserAuthProviderEnum.Password)
            return ApiResponseDto<AuthResponse>.BadRequest("This endpoint only links the Password provider.");

        var user = _db.Users.FirstOrDefault(u => u.Id == dto.ExistingUserId);
        if (user == null)
            return ApiResponseDto<AuthResponse>.NotFound("Existing user not found.");

        // Guard: user must not already have a password
        if (user.PasswordHash != null)
            return ApiResponseDto<AuthResponse>.Conflict("This account already has a password set.");

        user.PasswordHash = PasswordHasher.Hash(dto.Password!);
        _db.UserProviderLinks.Add(new UserProviderLink
        {
            UserId = user.Id,
            Provider = UserAuthProviderEnum.Password,
        });
        _db.SaveChanges();

        return Ok(user, "password");
    }

    private ApiResponseDto<AuthResponse> Ok(User user, string authMethod)
        => ApiResponseDto<AuthResponse>.Success(new AuthResponse
        {
            AccessToken  = _jwt.IssueAccessToken(user.Id, user.Email, user.FullName, authMethod),
            TokenType    = "Bearer",
            ExpiresInSeconds = 3600,
            User         = UserDto(user),
            Message      = "OK"
        });

    private static UserResponse UserDto(User u)
        => new()
        {
            Id                = u.Id.ToString(),
            Name              = u.FullName ?? "",
            Email             = u.Email,
            GoogleSubjectId   = u.GoogleSubjectId
        };
}
// =============================================================================
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gadema.Core.Dtos;
using Gadema.Core.Dtos.Authentication;
using Gadema.Core.Dtos.Response;
using Gadema.Core.Models;
using Gadema.Core.Services;
using Gadema.Data.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Gadema.Api.Services.Access.Authentication;

/// <summary>
/// Service for email/password authentication operations.
/// Handles registration, sign-in, password reset requests, and session management.
/// </summary>
public class EmailPasswordAuthService : CoreService
{
    private readonly IConfiguration _configuration;

    public EmailPasswordAuthService(GameDbContext db, ILogger<EmailPasswordAuthService> logger, IUserContext userContext, IConfiguration configuration)
        : base(db, logger, userContext) => _configuration = configuration;


    // ========================================================================
    // POST - Register a new user (email + password)
    // ========================================================================

    public async Task<ApiResponseDto<AuthResponse>> RegisterAsync(RegisterDto registerDto)
    {
        var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
        if (existingUser != null)
            return ApiResponseDto<AuthResponse>.Conflict($"A user with email '{registerDto.Email}' already exists.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = registerDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            EmailConfirmed = true, // Setting to true for simplified flow ToDO: change for production
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var expiryDate = DateTime.UtcNow.AddHours(24);
        var authResponse = new AuthResponse
        {
            AccessToken = GenerateJwtToken(user.Id, user.Email),
            TokenType = "Bearer",
            ExpiresInSeconds = (int)(expiryDate - DateTime.UtcNow).TotalSeconds,
            User = new UserResponse 
            { 
                Id = user.Id.ToString(), 
                Email = user.Email, 
                Name = user.DisplayName ?? "" 
            }
        };

        return ApiResponseDto<AuthResponse>.Success(authResponse);
    }

    // ========================================================================
    // POST - Sign in with email + password
    // ========================================================================

    public async Task<ApiResponseDto<AuthResponse>> SignInAsync(SignInDto signInDto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == signInDto.Email);

        if (user == null)
            return ApiResponseDto<AuthResponse>.Unauthorized("Invalid email or password.");

        var isValidPassword = BCrypt.Net.BCrypt.Verify(signInDto.Password, user.PasswordHash);
        if (!isValidPassword)
            return ApiResponseDto<AuthResponse>.Unauthorized("Invalid email or password.");

        var expiryDate = DateTime.UtcNow.AddHours(24);
        var authResponse = new AuthResponse
        {
            AccessToken = GenerateJwtToken(user.Id, user.Email),
            TokenType = "Bearer",
            ExpiresInSeconds = (int)(expiryDate - DateTime.UtcNow).TotalSeconds,
            User = new UserResponse 
            { 
                Id = user.Id.ToString(), 
                Email = user.Email, 
                Name = user.DisplayName ?? "" 
            }
        };

        return ApiResponseDto<AuthResponse>.Success(authResponse);
    }

    // ... existing RequestPasswordResetAsync and ResetPasswordAsync methods ...

    // ========================================================================
    // Helpers
    // ========================================================================

    private string GenerateJwtToken(Guid userId, string email)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        // Fixed key to match Program.cs: "Jwt:Secret" instead of "JWT:SecretKey"
        var keyStr = _configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured.");
        var key = Encoding.UTF8.GetBytes(keyStr);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { 
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), 
                new Claim(ClaimTypes.Email, email) 
            }),
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken() => Guid.NewGuid().ToString("N");

    // Removed 2FA helper methods (Verify2FACodeAsync, GenerateRecoveryCodes)
}
   
  
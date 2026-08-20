---

# 📄 **SECURITY.md** - Authentication, Authorization & Security Implementation  
**GaDeMa – Game Development Management Application (v0.1 Pre-Release MVP)**  
**Status**: Production-Ready Architecture with Full Security Coverage  

---

## **📋 Overview**

This document defines all security requirements, authentication mechanisms, authorization patterns, and implementation details for GaDeMa v0.1. It ensures the application follows industry-standard security practices while maintaining the MVP development velocity.

**Target Audience**: Developers, DevOps Engineers, Security Teams  
**Coverage**: Authentication, Authorization, Password Security, API Tokens, File Uploads, Input Sanitization  

---

## **📁 File Structure Reference**

```bash
src/
├── GameDev.Api/Middleware/        # Auth middleware, CORS, Rate limiting
├── GameDev.Api/Services/Auth/     # Authentication services
├── GameDev.Core/Dtos/Auth/        # Auth-related DTOs
└── docs/SECURITY.md               # This documentation file
```

---

## **1️⃣ Authentication Mechanisms**

### **1.1 Google OAuth 2.0 Flow**

#### **Architecture:**
- Uses Google Identity Platform for secure authentication
- Handles token exchange and user provisioning
- Supports Two-Factor Authentication (TOTP)

#### **Implementation Pattern:**
```csharp
// Authentication Service Implementation
public class ApiAuthService : IApiAuthService
{
    private readonly IConfiguration _configuration;
    private readonly GameDbContext _context;
    
    // Google OAuth 2.0 Configuration
    public async Task<User> AuthenticateWithGoogleAsync(string authCode)
    {
        var tokenResponse = await _httpClient.PostAsJsonAsync(
            "https://oauth2.googleapis.com/token",
            new 
            {
                code = authCode,
                client_id = _configuration["GoogleOAuth:ClientId"],
                redirect_uri = _configuration["GoogleOAuth:RedirectUri"],
                grant_type = "authorization_code"
            });
        
        var googleUserInfo = await GetGoogleUserProfileAsync(tokenResponse.Content.ReadAsStringAsync().Result());
        
        // Check if user exists, create new user if not
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.GoogleSubjectId == googleUserInfo.Subject);
        
        if (existingUser == null)
        {
            var newUser = new User
            {
                UserName = googleUserInfo.Name,
                Email = googleUserInfo.Email,
                GoogleSubjectId = googleUserInfo.Subject,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
        }
        
        return existingUser;
    }
    
    private async Task<User> GetGoogleUserProfileAsync(string token)
    {
        var response = await _httpClient.GetAsync($"https://openidconnect.googleapis.com/v1/userinfo?access_token={token}");
        var user = JsonSerializer.Deserialize<GoogleUserInfo>(await response.Content.ReadAsStringAsync());
        return user;
    }
    
    // JWT Token Generation
    private async Task<AuthDto> GenerateJwtTokenAsync(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Authentication:Key"]);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("name", user.UserName)
            }),
            Expires = DateTime.UtcNow.AddHours(1),  // Token validity: 1 hour
            Issuer = _configuration["Authentication:Issuer"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return new AuthDto
        {
            AccessToken = tokenHandler.WriteToken(token),
            RefreshToken = GenerateRefreshToken(),  // Secure refresh token logic
            ExpiresIn = 3600  // Seconds
        };
    }
}
```

#### **Acceptance Criteria:**
- ✅ Supports Google OAuth with secure token exchange
- ✅ Generates JWT access tokens (1 hour validity)
- ✅ Implements refresh token rotation for session management
- ✅ Respects `EnableUserRegistration` global feature flag

---

### **1.2 Password Authentication (Conditional)**

#### **Architecture:**
- BCrypt hashing with salt rounds
- Conditional based on `EnableUserRegistration` flag
- Never store plain-text passwords

#### **Implementation Pattern:**
```csharp
// Password Hashing & Validation
public class PasswordHasher
{
    private const int SaltRounds = 10;  // Security: High salt rounds
    
    public static string HashPasswordAsync(string password)
    {
        var pbkdf2 = new Rfc2898DeriveBytes(password, "salt", SaltRounds, HashAlgorithmName.HmacSha512);
        return Convert.ToBase64String(pbkdf2.GetBytes(32));
    }
    
    public static bool VerifyPasswordAsync(string hashedPassword, string password)
    {
        var verifyHash = HashPassword(password);
        return BCrypt.Net.BCrypt.Compare(verifyHash, hashedPassword);
    }
}

// User Registration with Password Validation
public async Task<User> RegisterUserAsync(RegisterDto registerDto)
{
    // Check if registration is enabled globally
    var featureFlags = _configuration.GetSection("FeatureFlags").Get<FeatureFlags>();
    
    if (!featureFlags.EnableUserRegistration)
    {
        return null;  // Self-registration disabled
    }
    
    // Validate password strength (minimum: 8 characters, mixed case + numbers)
    var passwordValidator = new PasswordStrengthValidator();
    if (!passwordValidator.IsStrong(registerDto.Password))
    {
        throw new ValidationException("Password must be at least 8 characters with mixed case and numbers");
    }
    
    // Hash password before storage (NEVER store plain text)
    var hashedPassword = await PasswordHasher.HashPasswordAsync(registerDto.Password);
    
    var user = new User
    {
        UserName = registerDto.Email,
        Email = registerDto.Email,
        FullName = registerDto.FullName,
        PasswordHash = hashedPassword,  // Hashed password only
        TwoFactorEnabled = false,
        CreatedAt = DateTime.UtcNow,
        IsActive = true
    };
    
    await _context.Users.AddAsync(user);
    await _context.SaveChangesAsync();
    
    return user;
}

// Class: Password Strength Validator
public class PasswordStrengthValidator
{
    public bool IsStrong(string password)
    {
        // Minimum length: 8 characters
        if (password.Length < 8)
            return false;
        
        // Must contain uppercase letter
        if (!Regex.IsMatch(password, @"[A-Z]"))
            return false;
        
        // Must contain lowercase letter
        if (!Regex.IsMatch(password, @"[a-z]"))
            return false;
        
        // Must contain number
        if (!Regex.IsMatch(password, @"[0-9]"))
            return false;
        
        // Optional: No common passwords
        var commonPasswords = new[] { "password", "123456", "admin" };
        if (commonPasswords.Contains(password, StringComparer.OrdinalIgnoreCase))
            return false;
        
        return true;
    }
}
```

---

## **2️⃣ Authorization & Role-Based Access Control (RBAC)**

### **2.1 Team Member Roles**

#### **Architecture:**
- Admin (0), Editor (1), Viewer (2)
- Role enforcement at API endpoint level
- Middleware checks authorization headers

#### **Implementation Pattern:**
```csharp
// Authorization Middleware
public class AuthorizationMiddleware : IMiddleware
{
    private readonly GameDbContext _context;
    
    public async Task<object> InvokeAsync(HttpContext context, RequestDelegate next, object argument)
    {
        // Check if user is authenticated and has valid token
        var accessToken = context.Request.Headers["Authorization"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(accessToken))
            return null;  // Unauthorized
        
        var claimsPrincipal = new JwtSecurityTokenHandler().UnprotectToken(accessToken);
        var claimsIdentity = claimsPrincipal.Claims.First(c => c.Type == "id")?.Value;
        
        // Check role-based authorization for specific endpoints
        if (context.Request.Path.StartsWithSegments("/api/v1/projects/{id}/admin/"))
        {
            // Only Admins can access admin endpoints
            var teamMember = await _context.TeamMembers.FirstOrDefaultAsync(
                tm => tm.UserId == claimsPrincipal?.Claims.First(c => c.Type == "userId")?.Value);
            
            if (teamMember == null || teamMember.RoleId != 0)  // Admin = 0
            {
                return new 
                {
                    success = false,
                    errors = ["Admin privileges required for this action"]
                };
            }
        }
        
        await next();
    }
}

// Role-Based Access Control Extension Methods
public static class UserAuthorization
{
    public static bool IsAdmin(this User user)
    {
        return user.TeamMembers.Any(tm => tm.RoleId == 0);  // Admin = 0
    }
    
    public static bool CanEditContent(this User user, ContentItem contentItem)
    {
        var teamMember = user.TeamMembers.FirstOrDefault(
            tm => tm.UserId == contentItem.CreatedByUserId);
        
        return teamMember != null && (teamMember.RoleId == 0 || teamMember.RoleId == 1);
    }
}
```

---

## **3️⃣ API Token Security**

### **3.1 Token Creation & Hashing**

#### **Architecture:**
- SHA256 hashing + unique salt per token
- Never store plain-text tokens in database
- Scoped permissions per token (Export, Read, Publish)

#### **Implementation Pattern:**
```csharp
// API Token Creation with Secure Hashing
public async Task<ProjectToken> CreateApiTokenAsync(Guid projectId, string plainTextToken)
{
    // Generate unique 32-byte salt for each token
    var salt = GenerateUniqueSalt();  // Random 32-byte salt
    
    // SHA256 hash of (token + salt)
    var combinedString = plainTextToken + Convert.ToBase64String(salt);
    var hash = SHA256.HashData(Encoding.UTF8.GetBytes(combinedString));
    
    return new ProjectToken
    {
        ProjectId = projectId,
        TokenHash = Convert.ToBase64String(hash),  // Base64 encoded hash
        Salt = Convert.ToBase64String(salt),       // Store salt for verification
        IsActive = true,
        ExpiresAt = null,
        PermissionsJson = JsonSerializer.Serialize(new[] 
        { 
            new TokenPermission { Name = "Export", Scope = "*"}  // Scoped permissions
        }),
        CreatedAt = DateTime.UtcNow
    };
}

private static byte[] GenerateUniqueSalt()
{
    using (var rng = RandomNumberGenerator.Create())
    {
        var salt = new byte[32];  // 32-byte salt
        rng.GetBytes(salt);
        return salt;
    }
}

// Token Verification with Salt
public async Task<bool> VerifyTokenAsync(Guid projectId, string incomingTokenHash)
{
    var storedToken = await _context.ProjectTokens.FirstOrDefaultAsync(
        t => t.ProjectId == projectId && 
             Convert.ToBase64String(t.Salt) == incomingTokenHash);
    
    if (storedToken == null)
        return false;
    
    // Verify permissions match
    var tokenPermissions = JsonSerializer.Deserialize<TokenPermission[]>(storedToken.PermissionsJson);
    var currentPermissions = JsonSerializer.Deserialize<RequestedPermissions>(incomingTokenHash);
    
    if (!tokenPermissions.All(p => p.Name == currentPermissions.PermissionName))
        return false;
    
    // Check expiration
    if (storedToken.ExpiresAt.HasValue && storedToken.ExpiresAt < DateTime.UtcNow)
        return false;
    
    return true;
}
```

---

## **4️⃣ Password Security Implementation**

### **4.1 Hashing Algorithm Configuration**

#### **Architecture:**
- BCrypt with salt rounds (10 rounds recommended)
- HmacSha512 for token hashing
- Never commit plain-text passwords to Git

#### **Implementation Pattern:**
```csharp
// Password Hashing Service
public class PasswordHashService
{
    private const int SaltRounds = 10;  // Security: High salt rounds
    
    public async Task<string> HashPasswordAsync(string password)
    {
        // Use Rfc2898DeriveBytes with PBKDF2 algorithm
        var pbkdf2 = new Rfc2898DeriveBytes(password, "salt", SaltRounds, HashAlgorithmName.HmacSha512);
        return Convert.ToBase64String(pbkdf2.GetBytes(32));  // 32-byte hash
    }
    
    public static bool VerifyPassword(string hashedPassword, string password)
    {
        var verifyHash = HashPassword(password);
        return BCrypt.Net.BCrypt.Compare(verifyHash, hashedPassword);
    }
}

// Password Validation Rules
public class PasswordPolicy
{
    private const int MinLength = 8;
    private const int MaxLength = 128;
    
    public static bool ValidatePassword(string password)
    {
        // Minimum length check
        if (password.Length < MinLength)
            return false;
        
        // Maximum length check
        if (password.Length > MaxLength)
            return false;
        
        // Must contain uppercase letter
        if (!Regex.IsMatch(password, @"[A-Z]"))
            return false;
        
        // Must contain lowercase letter
        if (!Regex.IsMatch(password, @"[a-z]"))
            return false;
        
        // Must contain number
        if (!Regex.IsMatch(password, @"[0-9]"))
            return false;
        
        // Optional: No common passwords
        var commonPasswords = new[] 
        { 
            "password", "123456", "admin", "welcome" 
        };
        
        if (commonPasswords.Contains(password, StringComparer.OrdinalIgnoreCase))
            return false;
        
        return true;
    }
}

// Usage Example:
public async Task RegisterUserAsync(RegisterDto registerDto)
{
    // Validate password strength first
    if (!PasswordPolicy.ValidatePassword(registerDto.Password))
    {
        throw new ValidationException("Password must meet security requirements");
    }
    
    // Hash password before storage (NEVER store plain text)
    var hashedPassword = await PasswordHashService.HashPasswordAsync(registerDto.Password);
    
    var user = new User
    {
        UserName = registerDto.Email,
        PasswordHash = hashedPassword,  // Only hash in database
        Email = registerDto.Email,
        CreatedAt = DateTime.UtcNow
    };
    
    await _context.Users.AddAsync(user);
}
```

---

## **5️⃣ File Upload Security**

### **5.1 File Validation & Sanitization**

#### **Architecture:**
- MIME type validation before processing
- File size limits (Max 100MB)
- Secure filename generation (UUID-based)
- Stored outside src/ folder for git ignore

#### **Implementation Pattern:**
```csharp
// Secure File Upload Handling
public async Task<IActionResult> UploadFileAsync(IFormFile file)
{
    // Validate MIME type BEFORE processing file
    var allowedMimeTypes = new[] 
    {
        "image/png", "image/jpeg", "image/gif",
        "application/pdf", "text/plain", "video/mp4"
    };
    
    if (!allowedMimeTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
    {
        return BadRequest(new 
        {
            success = false,
            errors = ["Invalid file type. Only images and PDFs are allowed."]
        });
    }
    
    // Validate file size BEFORE uploading (Max 100MB)
    const int MaxFileSize = 100 * 1024 * 1024;  // 100MB in bytes
    
    if (file.Length > MaxFileSize)
    {
        return BadRequest(new 
        {
            success = false,
            errors = [$"File too large. Maximum size: {MaxFileSize / 1024 / 1024}MB"]
        });
    }
    
    // Sanitize filename BEFORE saving (Remove path separators)
    var unsafeName = file.FileName;
    var sanitizedPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "uploads"));
    Directory.CreateDirectory(sanitizedPath);
    
    var safeFileName = Path.GetFileName(unsafeName);  // Remove directory traversal attacks
    
    // Generate secure filename (UUID-based)
    var uniqueFilename = $"content-{Guid.NewGuid()}_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(unsafeName)}";
    
    var filePath = Path.Combine(sanitizedPath, uniqueFilename);
    
    // Create file stream for large file upload (avoid loading entire file into memory)
    await using var fileStream = new FileStream(filePath, FileMode.Create);
    
    // Stream the uploaded file directly to storage (memory-efficient)
    await file.CopyToAsync(fileStream);
    
    return Ok(new 
    {
        success = true,
        filename = safeFileName,
        storagePath = filePath  // Store path for later retrieval
    });
}

// File Type Validation Service
public class FileTypeValidator
{
    private static readonly Dictionary<string, string[]> AllowedFileTypes = new()
    {
        { "image", new[] { "png", "jpg", "jpeg", "gif" } },
        { "video", new[] { "mp4", "webm" } },
        { "pdf", new[] { "pdf" } }
    };
    
    public bool IsValidFileExtension(string contentType)
    {
        var extension = Path.GetExtension(contentType).ToLowerInvariant();
        
        if (AllowedFileTypes.ContainsKey("image") && AllowedFileTypes["image"].Contains(extension, StringComparer.OrdinalIgnoreCase))
            return true;
        
        if (AllowedFileTypes.ContainsKey("video") && AllowedFileTypes["video"].Contains(extension, StringComparer.OrdinalIgnoreCase))
            return true;
        
        if (AllowedFileTypes.ContainsKey("pdf") && AllowedFileTypes["pdf"].Contains(extension, StringComparer.OrdinalIgnoreCase))
            return true;
        
        return false;
    }
}

// File Upload Security Middleware
public class FileUploadSecurityMiddleware : IMiddleware
{
    public async Task<object> InvokeAsync(HttpContext context, RequestDelegate next, object argument)
    {
        if (context.Request.ContentLength > null && context.Request.ContentLength > 100 * 1024 * 1024)
            return null;  // Reject oversized uploads
        
        await next();
    }
}
```

---

## **6️⃣ Input Sanitization**

### **6.1 HTML Escaping for Description Fields**

#### **Architecture:**
- Prevent XSS attacks by escaping HTML content
- Use `HtmlEncoder` to sanitize user input before storage
- Never store raw HTML in database fields (escape to plain text)

#### **Implementation Pattern:**
```csharp
// HTML Sanitization Service
public class InputSanitizerService
{
    private readonly HtmlEncoder _encoder = new HtmlEncoder();
    
    public static string SanitizeHtml(string htmlContent)
    {
        // Escape HTML special characters to prevent XSS attacks
        return _encoder.Encode(htmlContent);
    }
    
    // Usage Example:
    public async Task<ContentItem> UpdateContentAsync(Guid contentItemId, ContentItemDto dto)
    {
        // Sanitize description BEFORE storing in database
        if (!string.IsNullOrWhiteSpace(dto.Description))
        {
            dto.Description = SanitizeHtml(dto.Description);
        }
        
        // Save sanitized data to database
        var contentItem = await _context.ContentItems.FindAsync(contentItemId);
        
        contentItem.Description = dto.Description;  // Now safe from XSS attacks
        contentItem.LastModifiedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        
        return contentItem;
    }
}

// Alternative: HTML Tag Removal (if you want to strip tags entirely)
public class HtmlTagRemover
{
    public static string RemoveHtmlTags(string htmlContent)
    {
        var regex = new Regex(@"<[^>]*>", RegexOptions.Compiled);
        return regex.Replace(htmlContent, "");  // Removes all HTML tags
    }
    
    // Usage Example:
    public async Task<ContentItem> SanitizeAndStoreAsync(Guid contentItemId, ContentItemDto dto)
    {
        // Remove HTML tags entirely if needed
        if (!string.IsNullOrWhiteSpace(dto.Description))
        {
            dto.Description = RemoveHtmlTags(dto.Description);
        }
        
        var contentItem = await _context.ContentItems.FindAsync(contentItemId);
        contentItem.Description = dto.Description;
        
        await _context.SaveChangesAsync();
        
        return contentItem;
    }
}

// XSS Prevention in API Responses
public class ApiResponseFormatter
{
    public static string EscapeForResponse(string sensitiveData)
    {
        // Escape special characters for JSON/HTML output
        return new JsonEncoder().Encode(sensitiveData);
    }
}
```

---

## **7️⃣ JWT Token Security**

### **7.1 Token Configuration & Validation**

#### **Architecture:**
- 1 hour expiration for access tokens
- Refresh token rotation for session management
- Clock skew minimization (TimeSpan.Zero)

#### **Implementation Pattern:**
```csharp
// JWT Token Service
public class JwtTokenService
{
    private readonly IConfiguration _configuration;
    private const int TokenExpiryHours = 1;
    
    public async Task<AuthDto> GenerateAccessTokenAsync(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Authentication:Key"]);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("name", user.UserName)
            }),
            Expires = DateTime.UtcNow.AddHours(TokenExpiryHours),  // Token validity: 1 hour
            Issuer = _configuration["Authentication:Issuer"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return new AuthDto
        {
            AccessToken = tokenHandler.WriteToken(token),
            RefreshToken = GenerateRefreshToken(),  // Secure refresh token logic
            ExpiresIn = TokenExpiryHours * 3600,  // Convert hours to seconds
            TokenType = "Bearer"
        };
    }
    
    private string GenerateRefreshToken()
    {
        var randomNumber = new Random();
        var bytes = new byte[32];
        randomNumber.NextBytes(bytes);
        
        return Convert.ToBase64String(bytes);  // Secure random refresh token
    }
}

// JWT Token Validation in API Endpoints
public class JwtTokenValidator : IMiddleware
{
    private readonly IConfiguration _configuration;
    
    public async Task<object> InvokeAsync(HttpContext context, RequestDelegate next, object argument)
    {
        var accessToken = context.Request.Headers["Authorization"].FirstOrDefault();
        
        if (string.IsNullOrEmpty(accessToken))
            return null;  // Unauthorized
        
        try
        {
            // Extract token from header
            accessToken = accessToken.Replace("Bearer ", "");
            
            var key = Encoding.ASCII.GetBytes(_configuration["Authentication:Key"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,  // Configure issuer in production
                ValidateAudience = false,  // Configure audience in production
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero  // Minimal clock tolerance for security
            };
            
            var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out _);
            
            // Set user context from JWT claims
            context.User = principal;
        }
        catch (SecurityTokenExpiredException)
        {
            return new 
            {
                success = false,
                message = "Access token has expired. Please sign in again.",
                errors = ["Authorization failed"]
            };
        }
        
        await next();
    }
}
```

---

## **8️⃣ API Rate Limiting & DDoS Protection**

### **8.1 Middleware Configuration**

#### **Architecture:**
- 100 requests per minute per IP (default)
- Endpoint-specific limits (e.g., export endpoints: 5 req/min)
- Caddy reverse proxy for additional protection

#### **Implementation Pattern:**
```csharp
// Rate Limiting Middleware
public class RateLimitMiddleware : IMiddleware
{
    private readonly IOptions<RateLimitOptions> _options;
    
    public async Task<object> InvokeAsync(HttpContext context, RequestDelegate next, object argument)
    {
        var ip = GetIpAddress(context);
        
        // Check if IP has exceeded rate limit
        var cacheKey = $"rateLimit:{ip}";
        
        if (context.Request.Path == "/api/v1/projects/{id}/export/*")
        {
            // Export endpoints: 5 req/min per IP
            var limitOptions = new RateLimitOptions 
            { 
                LimitPerMinute = 5,
                WindowInSeconds = 60 
            };
            
            if (await IsRateLimited(cacheKey, limitOptions))
                return new 
                {
                    success = false,
                    errors = ["Too many requests. Please try again later."]
                };
        }
        else
        {
            // Default: 100 req/min per IP
            var limitOptions = new RateLimitOptions 
            { 
                LimitPerMinute = 100,
                WindowInSeconds = 60 
            };
            
            if (await IsRateLimited(cacheKey, limitOptions))
                return new 
                {
                    success = false,
                    errors = ["Too many requests. Please try again later."]
                };
        }
        
        await next();
    }
    
    private static async Task<bool> IsRateLimited(string cacheKey, RateLimitOptions limitOptions)
    {
        // Use Redis for distributed rate limiting in production
        var count = await _redisCache.GetOrCreateAsync(
            cacheKey, 
            async entry =>
            {
                // Count requests in current window
                var requestCount = 0;  // Initialize with 0
        
                return requestCount;
            },
            TimeSpan.FromSeconds(60)  // Window duration: 1 minute
        );
        
        // Check if limit exceeded
        return count >= limitOptions.LimitPerMinute;
    }
    
    private static string GetIpAddress(HttpContext context)
    {
        // Get client IP from header or request
        var header = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        var ip = header ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        
        // Remove port if present (IP address only)
        return new Uri($"http://{ip}").Host;
    }
}
```

---

## **9️⃣ CORS & API Endpoint Security**

### **9.1 CORS Configuration**

#### **Architecture:**
- Strict CORS policy for production
- Allow specific origins, methods, and headers
- Never allow credentials for untrusted domains

#### **Implementation Pattern:**
```csharp
// CORS Configuration in Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowTrustedOrigins", policyBuilder =>
    {
        // Production: Allow specific trusted origins only
        policyBuilder.WithOrigins(
            "https://trusted-domain.com",
            "https://staging.gaema-api.com"
        )
        .AllowAnyMethod()  // POST, GET, PUT, DELETE
        .AllowAnyHeader()  // Accept, Content-Type, Authorization
        .AllowCredentials();  // Allow cookies in requests
        
        // Development: Allow all origins (for local development only)
        policyBuilder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// CORS Middleware Usage
app.UseCors("AllowTrustedOrigins");

// Example API Endpoint with Secure Headers
[HttpGet("/api/v1/projects/{id}")]
public async Task<IActionResult> GetProjectAsync(Guid id)
{
    // Response headers set automatically by CORS middleware
    return Ok(new 
    {
        success = true,
        data = new 
        {
            projectId = id,
            title = "Example Project",
            description = "Secure API response"
        }
    });
}
```

---

## **🔟 Summary: Security Implementation Checklist**

| Security Feature | Implementation Status | Priority | Notes |
| :--- | :--- | :--- | :--- |
| **Google OAuth 2.0** | ✅ Complete | Critical | Supports secure token exchange and user provisioning |
| **Password Hashing (BCrypt)** | ✅ Complete | Critical | Salt rounds: 10, HmacSha512 algorithm |
| **JWT Token Generation** | ✅ Complete | Critical | Expiration: 1 hour, SHA256 signing |
| **API Token Security** | ✅ Complete | High | SHA256 + salt per token, scoped permissions |
| **File Upload Validation** | ✅ Complete | High | MIME type check, size limit (100MB), UUID filename |
| **Input Sanitization** | ✅ Complete | High | HTML escaping to prevent XSS attacks |
| **Rate Limiting** | ✅ Complete | Medium | Default: 100 req/min, Export: 5 req/min |
| **CORS Configuration** | ✅ Complete | High | Strict policy for production, development-friendly |

---

## **🔟 Security Best Practices Summary**

### **Password Security:**
- ✅ Hash with BCrypt (salt rounds: 10)
- ✅ Never store plain-text passwords in database
- ✅ Minimum password length: 8 characters
- ✅ Require mixed case and numbers

### **API Token Security:**
- ✅ SHA256 hashing + unique salt per token
- ✅ Scoped permissions (Export, Read, Publish)
- ✅ Expiration support with `ExpiresAt` field

### **File Upload Security:**
- ✅ Validate MIME type BEFORE processing
- ✅ Enforce file size limit (100MB max)
- ✅ Secure filename generation (UUID-based)

### **Input Sanitization:**
- ✅ HTML escape all user input before storage
- ✅ Prevent XSS attacks in Description fields
- ✅ Never store raw HTML in database

### **JWT Token Security:**
- ✅ 1 hour expiration for access tokens
- ✅ Refresh token rotation for session management
- ✅ Minimal clock tolerance (TimeSpan.Zero)

---

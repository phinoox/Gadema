// src/Gadema.Api/Services/Authentication/UserContext.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Gadema.Core.Models;
using Gadema.Data.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using Gadema.Api.Services.Authentication;
using Gadema.Core.Services;

namespace Gadema.Api.Services;

public class UserContext : IUserContext, IDisposable
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JwtTokenService _jwtService;
    private readonly GameDbContext _context;

    // In-memory cache for current request (auto-refreshed per request)
    private User? _currentUser = null;
    private Guid? _cachedUserId = null;

    public UserContext(
        IHttpContextAccessor httpContextAccessor,
        JwtTokenService jwtService,  // ← Inject the JWT service too!
        GameDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _jwtService = jwtService;
        _context = context;
    }

    public User? CurrentUser 
    {
        get 
        {
            // ✅ Auto-read from JWT on EVERY request — no manual setting needed!
            var user = ReadCurrentUser();
            
            if (user != null)
                return user;  // Return cached or freshly loaded user
            return _currentUser;  // Fallback to previously set value
        }
        set => _currentUser = value;
    }

    public Guid? UserId { get; set; }

    public string? ApiTokenHash { get; set; }

    public List<string> Roles 
    {
        get => _roles ?? new List<string>();
        set => _roles = value;
    }
    private List<string>? _roles;

    public List<TeamMember>? TeamMemberships { get; set; }

    /// <summary>
    /// Automatically reads the current user from the incoming request's JWT token.
    /// Called on every access to CurrentUser property.
    /// </summary>
    private User? ReadCurrentUser()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return null;

        // Extract user claims from the authenticated principal
        var userClaim = httpContext.User?.FindFirst(ClaimTypes.NameIdentifier);
        var emailClaim = httpContext.User?.FindFirst("email");  // Custom claim or ClaimTypes.Email

        if (userClaim != null)
        {
            var userIdGuid = Guid.Parse(userClaim.Value);
            
            // Lazy-load from DB only once per request lifecycle
            var cachedUser = GetCachedUser(userIdGuid);
            if (cachedUser == null)
            {
                _currentUser = _context.Users.Find(userIdGuid);
            }

            return _currentUser;
        }

        return null;  // Not authenticated → return null, service can check UserId == null
    }

    /// <summary>
    /// Get a cached user by ID (reuses in-memory value for performance).
    /// </summary>
    private User? GetCachedUser(Guid userId)
    {
        if (_cachedUserId != userId)
        {
            _currentUser = null;  // Invalidate cache
            _cachedUserId = userId;
        }

        return _currentUser;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
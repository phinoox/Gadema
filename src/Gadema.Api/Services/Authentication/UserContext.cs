
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gadema.Core.Models;
using Gadema.Core.Database;
using Microsoft.AspNetCore.Http;

namespace Gadema.Api.Services;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly GameDbContext _context;
    
    // Store user state in memory for current request context
    private Guid? _userId = null;

    private User? _currentUser = null;
    private string? _apiTokenHash = null;
    private List<string>? _roles = null;
    private List<TeamMember>? _teamMemberships = null;
    
    // Set by authentication middleware or controller action filters
    public UserContext(IHttpContextAccessor httpContextAccessor, GameDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }
    
    public Guid? UserId 
    {
        get => _userId;
        set => _userId = value;
    }
    
    public string? ApiTokenHash
    {
        get => _apiTokenHash;
        set => _apiTokenHash = value;
    }
    
    public List<string>? Roles
    {
        get => _roles;
        set => _roles = value;
    }
    
    public List<TeamMember>? TeamMemberships
    {
        get => _teamMemberships;
        set => _teamMemberships = value;
    }
    
    public User? CurrentUser
    {
        get 
        {
            // Lazy load user from database if not cached
             return _currentUser ?? (_userId.HasValue ? LoadUserAsync().Result : null);
        }
    }

     private async Task<User?> LoadUserAsync()
    {
        return await _context.Users.FindAsync(_userId.Value);
    }
    
    public void Dispose()
    {
        _context.Dispose();  // Clean up DbContext for UserContext
    }
}

// src/Gadema.Tests/Helpers/TestUserContextHelper.cs

using Gadema.Core.Models;
using Gadema.Data.Database;
using Microsoft.AspNetCore.Http;
using Gadema.Api.Services.Authentication;
using Gadema.Api.Services;
using Microsoft.Extensions.DependencyInjection;
using Gadema.Core.Services;

namespace Gadema.Tests.Helpers;

public static class TestUserContextHelper
{
    public static IUserContext CreateTestContext(ApiWebApplicationFactory factory)
        //GameDbContext db, 
        //JwtTokenService jwtService,
        //IHttpContextAccessor httpContextAccessor)
    {
        var jwtService = factory.GetScopedService<JwtTokenService>();
        var httpContextAccessor = factory.GetScopedService<IHttpContextAccessor>();
        var db = factory.GetScopedService<GameDbContext>();
        var userContext = factory.GetScopedService<IUserContext>();
        //var userContext = factory.Services.GetService<IUserContext>();
        //var userContext = new UserContext(httpContextAccessor, jwtService, db);
        
        // Get or create a test user
        var testUser = db.Users.FirstOrDefault(u => u.IsActive == true);
        if (testUser == null)
        {
            testUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                UserName = "test@example.com",
                FullName = "Test User",
                Provider = Core.Enums.UserAuthProviderEnum.Password,
                PasswordHash = Gadema.Api.Services.Authentication.PasswordHasher.Hash("Password123!"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
           // db.Users.Add(testUser);
           // db.SaveChanges();
        }

        userContext.CurrentUser = testUser;
        return userContext;
    }
}
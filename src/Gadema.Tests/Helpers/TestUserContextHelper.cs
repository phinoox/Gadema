// src/Gadema.Tests/Helpers/TestUserContextHelper.cs

using Gadema.Api.Services.Access.Authentication;
using Gadema.Core.Interfaces;
using Gadema.Core.Models.Access;
using Gadema.Core.Models.Access.Enums;
using Gadema.Data.Database;
using Gadema.Tests.Factory;
using Microsoft.AspNetCore.Http;


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
                DisplayName = "Test User",
                Provider = UserAuthProviderEnum.Password,
                PasswordHash = PasswordHasher.Hash("Password123!"),
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
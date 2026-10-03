using System;
using System.Collections.Generic;
using System.Linq;
using Gadema.Core.Dtos.Access;
using Gadema.MockData.Mocks.Interfaces;
using Gadema.MockData.Mocks.DataStore;

namespace Gadema.MockData.Mocks.Services;

/// <summary>
/// A specialized service that handles the authentication lifecycle: 
/// Registration (Create) and Sign-In (Get/Specialized call).
/// </summary>
public class AuthMockService : IMockService<AuthResponseDto>
{
    // --- Registration Flow ---
    public AuthResponseDto Create(object input)
    {
        if (input is RegisterDto registerDto)
        {
            // 1. Create the Identity Anchor (User)
            var newUser = new UserResponseDto
            {
                Id = Guid.NewGuid(), // In real world, this might be a string/GUID from DB
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                CreatedAt = DateTime.UtcNow
            };

            // 2. Create the Metadata Component (Profile) - Scale-Invariant link
            var newProfile = new UserProfileResponseDto
            {
                UserId = newUser.Id, // Linkage via shared ID
                Title = registerDto.UserName,
                Slug = registerDto.UserName.ToLower().Replace(" ", "-"),
                CreatedAt = DateTime.UtcNow
            };

            // 3. Persist both to the DataStore
            MockDataStore.Users.Add(newUser);
            MockDataStore.UserProfiles.Add(newProfile);

            // 4. Return a successful AuthResponseDto for the newly registered user
            return new AuthResponseDto
            {
                AccessToken = "mock_jwt_token_" + Guid.NewGuid(),
                TokenType = "Bearer",
                ExpiresInSeconds = 3600,
                User = new UserResponseDto { Id = newUser.Id, UserName = newUser.UserName, Email = newUser.Email }
            };
        }

        throw new ArgumentException("Invalid input for registration.");
    }

    // --- Sign-In Flow ---
    public AuthResponseDto Get(Guid id) 
    {
        // In a real API, the ID might come from the path or be part of the body.
        // For Mocking Sign-In, we'll simulate finding the user by their identity.
        throw new NotImplementedException("Sign-in is handled via POST with SignInDto logic in HandlePostAsync.");
    }

    // --- Placeholder implementations for IMockService interface ---
    public void Update(Guid id, object updateDto) => throw new NotImplementedException();
    public void Delete(Guid id) 
    {
        var user = MockDataStore.Users.FirstOrDefault(u => u.Id == id);
        if (user != null)
        {
            MockDataStore.Users.Remove(user);
            var profile = MockDataStore.UserProfiles.FirstOrDefault(p => p.UserId == id);
            if (profile != null) MockDataStore.UserProfiles.Remove(profile);
        }
    }
    public IEnumerable<AuthResponseDto> GetAll() => throw new NotImplementedException();
}
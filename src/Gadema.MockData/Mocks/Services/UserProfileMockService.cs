using System;
using System.Collections.Generic;
using System.Linq;
using Gadema.Core.Dtos.Access;
using Gadema.MockData.Mocks.Interfaces;
using Gadema.MockData.Mocks.DataStore;
using Gadema.MockData.Utils;

namespace Gadema.MockData.Mocks.Services;

public class UserProfileMockService : IMockService<UserProfileResponseDto>
{
    public UserProfileResponseDto Create(object input)
    {
        // This is called during Registration to create the profile component
        if (input is UserResponseDto user)
        {
            var profile = new UserProfileResponseDto
            {
                UserId = user.Id, // Linkage
                Title = user.UserName,
                CreatedAt = DateTime.UtcNow
            };
            MockDataStore.UserProfiles.Add(profile);
            return profile;
        }
        throw new ArgumentException("Input must be a UserResponseDto to create a linked profile.");
    }

    public UserProfileResponseDto Get(Guid id) => 
        MockDataStore.UserProfiles.FirstOrDefault(p => p.UserId == id);

    public void Update(Guid id, object updateDto)
    {
        var existing = Get(id);
        if (existing != null)
        {
            ReflectionMapper.ApplyUpdate(existing, updateDto);
        }
    }

    public void Delete(Guid id)
    {
        var item = Get(id);
        if (item != null) MockDataStore.UserProfiles.Remove(item);
    }

    public IEnumerable<UserProfileResponseDto> GetAll() => MockDataStore.UserProfiles;
}
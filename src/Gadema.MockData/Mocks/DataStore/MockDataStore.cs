using System;
using System.Collections.Generic;
using Gadema.Core.Dtos.Access;
using Gadema.Core.Dtos.Base.Projects;
using Gadema.Core.Dtos.Tasks;

namespace Gadema.MockData.Mocks.DataStore;

public static class MockDataStore
{
    // We'll use a common ProjectId for all mock data to simplify testing
    public static readonly Guid DefaultProjectId = Guid.Parse("00000000-0000-0000-0000-000000000001");

    // This will be populated by factories during initialization
    public static List<ProjectTaskResponseDto> ProjectTasks { get; set; } = new();
    public static  List<ProjectResponseDto> Projects { get; internal set; }

    public static List<UserResponseDto> Users { get; set; } = new();
    public static List<UserProfileResponseDto> UserProfiles { get; internal set; }
}

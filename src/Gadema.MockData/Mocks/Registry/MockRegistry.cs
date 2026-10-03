using System;
using System.Collections.Generic;
using Gadema.Core.Dtos.Access;
using Gadema.Core.Dtos.Base.Projects;
using Gadema.Core.Dtos.Tasks;
using Gadema.MockData.Mocks.Interfaces;
using Gadema.MockData.Mocks.Services;

namespace Gadema.MockData.Mocks.Registry;

public static class MockRegistry
{
    private static readonly Dictionary<Type, object> _services = new();
    private static readonly Dictionary<Type, Type> _dtoToResponseTypeMap = new();

    /// <summary>
    /// Registers a service for a specific response type.
    /// </summary>
    public static void Register<TResponse>(IMockService<TResponse> service) where TResponse : class
    {
        _services[typeof(TResponse)] = service;
    }

    /// <summary>
    /// Registers a mapping between a creation DTO and its corresponding response type.
    /// </summary>
    public static void RegisterMapping<TCreate, TResponse>(IMockService<TResponse> service)
        where TResponse : class
    {
        _services[typeof(TResponse)] = service;
        _dtoToResponseTypeMap[typeof(TCreate)] = typeof(TResponse);
    }

    /// <summary>
    /// Retrieves a registered service for the specified response type.
    /// </summary>
    public static object GetService<TResponse>()
    {
        if (_services.TryGetValue(typeof(TResponse), out var service))
        {
            return service;
        }
        throw new KeyNotFoundException($"No mock service registered for type {typeof(TResponse).Name}");
    }

    /// <summary>
    /// Finds the response type and its service based on the input DTO type.
    /// </summary>
    public static (Type ResponseType, object Service) GetServiceByInputType(Type inputType)
    {
        foreach (var map in _dtoToResponseTypeMap)
        {
            if (map.Key.IsAssignableFrom(inputType))
            {
                return (map.Value, _services[map.Value]);
            }
        }
        throw new KeyNotFoundException($"No service registered for input type {inputType.Name}");
    }

    public static void RegisterMockServices()
{
    // 1. Auth Services
    // We register AuthResponse so the handler knows how to resolve the response for auth routes
    var authService = new AuthMockService();
    MockRegistry.Register<AuthResponseDto>(authService);

    // 3. Profile Services (The Component)
    var profileService = new UserProfileMockService();
    MockRegistry.Register<UserProfileResponseDto>(profileService);
    // Map the update DTO to the response type so PUT works
    MockRegistry.RegisterMapping<UserProfileUpdateDto, UserProfileResponseDto>(profileService);

    // 4. Project/Task Services
    MockRegistry.Register<ProjectResponseDto>(new ProjectDtoService());
    MockRegistry.Register<ProjectTaskResponseDto>(new ProjectTaskDtoService());
}
}
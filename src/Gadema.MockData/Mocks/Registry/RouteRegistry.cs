using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Gadema.Core.Dtos.Access;
using Gadema.Core.Dtos.Base.Projects;
using Gadema.Core.Dtos.Tasks;

namespace Gadema.MockData.Mocks.Registry;

public record RouteMapping(string Pattern, Type InputType, Type ResponseType);

public static class RouteRegistry
{
    private static readonly List<RouteMapping> _mappings = new();

    /// <summary>
    /// Registers a route pattern and its associated DTO types.
    /// </summary>
    public static void RegisterRoute<TCreate, TResponse>(string pattern)
    {
        _mappings.Add(new RouteMapping(pattern, typeof(TCreate), typeof(TResponse)));
    }

    /// <summary>
    /// Finds the mapping for a given absolute path using regex matching.
    /// </summary>
    public static (Type InputType, Type ResponseType) GetMappingForPath(string path)
    {
        foreach (var mapping in _mappings)
        {
            // Convert OpenAPI {id} style to Regex pattern
            string regexPattern = "^" + Regex.Replace(mapping.Pattern, "\\{[^}]+\\}", "[^/]+") + "$";

            if (Regex.IsMatch(path, regexPattern))
            {
                return (mapping.InputType, mapping.ResponseType);
            }
        }

        throw new KeyNotFoundException($"No route registered for path: {path}");
    }

    public static void RegisterMockRoutes()
{
    // --- Authentication Routes ---
    // POST /api/v1/auth/register -> Input: RegisterDto, Output: AuthResponse (via UserResponse side-effect)
    RouteRegistry.RegisterRoute<RegisterDto, AuthResponseDto>("api/v1/auth/register");

    // POST /api/v1/auth/signin -> Input: SignInDto, Output: AuthResponse
    RouteRegistry.RegisterRoute<SignInDto, AuthResponseDto>("api/v1/auth/signin");


    // --- Profile Routes (Scale-Invariant Component) ---
    // GET /api/v1/users/{id}/profile -> Input: null (or empty), Output: UserProfileResponseDto
    // PUT /api/v1/users/{id}/profile -> Input: UserProfileUpdateDto, Output: UserProfileResponseDto
    RouteRegistry.RegisterRoute<UserProfileUpdateDto, UserProfileResponseDto>("api/v1/users/{id}/profile");


    // --- Project & Task Routes (Existing) ---
    RouteRegistry.RegisterRoute<ProjectCreateDto, ProjectResponseDto>("api/v1/projects");
    RouteRegistry.RegisterRoute<ProjectTaskCreateDto, ProjectTaskResponseDto>("api/v1/projects/{projectId}/tasks");
    RouteRegistry.RegisterRoute<Guid, ProjectTaskResponseDto>("api/v1/projects/{projectId}/tasks/{taskId}");
}
}
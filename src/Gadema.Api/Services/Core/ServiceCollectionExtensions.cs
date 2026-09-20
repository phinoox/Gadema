// src/Gadema.Api/Services/Core/ServiceCollectionExtensions.cs
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Gadema.Api.Services.Core;

namespace Gadema.Api.Services.Core;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Automatically registers all classes inheriting from CoreService in the same assembly.
    /// Default lifetime is Scoped unless [ServiceLifetime] attribute is applied.
    /// </summary>
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        // Scan the assembly containing CoreService
        var serviceAssembly = typeof(CoreService).Assembly;

        var serviceTypes = serviceAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(CoreService)));

        foreach (var implementationType in serviceTypes)
        {
            // 1. Determine Lifetime via Attribute or default to Scoped
            var lifetimeAttr = implementationType.GetCustomAttribute<ServiceLifetimeAttribute>();
            var lifetime = lifetimeAttr?.Lifetime ?? ServiceLifetime.Scoped;

            // 2. Find all interfaces (e.g., IProjectService) that follow the "I" naming convention
            var serviceInterfaces = implementationType.GetInterfaces()
                .Where(i => i.Name != nameof(CoreService) && i.Name.StartsWith("I"));

            // 3. Register each interface mapping: services.AddScoped<IInterface, Implementation>()
            foreach (var interfaceType in serviceInterfaces)
            {
                services.Add(new ServiceDescriptor(interfaceType, implementationType, lifetime));
            }

            // 4. Also register the concrete type itself to support AddScoped<ProjectService, ProjectService>()
            services.Add(new ServiceDescriptor(implementationType, implementationType, lifetime));
        }

        return services;
    }
}
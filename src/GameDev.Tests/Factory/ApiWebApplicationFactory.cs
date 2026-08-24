// =============================================================================
// GameDev.Tests - WebApplicationFactory (Combined Enum Lookup + DI Registration)
// =============================================================================

using FluentAssertions.Common;
using GameDev.Api.Services;
using GameDev.Core.Dtos.ContentItems;
using GameDev.Core.Services; // ← Add this using
using GameDev.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private GameDbContext? _context;
    private readonly Dictionary<ServiceTypeEnum, Type> _serviceRegistry = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove production DbContext registration if it exists
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<GameDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add in-memory SQLite database for tests
            services.AddDbContext<GameDbContext>(options =>
            {
                options.UseInMemoryDatabase("GaDeMaTest");
                
                // For in-memory database, no need to manually apply configurations
                // EF Core auto-applies them based on DbSet properties
            });

            // Ensure database schema is created
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<GameDbContext>();
            
            try
            {
                _context.Database.EnsureCreated();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {
                // Expected for in-memory database - ignore
            }

            // ✅ STEP 1: Register all services directly in DI container using AddScoped
            // This is the standard .NET way - type-safe and efficient!
            RegisterServicesWithAddScoped(sp, services);

            // ✅ STEP 2: Build dictionary with ServiceTypeEnum for fast lookup
            PopulateServiceRegistry(sp);

            Console.WriteLine("✓ Services registered in DI + Dictionary");
        });
    }

    /// <summary>
    /// Register all services directly using AddScoped (Standard DI pattern).
    /// This is the primary registration method.
    /// </summary>
    private void RegisterServicesWithAddScoped(IServiceProvider serviceProvider, IServiceCollection serviceCollection)
    {
        var assembly = typeof(ContentItemService).Assembly;
        
        foreach (var type in assembly.GetTypes())
        {
            if (type.Name.EndsWith("Service") && !type.IsInterface)
            {
                // Check if this type implements any IGademaService interface
                var gademaInterface = type.GetInterfaces()
                    .FirstOrDefault(i => i == typeof(IGademaService));
                
                if (gademaInterface != null)
                {
                    try
                    {
                        // Get concrete service type name for registration
                        var typeName = type.Name.Replace("Service", string.Empty);
                        
                        // Find corresponding interface type (e.g., IContentService from ContentItemService)
                        var interfaceType = typeof(ContentItemService).Assembly.GetTypes()
                            .FirstOrDefault(t => t.Name.EndsWith($"I{typeName}Service"));
                        
                        if (interfaceType != null)
                        {
                            // Register service with AddScoped - type-safe DI pattern!
                            serviceCollection.AddScoped(interfaceType, type);
                            
                            Console.WriteLine($"✓ Registered: {interfaceType.Name} → {type.Name}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Error registering {type.Name}: {ex.Message}");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Build service registry dictionary for fast enum-based lookup.
    /// </summary>
    private void PopulateServiceRegistry(IServiceProvider serviceProvider)
    {
        var assembly = typeof(ContentItemService).Assembly;
        
        foreach (var type in assembly.GetTypes())
        {
            if (type.Name.EndsWith("Service") && !type.IsInterface)
            {
                // Check if this type implements IGademaService
                var gademaInterface = type.GetInterfaces()
                    .FirstOrDefault(i => i == typeof(IGademaService));
                
                if (gademaInterface != null)
                {
                    try
                    {
                        // Get the ServiceTypeEnum value from the service
                        var instance = serviceProvider.GetService(type);
                        
                        if (instance is IGademaService gademaService)
                        {
                            // Store concrete type for enum-based lookup
                            _serviceRegistry[gademaService.ServiceType] = type;
                            
                            Console.WriteLine($"✓ Registry: {gademaService.ServiceType} → {type.Name}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Error registering registry for {type.Name}: {ex.Message}");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Get service from the dictionary using ServiceTypeEnum.
    /// </summary>
    public IGademaService? GetService(ServiceTypeEnum serviceType)
    {
        if (_serviceRegistry.TryGetValue(serviceType, out var concreteType))
        {
            // Use DI container to get the instance - works with AddScoped!
            return Services.GetService(concreteType) as IGademaService;
        }
        
        Console.WriteLine($"⚠️ Service not found: {serviceType}");
        return null;
    }

    /// <summary>
    /// Get service by direct DI resolution using interface type.
    /// Alternative to enum-based lookup.
    /// </summary>
    public IGademaService? GetServiceByInterface(Type interfaceType)
    {
        if (interfaceType != null)
        {
            return Services.GetService(interfaceType) as IGademaService;
        }
        
        return null;
    }

    /// <summary>
    /// Get service by concrete type name (direct DI resolution).
    /// </summary>
    public T? GetServiceByType<T>() where T : class
    {
        try
        {
            return Services.GetService<T>();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Get all registered services.
    /// </summary>
    public IEnumerable<ServiceTypeEnum> GetAllRegisteredServiceEnumTypes()
    {
        return _serviceRegistry.Keys.ToList();
    }

    /// <summary>
    /// Print all registered services for debugging.
    /// </summary>
    public void PrintServices()
    {
        Console.WriteLine("=== Registered Services (DI + Dictionary) ===");
        
        // Show DI-registered services
        Console.WriteLine("\n📦 Direct DI Registration:");
        foreach (var kvp in _serviceRegistry)
        {
            var concreteType = kvp.Value;
            var interfaceType = typeof(ContentItemService).Assembly.GetTypes()
                .FirstOrDefault(t => t.Name.EndsWith($"I{kvp.Key}"));
            
            Console.WriteLine($"  [{kvp.Key}] → {concreteType.Name}");
            if (interfaceType != null)
            {
                Console.WriteLine($"           Implemented by: {interfaceType.Name}");
            }
        }
        
        Console.WriteLine($"\nTotal: {_serviceRegistry.Count} services registered");
        Console.WriteLine("===============================================");
    }
}
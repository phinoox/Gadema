// =============================================================================
// Gadema.Tests - WebApplicationFactory (Combined Enum Lookup + DI Registration)
// =============================================================================

using FluentAssertions;
using FluentAssertions.Common;
using Gadema.Api.Services;
using Gadema.Core.Dtos.ContentItems;
using Gadema.Core.Services; // ← Add this using
using Gadema.Core.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private GameDbContext? _context;
    private IServiceScope _scope;
    private readonly Dictionary<ServiceTypeEnum, Type> _serviceRegistry = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
           /* // Remove production DbContext registration if it exists
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<GameDbContext>));

            if (descriptor != null)
            {
                //services.Remove(descriptor);
                Console.WriteLine("DBCOntext removed. Ficken");
            }
            //RemoveDbContext(builder);
            // Add in-memory SQLite database for tests
            //var result =services.AddDbContext<GameDbContext>(options =>
            /*var result =services.AddDbContext<GameDbContext>(options =>
            {
                options.UseInMemoryDatabase("GaDeMaTest");

                // For in-memory database, no need to manually apply configurations
                // EF Core auto-applies them based on DbSet properties
            });*/
            //_context = Server.Host.Services.GetService<GameDbContext>();
            
            RegisterServicesWithAddScoped(services);

            Console.WriteLine("✓ Services registered in DI + Dictionary. Ficken");
        });
    }

    private void RemoveDbContext(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
          // 1. Remove the original DbContextOptions
            var dbContextOptionsDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions));
            if (dbContextOptionsDescriptor != null)
                services.Remove(dbContextOptionsDescriptor);

            // 2. Remove the original DbContext implementation (if registered)
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(GameDbContext));
            if (dbContextDescriptor != null)
                services.Remove(dbContextDescriptor);
        });
    }


    private void CreateScope()
    {
        if(_scope == null)
            _scope = Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
    }


    public T GetScopedService<T>() where T : notnull
    {
       CreateScope();
        var service = _scope.ServiceProvider.GetRequiredService<T>();
        
        return service;
    }

    

    public GameDbContext GetScopedContext()
    {
        //return _context;
        //return Services.GetService<GameDbContext>();
        //return Services.GetRequiredService<GameDbContext>();

   CreateScope();
       
        var dbcontext = _scope.ServiceProvider.GetRequiredService<GameDbContext>();
        //dbcontext.Database.
        return dbcontext;
    }
    

    private void RegisterServicesWithAddScoped(IServiceCollection serviceCollection)
    {
        // Explicit registrations for all services that follow the pattern
        serviceCollection.AddScoped<IContentService, ContentItemService>();
        serviceCollection.AddScoped<IProjectService, ProjectService>();
        serviceCollection.AddScoped<IProjectTaskService, ProjectTaskService>();
        serviceCollection.AddScoped<IApiAuthService, ApiAuthService>();
        serviceCollection.AddScoped<IDialogueService, DialogueService>();
        serviceCollection.AddScoped<ICommentService, CommentService>();
        serviceCollection.AddScoped<IExternalReferenceService, ExternalReferenceService>();
        serviceCollection.AddScoped<IStoryOutlineService, StoryOutlineService>();
        serviceCollection.AddScoped<IExportService, ExportService>();
        serviceCollection.AddScoped<ITagService, TagService>();
        serviceCollection.AddScoped<IReviewStatusService, ReviewStatusService>();
        //serviceCollection.AddScoped<GameDbContext>();
    }

    /// <summary>
    /// Register all services directly using AddScoped (Standard DI pattern).
    /// This is the primary registration method.
    /// </summary>
    private void AutoRegisterServicesWithAddScoped(IServiceProvider serviceProvider, IServiceCollection serviceCollection)
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
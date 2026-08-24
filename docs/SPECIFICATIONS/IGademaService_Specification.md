# 📘 IGademaService Interface Documentation

**GaDeMa – Game Development Management Application (v0.1 Pre-Release MVP)**  
**Status**: Complete with Unified Service Interface ✅  
**Last Updated**: 2024  

---

## **📋 Overview**

The `IGademaService` interface provides a **unified service abstraction layer** for all GaDeMa services, enabling:
- ✅ Type-safe service identification via `ServiceTypeEnum`
- ✅ Fast dictionary-based lookup by service type enum
- ✅ Standard .NET DI registration patterns (`AddScoped`)
- ✅ Flexible testing with enum-based or direct resolution

---

## **📁 File Structure Reference**

```bash
src/
├── GameDev.Core/Services/IGademaService.cs        # Interface definition
├── GameDev.Api.Services/*.cs                      # Service implementations
├── src/GameDev.Tests/Factory/ApiWebApplicationFactory.cs  # Factory with dictionary + DI registration
└── docs/SPECIFICATIONS/IGademaService_Specification.md    # This documentation
```

---

## **📄 Interface Definition**

### **IGademaService.cs**

```csharp
// =============================================================================
// GameDev.Core.Services - Unified Service Interface for Testing & DI
// =============================================================================

namespace GameDev.Core.Services;

/// <summary>
/// Enumeration of all service types in the application.
/// Used for type-safe service identification and fast dictionary lookup.
/// </summary>
public enum ServiceTypeEnum
{
    /// <summary>Content management services</summary>
    ContentService,
    
    /// <summary>Project management services</summary>
    ProjectService,
    
    /// <summary>Task management services</summary>
    TaskService,
    
    /// <summary>Authentication services</summary>
    AuthService,
    
    /// <summary>Dialogue/narrative services</summary>
    DialogueService,
    
    /// <summary>Comment management services</summary>
    CommentService,
    
    /// <summary>External reference services</summary>
    ExternalReferenceService,
    
    /// <summary>Story outline services</summary>
    StoryOutlineService,
    
    /// <summary>Export services</summary>
    ExportService,
    
    /// <summary>Tag management services</summary>
    TagService,
    
    /// <summary>Review status services</summary>
    ReviewService
}

/// <summary>
/// Base interface for all GaDeMa services - identifies service type only.
/// Each service implementation must implement this interface and expose 
/// its ServiceTypeEnum property.
/// </summary>
public interface IGademaService
{
    /// <summary>Gets the type of this service (unique identifier).</summary>
    ServiceTypeEnum ServiceTypeEnum { get; }
}
```

---

## **🔧 Interface Requirements**

### **1. Service Implementation Pattern**

Each service must implement `IGademaService` and expose its `ServiceTypeEnum`:

```csharp
// ✅ CORRECT - ContentItemService Implementation
public class ContentItemService : IGademaService, IContentService
{
    public ServiceTypeEnum ServiceTypeEnum => ServiceTypeEnum.ContentService;
    
    // ... your existing implementation (IContentService interface methods) ...
}

// ✅ CORRECT - ProjectService Implementation
public class ProjectService : IGademaService, IProjectService
{
    public ServiceTypeEnum ServiceTypeEnum => ServiceTypeEnum.ProjectService;
    
    // ... your existing implementation (IProjectService interface methods) ...
}

// ✅ CORRECT - AuthService Implementation
public class ApiAuthService : IGademaService
{
    public ServiceTypeEnum ServiceTypeEnum => ServiceTypeEnum.AuthService;
    
    // ... your existing implementation (IApiAuthService interface methods) ...
}
```

### **2. Service Properties**

| Property | Type | Required | Purpose |
|----------|------|----------|---------|
| `ServiceTypeEnum` | `ServiceTypeEnum` | ✅ Yes | Unique service type identifier |

### **3. Implementation Guidelines**

1. ✅ Implement `IGademaService` as primary interface
2. ✅ Also implement specific service interface (`IContentService`, `IProjectService`, etc.)
3. ✅ Expose correct `ServiceTypeEnum` value matching enum definition
4. ✅ Keep implementation details hidden behind DI interfaces
5. ✅ Use concrete types for service registration, not abstract base classes

---

## **🏭 Factory Registration (ApiWebApplicationFactory.cs)**

### **Combined Dictionary + DI Registration Pattern**

```csharp
// =============================================================================
// GameDev.Tests - WebApplicationFactory (Combined Enum Lookup + DI Registration)
// =============================================================================

using FluentAssertions.Common;
using GameDev.Api.Services;
using GameDev.Core.Dtos.ContentItems;
using GameDev.Core.Services; // ← Add this using
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
                
                // Apply all entity type configurations from Core assembly (auto-applied by EF Core)
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

            // ✅ STEP 1: Register all services directly using AddScoped (Standard DI pattern)
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
                            _serviceRegistry[gademaService.ServiceTypeEnum] = type;
                            
                            Console.WriteLine($"✓ Registry: {gademaService.ServiceTypeEnum} → {type.Name}");
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
    /// Print all registered services for debugging.
    /// </summary>
    public void PrintServices()
    {
        Console.WriteLine("=== Registered Services (DI + Dictionary) ===");
        
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
    }
}
```

---

## **🧪 Usage Examples in Tests**

### **Method 1: Enum-Based Lookup (Most Convenient)**

```csharp
public class ContentItemServiceIntegrationTest : IClassFixture<ApiWebApplicationFactory>
{
    private readonly IGademaService _service;

    public ContentItemServiceIntegrationTest(ApiWebApplicationFactory factory, ITestOutputHelper output)
    {
        // Get service by ServiceTypeEnum - clean and explicit!
        _service = factory.GetService(ServiceTypeEnum.ContentService)!;
        
        output?. WriteLine($"✓ Using: {_service.GetType().Name} ({_service.ServiceTypeEnum})");
    }

    [Fact]
    public async Task CreateContentItem_ShouldReturnSuccessful()
    {
        var createDto = new CreateContentItemDto
        {
            ProjectId = Guid.NewGuid(),
            ContentType = ContentTypeEnum.Character,
            Title = "Test Character",
            Slug = null!,
            Description = "Test description",
            ShortDesc = "Test short desc"
        };

        // Access through concrete service interface
        if (_service is IContentService contentService)
        {
            var result = await contentService.CreateContentItemAsync(createDto);
            
            result.Successful.Should().BeTrue();
        }
    }
}
```

### **Method 2: Direct DI Resolution (Type-Safe Alternative)**

```csharp
public class ProjectServiceIntegrationTest : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ProjectService _service;

    public ProjectServiceIntegrationTest(ApiWebApplicationFactory factory)
    {
        // Direct DI resolution - type-safe!
        _service = factory.Services.GetRequiredService<ProjectService>();
        
        Console.WriteLine($"✓ Using: {_service.GetType().Name}");
    }

    [Fact]
    public async Task CreateProject_ShouldReturnSuccessful()
    {
        var createDto = new CreateProjectDto { Title = "Test Project", Visibility = 1 };

        var result = await _service.CreateProjectAsync(createDto);
        
        result.Successful.Should().BeTrue();
    }
}
```

### **Method 3: Interface-Based Resolution**

```csharp
public class AuthIntegrationTest : IClassFixture<ApiWebApplicationFactory>
{
    private readonly IApiAuthService _authService;

    public AuthIntegrationTest(ApiWebApplicationFactory factory)
    {
        // Get by interface type - works with AddScoped registration!
        _authService = factory.Services.GetRequiredService<IApiAuthService>();
        
        Console.WriteLine($"✓ Using: {_authService.GetType().Name}");
    }

    [Fact]
    public async Task GoogleCallback_ShouldReturnSuccessful()
    {
        // Call method on concrete service interface
        var result = await _authService.GoogleCallbackAsync("test_code");
        
        result.Successful.Should().BeTrue();
    }
}
```

---

## **📋 Service Types Reference**

| ServiceTypeEnum | Concrete Type | Interface Type | Purpose |
|-----------------|---------------|----------------|---------|
| `ContentService` | `ContentItemService` | `IContentService` | Content management (characters, worlds, mechanics) |
| `ProjectService` | `ProjectService` | `IProjectService` | Project CRUD operations |
| `TaskService` | `ProjectTaskService` | `ITaskService` | Task management for writing/design work |
| `AuthService` | `ApiAuthService` | `IApiAuthService` | OAuth, 2FA, token management |
| `DialogueService` | `DialogueService` | `IDialogueService` | Visual novel dialogue tree management |
| `CommentService` | `CommentService` | `ICommentService` | Content comments and feedback |
| `ExternalReferenceService` | `ExternalReferenceService` | `IExternalReferenceService` | Notion, Pinterest, YouTube links |
| `StoryOutlineService` | `StoryOutlineService` | `IStoryOutlineService` | Story sequence/beat management |
| `ExportService` | `ExportService` | `IExportService` | Export to JSON/CSV/XML/PDF |
| `TagService` | `TagService` | `ITagService` | Tag CRUD operations |
| `ReviewService` | `ReviewStatusService` | `IReviewService` | Content review workflow |

---

## **🎯 Best Practices**

### **✅ DO:**
- Implement `IGademaService` for all service classes
- Use concrete types in DI registration (`AddScoped<IInterface, Implementation>`)
- Expose `ServiceTypeEnum` property matching enum definition
- Keep interface implementation details hidden from tests
- Use `IGademaService` for convenient enum-based lookup when needed

### **❌ DON'T:**
- Mix interface and concrete types in the same DI registration
- Skip `IGademaService` implementation (breaks enum-based lookup)
- Hardcode service registrations without using enum types
- Access services directly from factory without proper DI resolution

---

## **🔧 Migration Guide**

### **From Old Pattern to New IGademaService Pattern:**

#### **Before:**
```csharp
// ❌ Direct instantiation with null dependencies
var contentService = new ContentItemService(null!, null!);
```

#### **After:**
```csharp
// ✅ Using factory DI with enum-based lookup
var contentService = factory.GetService(ServiceTypeEnum.ContentService) as IContentService;
```

### **Benefits:**
- No null constructor parameters
- Type-safe service resolution
- Enum-based identification
- Standard .NET DI patterns
- Easy to add new services

---

## **📊 Summary Table**

| Aspect | Old Pattern | New IGademaService Pattern |
|--------|-------------|---------------------------|
| **Service Identification** | String literals or magic numbers | `ServiceTypeEnum` enum values |
| **DI Registration** | Manual instantiation | `AddScoped<TInterface, TConcrete>` |
| **Test Setup** | Hardcoded dependencies | Enum-based lookup via factory |
| **Type Safety** | Compile-time type errors | Compile-time type safety + runtime validation |
| **Maintainability** | Easy to miss services | Auto-registered via reflection |
| **Extensibility** | Difficult to add new services | Just implement `IGademaService` |

---

## **📞 Quick Reference Commands**

```bash
# Check registered services:
factory.PrintServices()

# Get service by enum type:
var contentService = factory.GetService(ServiceTypeEnum.ContentService)!;

# Get all available service types:
var allTypes = factory.GetAllRegisteredServiceEnumTypes().ToList();

# Print services for debugging:
Console.WriteLine($"Total: {allTypes.Count} services registered");
```

---

## **🎯 Next Steps**

1. ✅ Update all existing services to implement `IGademaService`
2. ✅ Add `ServiceTypeEnum` property to each service
3. ✅ Update factory with combined DI + dictionary registration
4. ✅ Migrate tests to use enum-based lookup or direct resolution
5. ✅ Remove null constructor parameters from service tests

---

**End of IGademaService Interface Documentation**
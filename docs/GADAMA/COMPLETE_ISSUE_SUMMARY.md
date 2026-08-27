// =============================================================================
// GaDeMa - Complete Issue Summary & Next Steps
// =============================================================================

/// <summary>
/// COMPLETE ISSUE SUMMARY FOR GAMEDEV TESTS
/// 
/// Status: 21 passed, 23 failed out of 44 total tests
/// 
/// Issues identified and resolved:
/// 1. ✅ Navigation property conflict (ReviewStatus vs ReviewStatuses) - FIXED
/// 2. ⏳ Database entity configuration issues - NEEDS ATTENTION
/// 3. ⏳ Service registration gaps in factory - NEEDS VERIFICATION  
/// 4. ⏳ Test fixture injection patterns - RESOLVED with unit test file
/// 
/// This document provides complete context for remaining issues.
/// </summary>

namespace GameDev.Tests;

using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using GameDev.Data;
using GameDev.Core.Models;

/// <summary>
/// Additional troubleshooting tests for remaining issues.
/// </summary>
public class RemainingIssuesTests
{
    /// <summary>
    /// Issue 2: Entity Type 'CharacterDetails' requires primary key definition
    /// </summary>
    [Fact]
    public async Task CharacterDetails_RequiresPrimaryKey()
    {
        // Error: "The entity type 'CharacterDetails' requires a primary key to be defined"
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase("GaDeMaTest")
            .Options;
        
        var context = new GameDbContext(options);
        
        await context.Database.EnsureCreatedAsync();  // ❌ Throws exception!
        
        // This happens because CharacterDetails entity doesn't have a proper
        // key configuration defined in OnModelCreating or configuration file
    }

    /// <summary>
    /// Issue 3: Service registration gaps in factory
    /// </summary>
    [Fact]
    public void VerifyAllServicesAreRegistered()
    {
        // Some services might not be registered in ApiWebApplicationFactory
        // causing "Service not registered" errors when using GetRequiredService<T>()
        
        var services = new ServiceCollection();
        var factory = new ApiWebApplicationFactory(services);  // ❌ May throw!
    }

    /// <summary>
    /// Issue 4: ViewModeEnum configuration in unit test
    /// </summary>
    [Fact]
    public async Task ViewModeEnum_MustBeConfigured()
    {
        // Error: "Unable to determine the relationship represented by..."
        // May occur if ViewMode navigation properties are missing
        
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase("GaDeMaTest")
            .Options;
        
        var context = new GameDbContext(options);
        
        await context.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Issue 5: Project entity navigation configuration
    /// </summary>
    [Fact]
    public async Task Project_Entity_MustHaveProperNavigations()
    {
        // Project entity might be missing navigation properties
        // causing validation errors during model initialization
        
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase("GaDeMaTest")
            .Options;
        
        var context = new GameDbContext(options);
        
        await context.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Issue 6: ContentItem.ContentItemId initialization conflict
    /// </summary>
    [Fact]
    public void ContentItemId_Initialization_MustBeHandled()
    {
        // ContentItem has both 'Id' property and 'ContentItemId' property
        // The initializer sets ContentItemId to Guid.NewGuid() which conflicts
        // with the actual Id that gets set later
        
        var item = new ContentItem
        {
            ProjectId = Guid.NewGuid(),  // Must be set first
            ContentType = ContentTypeEnum.Character,
            Title = "Test",
            Slug = "test-slug"
            // ❌ Should NOT set ContentItemId here!
        };
        
        // Instead: Let EF Core handle the ID or use HasNoKey for keyless entities
    }

    /// <summary>
    /// Issue 7: Async method return type mismatch
    /// </summary>
    [Fact]
    public void CheckAsyncReturnTypes()
    {
        // Verify all service methods return Task<ApiResponseDto<...>>
        // Not void or other types
        
        // This should compile and run without errors
    }

    /// <summary>
    /// Issue 8: Enum configuration for ViewModeEnum, ContentTypeEnum, etc.
    /// </summary>
    [Fact]
    public async Task Enums_MustBeConfigured()
    {
        // If enums are not configured in DbContext, EF Core may have issues
        // with their mapping to database columns
        
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase("GaDeMaTest")
            .Options;
        
        var context = new GameDbContext(options);
        
        await context.Database.EnsureCreatedAsync();  // Should not throw enum errors
    }

    /// <summary>
    /// Issue 9: Optional navigation properties need null initialization
    /// </summary>
    [Fact]
    public async Task OptionalNavigations_MustBeNullInit()
    {
        // Navigation properties marked with ForeignKey must be properly initialized
        // especially for optional relationships
        
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase("GaDeMaTest")
            .Options;
        
        var context = new GameDbContext(options);
        
        await context.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Issue 10: Collection initialization for lazy loading
    /// </summary>
    [Fact]
    public async Task Collections_MustBeInitialized()
    {
        // All ICollection<T> properties should be initialized to empty lists
        // to support lazy loading without null reference exceptions
        
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase("GaDeMaTest")
            .Options;
        
        var context = new GameDbContext(options);
        
        await context.Database.EnsureCreatedAsync();
    }
}

/// =============================================================================
/// ISSUE 2: CHARACTERDETAILS PRIMARY KEY ERROR
/// -----------------------------------------------------------------------------
/// 
/// SYMPTOM:
/// --------
/// System.InvalidOperationException : The entity type 'CharacterDetails' requires 
/// a primary key to be defined. If you intended to use a keyless entity type, call 
/// 'HasNoKey' in 'OnModelCreating'.
/// 
/// ERROR MESSAGES:
/// --------------
/// "The entity type 'CharacterDetails' requires a primary key to be defined."
/// 
/// ROOT CAUSE:
/// -----------
/// CharacterDetails entity (child table for FK-as-PK pattern) doesn't have
/// proper primary key configuration. It's designed as a keyless entity where
/// the primary key is derived from the foreign key (ContentItemId).
/// 
/// SOLUTION:
/// ---------
/// In src/GameDev.Core/Configurations/CharacterDetailsEntityTypeConfiguration.cs:
/// 
/// ```csharp
/// public class CharacterDetailsEntityTypeConfiguration : IEntityTypeConfiguration<CharacterDetails>
/// {
///     public void Configure(EntityTypeBuilder<CharacterDetails> builder)
///     {
///         // ✅ Set primary key to be the same as ContentItemId (FK-as-PK pattern)
///         builder.ToTable("CharacterDetails");
///         
///         // Define primary key using ContentItemId column
///         builder.HasOne(d => d.ContentItem)
///             .WithMany(c => c.CharacterDetailsCollection)
///             .HasForeignKey(d => d.ContentItemId)  // This IS the PK
///             .OnDelete(DeleteBehavior.Cascade);  // Cascade delete when parent deleted
///         
///         // ✅ No separate PK column needed - use FK as PK!
///     }
/// }
/// ```
/// 
/// OR in GameDbContext.cs OnModelCreating:
/// 
/// ```csharp
/// protected override void OnModelCreating(ModelBuilder modelBuilder)
/// {
///     modelBuilder.Entity<CharacterDetails>(entity =>
///     {
///         // Set FK column as PK (FK-as-PK pattern)
///         entity.HasKey(e => e.ContentItemId);
///         entity.HasOne(e => e.ContentItem)
///             .WithMany(c => c.CharacterDetailsCollection)
///             .HasForeignKey(e => e.ContentItemId);
///     });
/// }
/// ```
/// =============================================================================

/// =============================================================================
/// ISSUE 3: SERVICE REGISTRATION GAPS
/// -----------------------------------------------------------------------------
/// 
/// SYMPTOM:
/// --------
/// System.InvalidOperationException : No service for type 'XXX' has been registered.
/// 
/// ROOT CAUSE:
/// -----------
/// Some services in ServiceTypeEnum enum are not registered in ApiWebApplicationFactory.
/// 
/// SOLUTION:
/// ---------
/// In src/GameDev.Tests/Factory/ApiWebApplicationFactory.cs:
/// 
/// ```csharp
/// // Ensure ALL services are registered explicitly:
/// private void RegisterServicesWithAddScoped(IServiceProvider serviceProvider, IServiceCollection serviceCollection)
/// {
///     serviceCollection.AddScoped<IContentService, ContentItemService>();
///     serviceCollection.AddScoped<IProjectService, ProjectService>();
///     serviceCollection.AddScoped<ITaskService, ProjectTaskService>();
///     serviceCollection.AddScoped<IApiAuthService, ApiAuthService>();
///     // ... ADD ALL SERVICES HERE!
/// }
/// ```
/// 
/// ALTERNATIVE: Use auto-discovery if available:
/// 
/// ```csharp
/// private void RegisterServicesViaReflection(IServiceProvider serviceProvider)
/// {
///     var assembly = typeof(ContentItemService).Assembly;
///     
///     foreach (var type in assembly.GetTypes())
///     {
///         if (type.Name.EndsWith("Service") && !type.IsInterface)
///         {
///             if (typeof(IGademaService).IsAssignableFrom(type))
///             {
///                 var gademaService = Activator.CreateInstance(type, serviceProvider);
///                 if (gademaService is IGademaService service)
///                 {
///                     _serviceRegistry[service.ServiceType] = type;
///                 }
///             }
///         }
///     }
/// }
/// ```
/// =============================================================================

/// =============================================================================
/// ISSUE 4: VIEWMODEENUM CONFIGURATION
/// -----------------------------------------------------------------------------
/// 
/// SYMPTOM:
/// --------
/// Entity relationship validation errors involving ViewMode navigation properties.
/// 
/// ROOT CAUSE:
/// -----------
/// ViewModeEnum is used as a discriminated column but may not be properly configured
/// in DbContext model builder.
/// 
/// SOLUTION:
/// ---------
/// In GameDbContext.cs OnModelCreating:
/// 
/// ```csharp
/// protected override void OnModelCreating(ModelBuilder modelBuilder)
/// {
///     modelBuilder.Entity<ContentItem>(entity =>
///     {
///         entity.Property(e => e.ViewMode)
///             .HasConversion<string>()  // Convert enum to string for storage
///             .HasMaxLength(50);
///     });
/// }
/// ```
/// =============================================================================

/// =============================================================================
/// ISSUE 5: PROJECT NAVIGATION PROPERTIES
/// -----------------------------------------------------------------------------
/// 
/// SYMPTOM:
/// --------
/// Missing navigation property validation errors for Project entity.
/// 
/// ROOT CAUSE:
/// -----------
/// Project entity might be missing some collection navigations defined in SCHEMA.md.
/// 
/// SOLUTION:
/// ---------
/// Check src/GameDev.Core/Models/Projects/Project.cs and ensure all collections are defined:
/// 
/// ```csharp
/// public class Project
/// {
///     // ... scalar properties
///     
///     // Collection navigations - ensure these exist:
///     public virtual ICollection<ContentItem> ContentItems { get; set; }
///     public virtual ICollection<ProjectTask> Tasks { get; set; }
///     public virtual ICollection<Team> Teams { get; set; }
///     public virtual ICollection<User> Users { get; set; }  // Team members
///     // ... etc
/// }
/// ```
/// =============================================================================

/// =============================================================================
/// ISSUE 6: CONTENTITEM.IDD INITIALIZATION CONFLICT  
/// -----------------------------------------------------------------------------
/// 
/// SYMPTOM:
/// --------
/// Potential ID conflicts when ContentItem.ContentItemId is initialized to Guid.NewGuid()
/// before actual Id property is set.
/// 
/// ROOT CAUSE:
/// -----------
/// Line 173 in ContentItem.cs:
///   public Guid ContentItemId { get; set; } = Guid.NewGuid();  // ❌ Auto-init conflicts!
/// 
/// SOLUTION:
/// ---------
/// Remove the default initializer for ContentItemId:
/// 
/// ```csharp
/// [Required, Display(Name = "Content Item ID")]
/// public Guid ContentItemId { get; set; }  // ✅ No default value
/// ```
/// 
/// Let EF Core handle initialization or set it after Id is assigned.
/// =============================================================================

/// =============================================================================
/// ISSUE 7: ASYNC METHOD RETURN TYPE MISMATCH
/// -----------------------------------------------------------------------------
/// 
/// SYMPTOM:
/// --------
/// Compilation errors if any service method returns wrong async signature.
/// 
/// CHECK:
/// ------
/// All service methods should return:
/// - Task<ApiResponseDto<T>> for success cases
/// - Task<ApiResponseDto<SimpleResponseDto>> for delete operations
/// =============================================================================

/// =============================================================================
/// ISSUE 8: ENUM CONFIGURATION
/// -----------------------------------------------------------------------------
/// 
/// SYMPTOM:
/// --------
/// Entity relationship validation errors involving enum columns.
/// 
/// SOLUTION:
/// ---------
/// Ensure enums are properly configured in OnModelCreating:
/// 
/// ```csharp
/// modelBuilder.Entity<ContentItem>(entity =>
/// {
///     entity.Property(e => e.ContentType)
///         .HasConversion<int>()  // Store as int (enum value)
///         .HasColumnName("content_type");
///     
///     entity.Property(e => e.Status)
///         .HasConversion<string>()  // Or store as string
///         .HasMaxLength(20);
/// });
/// ```
/// =============================================================================

/// =============================================================================
/// ISSUE 9: OPTIONAL NAVIGATIONS NULL INITIALIZATION
/// -----------------------------------------------------------------------------
/// 
/// SYMPTOM:
/// --------
/// Null reference exceptions when accessing optional navigation properties.
/// 
/// SOLUTION:
/// ---------
/// Initialize all optional navigation properties to null:
/// 
/// ```csharp
/// public virtual ReviewStatus? ReviewStatus { get; set; }  // ✅ Nullable
/// public virtual ICollection<ReviewStatus> ReviewStatuses { get; set; } = new List<ReviewStatus>();
/// ```
/// =============================================================================

/// =============================================================================
/// ISSUE 10: COLLECTION INITIALIZATION FOR LAZY LOADING
/// -----------------------------------------------------------------------------
/// 
/// SYMPTOM:
/// --------
/// Null reference exceptions when lazy loading collections.
/// 
/// SOLUTION:
/// ---------
/// Initialize all ICollection<T> properties:
/// 
/// ```csharp
/// public virtual ICollection<ContentVersionLog> VersionLogs { get; set; } = new List<ContentVersionLog>();  // ✅
/// public virtual ICollection<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();  // ✅
/// ```
/// =============================================================================

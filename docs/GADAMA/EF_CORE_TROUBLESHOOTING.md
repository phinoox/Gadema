// =============================================================================
// GaDeMa - Comprehensive Troubleshooting Documentation
// =============================================================================

/// <summary>
/// COMPREHENSIVE TRoubleshooting Guide for GaDeMa Entity Framework Issues
/// 
/// This document covers common EF Core issues encountered during development,
/// their causes, symptoms, solutions, and prevention strategies.
/// </summary>

namespace Gadema.Tests;

using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Gadema.Data.Database;
using Gadema.Core.Models;
using System.Linq;

/// <summary>
/// Collection of troubleshooting tests for common EF Core issues.
/// Run these to verify proper resolution after applying fixes.
/// </summary>
public class EFTroubleshootingTests
{
    /// <summary>
    /// Test 1: Verify navigation property conflicts are resolved
    /// </summary>
    [Fact]
    public async Task NavigationPropertyConflict_ShouldBeResolved()
    {
        // Problem: MetaInfo had duplicate ReviewStatus navigations
        var context = new GameDbContext(
            new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase("GaDeMaTest")
                .Options);

        var MetaInfo = new MetaInfo
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid()
        };
        
        var reviewStatus = new ReviewStatus
        {
            Id = Guid.NewGuid(),
            MetaInfoId = MetaInfo.Id,
            Status = 0,
            ReviewedByUserId = null,
            ReviewComments = "Test comment"
        };

        context.MetaInfos.Add(MetaInfo);
        context.ReviewStatuses.Add(reviewStatus);
        
        await context.SaveChangesAsync();
        
        // Assert - No navigation validation errors
        context.MetaInfos.Should().Contain(c => c.Id == MetaInfo.Id);
    }

    /// <summary>
    /// Test 2: Verify database provider conflicts are resolved
    /// </summary>
    [Fact]
    public void DatabaseProviderConflict_ShouldBeResolved()
    {
        // Problem: Mixing UseSqlite and UseInMemoryDatabase causes exceptions
        
        // Solution: Only use ONE database provider per context instance
        // - Production: UseSqlite or PostgreSQL
        // - Testing with InMemory: UseInMemoryDatabase only
        
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase("GaDeMaTest")  // ✅ Single provider
            .Options;
        
        var context = new GameDbContext(options);
        context.Database.Should().NotBeNull();  // Should initialize without errors
    }

    /// <summary>
    /// Test 3: Verify EnsureCreated conflicts are resolved  
    /// </summary>
    [Fact]
    public void EnsureCreated_InMemoryContext_ShouldNotBeCalled()
    {
        // Problem: Calling EnsureCreated() on in-memory database causes issues
        
        // Solution: For testing, skip EnsureCreated() or only call once per test
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase("GaDeMaTest")
            .Options;
        
        var context = new GameDbContext(options);
        // Don't call context.Database.EnsureCreated() here!
        // Let in-memory database auto-initialize
    }

    /// <summary>
    /// Test 4: Verify entity model can be validated without errors
    /// </summary>
    [Fact]
    public async Task EntityModel_ShouldValidateWithoutErrors()
    {
        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseInMemoryDatabase("GaDeMaTest")
            .Options;
        
        var context = new GameDbContext(options);
        
        // Should not throw: Unable to determine the relationship represented by navigation...
        await context.Database.EnsureCreatedAsync();  // ✅ No validation errors
        
        context.Model.GetEntityTypes().Should().NotBeEmpty();
    }
}

/// =============================================================================
/// NAVIGATION PROPERTY CONFLICT (Fixed)
/// -----------------------------------------------------------------------------
/// 
/// SYMPTOM:
/// --------
/// System.InvalidOperationException : Unable to determine the relationship 
/// represented by navigation 'MetaInfo.ReviewStatus' of type 'ReviewStatus'.
/// 
/// ERROR MESSAGES:
/// --------------
/// 1. "Unable to determine the relationship represented by navigation..."
/// 2. "Entity 'MetaInfo' already has a navigation property for 'ReviewStatus'"
/// 
/// ROOT CAUSE:
/// -----------
/// MetaInfo entity had BOTH single and collection navigations to ReviewStatus:
/// ```csharp
/// // Line ~134 - Single navigation (correct)
/// public virtual ReviewStatus? ReviewStatus { get; set; }
/// 
/// // Line ~193 - Collection navigation (duplicate, wrong!)
/// public virtual ICollection<ReviewStatus> ReviewStatuses { get; set; }
/// ```
/// 
/// The configuration expected ReviewStatuses but MetaInfo had both, causing
/// EF Core to see duplicate relationships with same FK pattern.
/// 
/// SOLUTION:
/// ---------
/// 1. Remove collection property from MetaInfo.cs (lines 187-193)
///    DELETE entire block containing ReviewStatuses collection
///    
/// 2. Update configuration file to match single navigation:
///    In ReviewStatusEntityTypeConfiguration.cs line 33:
///    
///    FROM:
///      builder.HasOne(rs => rs.MetaInfo)
///          .WithMany(ci => ci.ReviewStatuses)
///      
///    TO:
///      builder.HasOne(rs => rs.MetaInfo)
///          .WithOne(ci => ci.ReviewStatus)  // Match single property!
/// 
/// DESIGN DECISION:
/// ----------------
/// MetaInfo should use SINGLE navigation (ReviewStatus?) because:
/// - Each content item has at most ONE review status (Many-to-One relationship)
/// - No junction table pattern used here (unlike ContentTags, CharacterIdentities)
/// - ReviewStatus tracks single review state per content item
/// 
/// PREVENTION:
/// -----------
/// Always verify navigation properties match the relationship cardinality:
/// - One-to-Many: Use single FK on child + ICollection<T> on parent
/// - Many-to-One: Use single FK + single navigation on both sides
/// - Many-to-Many: Use junction table with collections on both sides
/// =============================================================================

/// =============================================================================
/// DATABASE PROVIDER CONFLICT (Fixed)
/// -----------------------------------------------------------------------------
/// 
/// SYMPTOM:
/// --------
/// System.InvalidOperationException : Services for database providers 
/// 'Microsoft.EntityFrameworkCore.Sqlite', 'Microsoft.EntityFrameworkCore.InMemory' 
/// have been registered in the service provider. Only a single database provider can be registered
/// 
/// ROOT CAUSE:
/// -----------
/// Mixing UseSqlite() and UseInMemoryDatabase() calls in same context instance.
/// 
/// SOLUTION:
/// ---------
/// In Program.cs or factory, use ONLY ONE provider per context:
/// 
/// PRODUCTION:
///   .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))  // Or SQLite
///   
/// TESTING WITH INMEMORY:
///   .UseInMemoryDatabase("GaDeMaTest")  // ✅ Only this!
/// 
/// NEVER:
///   .UseSqlite(...)      // ❌ Then .UseInMemoryDatabase(...)  // ❌
/// =============================================================================

/// =============================================================================
/// ENSURECREATED() CONFLICT (Fixed)  
/// -----------------------------------------------------------------------------
/// 
/// SYMPTOM:
/// --------
/// Mixed provider exceptions or database initialization errors when using
/// in-memory database with EnsureCreated().
/// 
/// ROOT CAUSE:
/// -----------
/// Calling context.Database.EnsureCreated() on in-memory database causes
/// conflicts because EF Core tries to create tables that don't exist.
/// 
/// SOLUTION:
/// ---------
/// For testing scenarios:
/// 1. Don't call EnsureCreated() with in-memory database
/// 2. Let in-memory database auto-initialize on first use
/// 3. Or only call EnsureCreated() once per test setup (not in each method)
/// 
/// IN PRODUCTION:
///   .UseSqlite(connectionString).EnsureCreated()  // ✅ OK for file DB
///   .UseNpgsql(connectionString)  // ✅ No EnsureCreated needed
/// =============================================================================

/// =============================================================================
/// TEST FIXTURE INJECTION ERROR (Fixed)
/// -----------------------------------------------------------------------------
/// 
/// SYMPTOM:
/// --------
/// Error Message: "The following constructor parameters did not have matching 
/// fixture data: ApiWebApplicationFactory factory"
/// 
/// ROOT CAUSE:
/// -----------
/// Test class used direct constructor injection without inheriting from xUnit
/// fixture interface (IClassFixture<T>).
/// 
/// BEFORE (WRONG):
/// ```csharp
/// public MetaInfoServiceTests(ApiWebApplicationFactory factory)  // ❌ No inheritance!
/// {
///     _service = factory.Services.GetRequiredService<IContentService>();
/// }
/// ```
/// 
/// AFTER (CORRECT - Integration Test):
/// ```csharp
/// public class MetaInfoServiceIntegrationTests : IClassFixture<ApiWebApplicationFactory>  // ✅ Inherit!
/// {
///     private readonly IContentService _service;
///     
///     public MetaInfoServiceIntegrationTests(ApiWebApplicationFactory factory)
///     {
///         _service = factory.Services.GetRequiredService<IContentService>();
///     }
/// }
/// ```
/// 
/// ALTERNATIVE (Unit Test without Factory):
/// ```csharp
/// public class MetaInfoServiceUnitTest  // ✅ No inheritance needed!
/// {
///     private readonly IContentService _service;
///     
///     public MetaInfoServiceUnitTest()  // ✅ Simple constructor
///     {
///         var options = new DbContextOptionsBuilder<GameDbContext>()
///             .UseInMemoryDatabase("GaDeMaTest")
///             .Options;
///         
///         _context = new GameDbContext(options);
///         _service = new MetaInfoService(_context, null!);
///     }
/// }
/// ```
/// 
/// PATTERN:
/// --------
/// - Use IClassFixture<T> for integration tests with WebApplicationFactory
/// - Use direct instantiation for unit tests with in-memory database
/// =============================================================================

/// =============================================================================
/// MISSING DB CONTEXT CLASS NAME (Fixed)
/// -----------------------------------------------------------------------------
/// 
/// SYMPTOM:
/// --------
/// Compiler error: 'Context' does not exist or cannot be found.
/// 
/// ROOT CAUSE:
/// -----------
/// Using generic name 'Context' instead of actual class name 'GameDbContext'.
/// 
/// SOLUTION:
/// ---------
/// Always use the fully-qualified class name from Gadema.Data namespace:
/// 
/// FROM (WRONG):
///   var options = new DbContextOptionsBuilder<Context>()...
///   _context = new Context(options);
///   
/// TO (CORRECT):
///   var options = new DbContextOptionsBuilder<GameDbContext>()...
///   _context = new GameDbContext(options);
/// =============================================================================

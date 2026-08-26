// =============================================================================
// GaDeMa - Navigation Property Conflict Resolution Document
// =============================================================================

/// <summary>
/// RESOLUTION FOR: Entity Framework Navigation Property Conflicts
/// 
/// Problem: ContentItem entity had both single and collection navigation properties
/// to ReviewStatus, causing EF Core validation errors.
/// </summary>

namespace GameDev.Tests;

using Xunit;
using FluentAssertions;

/// <summary>
/// Test to verify navigation property conflicts are resolved.
/// </summary>
public class NavigationPropertyConflictResolutionTests
{
    [Fact]
    public async Task ContentItem_ShouldNotHaveDuplicateNavigationProperties()
    {
        // Arrange
        var context = new GameDbContext(
            new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase("GaDeMaTest")
                .Options);

        // Act - Try to add a ReviewStatus entity
        var contentItem = new ContentItem
        {
            Id = Guid.NewGuid(),
            ProjectId = Guid.NewGuid()
        };
        
        var reviewStatus = new ReviewStatus
        {
            Id = Guid.NewGuid(),
            ContentItemId = contentItem.Id,
            Status = 0,
            ReviewedByUserId = null,
            ReviewComments = "Test comment"
        };

        // Assert - Should not throw navigation validation errors
        context.ContentItems.Add(contentItem);
        context.ReviewStatuses.Add(reviewStatus);
        
        await context.SaveChangesAsync();
        
        context.ContentItems.Should().Contain(c => c.Id == contentItem.Id);
    }
}

/// <summary>
/// Documentation for Navigation Property Conflict Resolution
/// =============================================================================
/// 
/// PROBLEM:
/// --------
/// The ContentItem entity in src/GameDev.Core/Models/Content/ContentItem.cs had BOTH:
/// 1. Single navigation: public virtual ReviewStatus? ReviewStatus { get; set; }
/// 2. Collection navigation: public virtual ICollection<ReviewStatus> ReviewStatuses { get; set; }
/// 
/// The configuration file ReviewStatusEntityTypeConfiguration expected:
/// builder.HasOne(rs => rs.ContentItem).WithMany(ci => ci.ReviewStatuses)
/// 
/// This created a navigation conflict because EF Core saw two different ways to access
/// the same relationship, causing validation errors during model initialization.
/// 
/// ROOT CAUSE:
/// -----------
/// During development, multiple versions of the entity were merged without properly
/// consolidating duplicate navigation properties. The `ReviewStatuses` collection was
/// added but not removed when the single `ReviewStatus?` property already existed.
/// 
/// SOLUTION:
/// ---------
/// 1. Remove the duplicate collection property from ContentItem.cs (lines 187-193)
///    - Delete the entire block containing ReviewStatuses collection
///    - Keep only the single ReviewStatus? property (line ~134)
/// 
/// 2. Update configuration file to use single navigation:
///    In src/GameDev.Core/Configurations/Tasks/ReviewStatusEntityTypeConfiguration.cs:
///    
///    FROM:
///      builder.HasOne(rs => rs.ContentItem)
///          .WithMany(ci => ci.ReviewStatuses)
///      
///    TO:
///      builder.HasOne(rs => rs.ContentItem)
///          .WithOne(ci => ci.ReviewStatus)  // Match the single property!
/// 
/// DESIGN DECISION:
/// ----------------
/// ContentItem should use a SINGLE navigation property (ReviewStatus?) because:
/// - Each content item has at most ONE review status (Many-to-One relationship)
/// - The junction table pattern (ContentTags, CharacterIdentities) is NOT used here
/// - ReviewStatus tracks a single review state per content item
/// 
/// If multiple reviews were needed, we would use:
/// 1. A separate "ReviewComments" entity with FK to ContentItem
/// 2. Or remove the navigation and access via DbContext directly
/// 
/// VERIFICATION:
/// -------------
/// After applying this fix:
/// 1. dotnet build should succeed without errors
/// 2. Unit tests should pass (no navigation validation exceptions)
/// 3. Integration tests should pass (proper relationship mapping)
/// 
/// IMPACT:
/// -------
/// This change affects:
/// - src/GameDev.Core/Models/Content/ContentItem.cs
/// - src/GameDev.Core/Configurations/Tasks/ReviewStatusEntityTypeConfiguration.cs
/// - All entities that reference ContentItem.ReviewStatus navigation property
/// 
/// No data migration needed - this is purely a model definition change.
/// =============================================================================
</summary>

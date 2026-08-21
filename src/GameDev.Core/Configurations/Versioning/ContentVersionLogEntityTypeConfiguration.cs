// =============================================================================
// GameDev.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameDev.Core.Models;

namespace GameDev.Core.Configurations.Versioning;

/// <summary>
/// Configuration for ContentVersionLog entity in version control system.
/// </summary>
public class ContentVersionLogEntityTypeConfiguration : IEntityTypeConfiguration<ContentVersionLog>
{
    public void Configure(EntityTypeBuilder<ContentVersionLog> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);

        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ContentItemId);
        builder.HasIndex(e => e.VersionNumber);  // Changed from Version to match model property
        builder.HasIndex(e => e.CreatedByUserId);  // Changed from SnapshotType (not in ContentVersionLog model)

        // Navigation property: ContentItem (Cascade delete)
        builder.HasOne(cvl => cvl.ContentItem)
            .WithMany(ci => ci.VersionLogs)
            .HasForeignKey(cvl => cvl.ContentItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Properties configuration
        builder.Property(e => e.VersionNumber).IsRequired();  // Changed from Version to VersionNumber
        builder.Property(e => e.ChangedByUserId).IsRequired();  // Changed from CreatedByUserId to match model
        builder.Property(e => e.ChangeDescription);  // Added missing property

    }
}

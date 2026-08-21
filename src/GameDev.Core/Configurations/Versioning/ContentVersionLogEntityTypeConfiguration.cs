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
    /// <summary>
    /// Configure ContentVersionLog entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<ContentVersionLog> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ContentItemId);
        builder.HasIndex(e => e.Version);
        builder.HasIndex(e => e.SnapshotType);
        builder.HasIndex(e => e.CreatedByUserId);
        builder.HasIndex(e => e.CreatedAt);  // Query recent logs
        
        // Navigation property: ContentItem (Cascade delete)
        builder.HasOne(cvl => cvl.ContentItem)
            .WithMany(ci => ci.VersionLogs)
            .HasForeignKey(cvl => cvl.ContentItemId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Properties configuration
        builder.Property(e => e.Version).IsRequired();
        builder.Property(e => e.CreatedByUserId).IsRequired();
        builder.Property(e => e.SnapshotDataJson).HasMaxLength(50000);
    }
}
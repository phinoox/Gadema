// =============================================================================
using GameDev.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
// GameDev.Core - Shared Domain Models & Interfaces
// =============================================================================

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
        builder.HasIndex(e => e.VersionNumber);  // Query recent versions
        
        // Navigation property: ContentItem (SetNull to preserve version history)
        builder.HasOne(cvl => cvl.ContentItem)
            .WithMany(ci => ci.VersionLogs)
            .HasForeignKey(cvl => cvl.ContentItemId)
            .OnDelete(DeleteBehavior.SetNull);

        // Properties configuration
        builder.Property(e => e.ChangedByUserId).IsRequired();
        builder.Property(e => e.ChangeDescription).HasMaxLength(2048);
    }
}

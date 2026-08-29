// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Versioning;

/// <summary>
/// Configuration for ContentSnapshot entity in game development management system.
/// </summary>
public class ContentSnapshotEntityTypeConfiguration : IEntityTypeConfiguration<ContentSnapshot>
{
    /// <summary>
    /// Configure ContentSnapshot entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<ContentSnapshot> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ContentItemId);
        builder.HasIndex(e => e.SnapshotVersion);
        builder.HasIndex(e => e.SnapshotType);
        builder.HasIndex(e => e.CreatedByUserId);
        builder.HasIndex(e => e.CreatedAt);  // Query recent snapshots
        
        // Properties configuration
        builder.Property(e => e.SnapshotDataJson).HasMaxLength(50000);
    }
}

// =============================================================================
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.WorldBuilding;

/// <summary>
/// Configuration for WorldLocation entity in game development management system.
/// Represents hierarchical world-building locations (countries, regions, cities, villages, landmarks).
/// </summary>
public class WorldLocationEntityTypeConfiguration : IEntityTypeConfiguration<WorldLocation>
{
    public void Configure(EntityTypeBuilder<WorldLocation> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.MetaInfoId);
        builder.HasIndex(e => e.LocationType);
        builder.HasIndex(e => e.ParentId);
        
        // MetaInfo relationship (REQUIRED)
        builder.HasOne(wl => wl.MetaInfo)
            .WithMany()
            .HasForeignKey(wl => wl.MetaInfoId)
            .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade through MetaInfo
        
        // Self-referencing parent/children relationship
        builder.HasOne(wl => wl.Parent)
            .WithMany(wl => wl.Children)
            .HasForeignKey(wl => wl.ParentId)
            .OnDelete(DeleteBehavior.SetNull);  // When parent deleted, children become top-level
        
        // Properties
        builder.Property(e => e.Description).HasMaxLength(4096);
    }
}
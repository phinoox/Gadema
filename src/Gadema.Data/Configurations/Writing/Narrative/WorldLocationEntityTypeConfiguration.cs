using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Writing.WorldBuilding;

namespace Gadema.Data.Configurations.Writing.Narrative;

/// <summary>
/// Configuration for WorldLocation entity in game development management system.
/// </summary>
public class WorldLocationEntityTypeConfiguration : IEntityTypeConfiguration<WorldLocation>
{
    /// <summary>
    /// Configure WorldLocation entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<WorldLocation> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);

        builder.ToTable("WorldLocations");

        // Self-referencing hierarchy (Parent/Children)
        builder.HasOne(w => w.Parent)
            .WithMany(w => w.Children)
            .HasForeignKey(w => w.ParentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder.HasIndex(e => e.MetaInfoId).HasDatabaseName("IX_WorldLocation_MetaInfoId");
        builder.HasIndex(e => e.LocationType).HasDatabaseName("IX_WorldLocation_LocationType");
    }
}

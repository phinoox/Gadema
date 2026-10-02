using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Writing.WorldBuilding;

namespace Gadema.Data.Configurations.Writing.Narrative;

/// <summary>
/// Configuration for Faction entity in game development management system.
/// </summary>
public class FactionEntityTypeConfiguration : IEntityTypeConfiguration<Faction>
{
    /// <summary>
    /// Configure Faction entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<Faction> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);

        builder.ToTable("Factions");

        // Indexes
        builder.HasIndex(e => e.MetaInfoId).HasDatabaseName("IX_Faction_MetaInfoId");
        
        // Relationship to WorldLocation (Optional)
        builder.HasOne(f => f.Location)
            .WithMany()
            .HasForeignKey(f => f.LocationId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

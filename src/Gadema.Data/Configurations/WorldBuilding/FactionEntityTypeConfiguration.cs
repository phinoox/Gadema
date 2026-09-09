// src/Gadema.Data/Configurations/WorldBuilding/FactionEntityTypeConfiguration.cs
using Gadema.Core.Models.WorldBuilding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.WorldBuilding;

public class FactionEntityTypeConfiguration : IEntityTypeConfiguration<Faction>
{
    public void Configure(EntityTypeBuilder<Faction> builder)
    {
        builder.HasKey(e => e.Id);
        
        // Indexes
        builder.HasIndex(e => e.MetaInfoId);
        builder.HasIndex(e => e.LocationId);
        
        // MetaInfo relationship (REQUIRED)
        builder.HasOne(f => f.MetaInfo)
            .WithMany()
            .HasForeignKey(f => f.MetaInfoId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // WorldLocation relationship (OPTIONAL)
        builder.HasOne(f => f.Location)
            .WithMany()  // Location doesn't need a back-reference to factions
            .HasForeignKey(f => f.LocationId)
            .OnDelete(DeleteBehavior.SetNull);  // When location deleted, faction loses reference but survives
        
        // Properties
        builder.Property(e => e.Ideology).HasMaxLength(4096);
        builder.Property(e => e.Goals).HasMaxLength(4096);
    }
}
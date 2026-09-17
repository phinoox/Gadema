// src/Gadema.Data/Configurations/Writing/WorldLocationEntityTypeConfiguration.cs
using Gadema.Core.Models.Writing.WorldBuilding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Narrative;

public class WorldLocationEntityTypeConfiguration : IEntityTypeConfiguration<WorldLocation>
{
    public void Configure(EntityTypeBuilder<WorldLocation> builder)
    {
        builder.ToTable("WorldLocations");
        builder.HasKey(e => e.Id);

        // Indexes
        builder.HasIndex(e => e.MetaInfoId).HasDatabaseName("IX_WorldLocation_MetaInfoId");
        
        // Note: Project access is handled via the ContentMetaInfo in the Service layer
    }
}
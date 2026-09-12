// src/Gadema.Data/Configurations/Writing/WorldLocationEntityTypeConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class WorldLocationEntityTypeConfiguration : IEntityTypeConfiguration<WorldLocation>
{
    public void Configure(EntityTypeBuilder<WorldLocation> builder)
    {
        builder.ToTable("WorldLocations");
        builder.HasKey(e => e.Id);

        // Indexes
        builder.HasIndex(e => e.MetaInfoId).HasDatabaseName("IX_WorldLocation_MetaInfoId");
        
        // Note: Project access is handled via the MetaInfo in the Service layer
    }
}
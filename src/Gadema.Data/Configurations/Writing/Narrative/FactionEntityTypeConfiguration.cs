// src/Gadema.Data/Configurations/Writing/FactionEntityTypeConfiguration.cs
using Gadema.Core.Models.WorldBuilding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Narrative;

public class FactionEntityTypeConfiguration : IEntityTypeConfiguration<Faction>
{
    public void Configure(EntityTypeBuilder<Faction> builder)
    {
        builder.ToTable("Factions");
        builder.HasKey(e => e.Id);

        // Indexes
        builder.HasIndex(e => e.MetaInfoId).HasDatabaseName("IX_Faction_MetaInfoId");
        
        // Note: Project access is handled via the ContentMetaInfo in the Service layer
        // builder.HasIndex(e => e.ProjectId).HasDatabaseName("IX_Faction_ProjectId"); // Removed per convention
    }
}
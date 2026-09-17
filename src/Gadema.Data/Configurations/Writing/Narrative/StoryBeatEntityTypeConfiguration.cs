// src/Gadema.Data/Configurations/Writing/StoryBeatEntityTypeConfiguration.cs
using Gadema.Core.Models.Writing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Narrative;

public class StoryBeatEntityTypeConfiguration : IEntityTypeConfiguration<StoryBeat>
{
    public void Configure(EntityTypeBuilder<StoryBeat> builder)
    {
        builder.ToTable("StoryBeats");
        builder.HasKey(e => e.Id);

        // Many-to-Many with OutlineSection
        builder.HasMany(e => e.LinkedOutlineSections)
               .WithMany(e => e.LinkedBeats)
               .UsingEntity(j => j.ToTable("OutlineSectionBeats"));

        // Many-to-Many with Scene
        builder.HasMany(e => e.Scenes)
               .WithMany(e => e.StoryBeats)
               .UsingEntity(j => j.ToTable("SceneBeats"));

        // Indexes
        builder.HasIndex(e => e.MetaInfoId).HasDatabaseName("IX_StoryBeat_MetaInfoId");
        builder.HasIndex(e => e.StoryId).HasDatabaseName("IX_StoryBeat_StoryId");
        builder.HasIndex(e => e.OrderIndex).HasDatabaseName("IX_StoryBeat_OrderIndex");
    }
}
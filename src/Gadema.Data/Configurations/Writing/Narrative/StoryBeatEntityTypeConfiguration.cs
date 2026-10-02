using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Writing.Narrative;

namespace Gadema.Data.Configurations.Writing.Narrative;

/// <summary>
/// Configuration for StoryBeat entity in game development management system.
/// </summary>
public class StoryBeatEntityTypeConfiguration : IEntityTypeConfiguration<StoryBeat>
{
    /// <summary>
    /// Configure StoryBeat entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<StoryBeat> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);

        builder.ToTable("StoryBeats");

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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Writing.Narrative;

namespace Gadema.Data.Configurations.Writing.Narrative;

/// <summary>
/// Configuration for SceneStoryBeatMapping junction entity in game development management system.
/// </summary>
public class SceneStoryBeatMappingEntityTypeConfiguration : IEntityTypeConfiguration<SceneStoryBeatMapping>
{
    /// <summary>
    /// Configure SceneStoryBeatMapping entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<SceneStoryBeatMapping> builder)
    {
        // Composite primary key for the junction table
        builder.HasKey(ssbm => new { ssbm.SceneId, ssbm.StoryBeatId });

        // Indexes for performance
        builder.HasIndex(e => e.SceneId);
        builder.HasIndex(e => e.StoryBeatId);
        builder.HasIndex(e => e.BeatOrderIndex);
        builder.HasIndex(e => e.CreatedAt);
        
        // Navigation property: Scene (Cascade delete)
        builder.HasOne(ssbm => ssbm.Scene)
            .WithMany() // Corrected from empty WithMany to use the collection on Scene
            .HasForeignKey(ssbm => ssbm.SceneId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Navigation property: StoryBeat (Cascade delete)
        builder.HasOne(ssbm => ssbm.StoryBeat)
            .WithMany() // Corrected from empty WithMany to use the collection on StoryBeat
            .HasForeignKey(ssbm => ssbm.StoryBeatId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Properties configuration
        builder.Property(e => e.BeatOrderIndex).HasDefaultValue(0);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}

// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================
using Gadema.Core.Models.Writing.Narrative;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Narrative;

/// <summary>
/// Configuration for SceneStoryBeatMapping junction entity in game development management system.
/// Enables Many-to-Many relationship between Scene and StoryBeat.
/// Allows a single scene to fulfill multiple narrative beats, and a beat to be referenced by many scenes.
/// </summary>
public class SceneStoryBeatMappingEntityTypeConfiguration : IEntityTypeConfiguration<SceneStoryBeatMapping>
{
    public void Configure(EntityTypeBuilder<SceneStoryBeatMapping> builder)
    {
        // Composite primary key for the junction table
        builder.HasKey(ssbm => new { ssbm.SceneId, ssbm.StoryBeatId });
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.SceneId);
        builder.HasIndex(e => e.StoryBeatId);
        builder.HasIndex(e => e.BeatOrderIndex);
        builder.HasIndex(e => e.CreatedAt);
        
        // Navigation property: Scene
        builder.HasOne(ssbm => ssbm.Scene)
            .WithMany()
            .HasForeignKey(ssbm => ssbm.SceneId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete mapping when scene deleted
        
        // Navigation property: StoryBeat
        builder.HasOne(ssbm => ssbm.StoryBeat)
            .WithMany()
            .HasForeignKey(ssbm => ssbm.StoryBeatId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete mapping when beat deleted
        
        // Properties configuration
        builder.Property(e => e.BeatOrderIndex).HasDefaultValue(0);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}

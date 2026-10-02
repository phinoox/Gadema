using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Writing.Narrative;

namespace Gadema.Data.Configurations.Writing.Narrative;

/// <summary>
/// Configuration for Scene entity in game development management system.
/// </summary>
public class SceneEntityTypeConfiguration : IEntityTypeConfiguration<Scene>
{
    /// <summary>
    /// Configure Scene entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<Scene> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);

        builder.ToTable("Scenes");

        // Junction Table for Many-to-Many with StoryBeat
        builder.HasMany(e => e.StoryBeats)
            .WithMany(e => e.Scenes)
            .UsingEntity(j => j.ToTable("SceneBeats"));

        // Owned Collections (Character Relations & States)
        builder.HasMany(e => e.CharacterRelations)
            .WithOne(e => e.TriggerScene) 
            .HasForeignKey(e => e.TriggerSceneId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.CharacterStates)
            .WithOne(e => e.TriggerScene) 
            .HasForeignKey(e => e.TriggerSceneId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.MetaInfoId).HasDatabaseName("IX_Scene_MetaInfoId");
        builder.HasIndex(e => e.StoryChapterId).HasDatabaseName("IX_Scene_StoryChapterId");
    }
}

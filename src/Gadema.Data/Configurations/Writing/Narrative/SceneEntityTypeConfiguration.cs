// src/Gadema.Data/Configurations/Writing/SceneEntityTypeConfiguration.cs
using Gadema.Core.Models.Writing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Narrative;

public class SceneEntityTypeConfiguration : IEntityTypeConfiguration<Scene>
{
    public void Configure(EntityTypeBuilder<Scene> builder)
    {
        builder.ToTable("Scenes");

        builder.HasKey(e => e.Id);

        // Junction Table for Many-to-Many with StoryBeat
        builder.HasMany(e => e.StoryBeats)
               .WithMany(e => e.Scenes)
               .UsingEntity(j => j.ToTable("SceneBeats"));

        // Owned Collections
        builder.HasMany(e => e.CharacterRelations)
               .WithOne(e => e.TriggerScene) // Assuming FK is SceneId
               .HasForeignKey(e => e.TriggerSceneId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.CharacterStates)
               .WithOne(e => e.TriggerScene) // Assuming FK is SceneId
               .HasForeignKey(e => e.TriggerSceneId)
               .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(e => e.MetaInfoId).HasDatabaseName("IX_Scene_MetaInfoId");
        builder.HasIndex(e => e.StoryChapterId).HasDatabaseName("IX_Scene_StoryChapterId");
    }
}

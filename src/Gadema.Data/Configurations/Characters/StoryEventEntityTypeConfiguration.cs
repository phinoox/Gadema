// src/Gadema.Data/Configurations/Characters/StoryEventEntityTypeConfiguration.cs
using Gadema.Core.Models.Characters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Characters;

public class StoryEventEntityTypeConfiguration : IEntityTypeConfiguration<StoryEvent>
{
    public void Configure(EntityTypeBuilder<StoryEvent> builder)
    {
        // Index on SceneId for querying events by scene
        builder.HasIndex(e => e.SceneId)
            .HasDatabaseName("IX_StoryEvent_SceneId");

        // Index on ActorMetaInfoId for querying events by actor
        builder.HasIndex(e => e.ActorMetaInfoId)
            .HasDatabaseName("IX_StoryEvent_ActorMetaInfoId");

        // Index on EventType for filtering by event type
        builder.HasIndex(e => e.EventType)
            .HasDatabaseName("IX_StoryEvent_EventType");

        // Index on TargetEntityTypeId + TargetEntityId for looking up the changed entity
        builder.HasIndex(e => new { e.TargetEntityTypeId, e.TargetEntityId })
            .HasDatabaseName("IX_StoryEvent_TargetEntity");

        // Restrict: Scene is required — event loses context if scene is deleted
        builder.HasOne(e => e.Scene)
            .WithMany(s => s.StoryEvents)
            .HasForeignKey(e => e.SceneId)
            .OnDelete(DeleteBehavior.Restrict);

        // Restrict: ActorMetaInfo is required — event has no actor without identity
        builder.HasOne(e => e.ActorMetaInfo)
            .WithMany()
            .HasForeignKey(e => e.ActorMetaInfoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

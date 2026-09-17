// src/Gadema.Data/Configurations/Characters/CharacterStateEntityTypeConfiguration.cs
using Gadema.Core.Models.Characters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Writing.Characters;

public class CharacterStateEntityTypeConfiguration : IEntityTypeConfiguration<CharacterState>
{
    public void Configure(EntityTypeBuilder<CharacterState> builder)
    {
        // Index on MetaInfoId for fast lookups
        builder.HasIndex(e => e.MetaInfoId)
            .HasDatabaseName("IX_CharacterState_MetaInfoId");

        // Index on FactionId for filtering by faction
        builder.HasIndex(e => e.FactionId)
            .HasDatabaseName("IX_CharacterState_FactionId");

        // Index on LocationId for filtering by location
        builder.HasIndex(e => e.LocationId)
            .HasDatabaseName("IX_CharacterState_LocationId");

        // Index on Role for filtering by character role
        builder.HasIndex(e => e.Role)
            .HasDatabaseName("IX_CharacterState_Role");

        // Index on LifeStatus for filtering by life status
        builder.HasIndex(e => e.LifeStatus)
            .HasDatabaseName("IX_CharacterState_LifeStatus");

        // Index on TriggerSceneId for querying state changes per scene
        builder.HasIndex(e => e.TriggerSceneId)
            .HasDatabaseName("IX_CharacterState_TriggerSceneId");

        // Restrict: if ContentMetaInfo is deleted, cascade the delete (state loses identity)
        builder.HasOne(e => e.ContentMetaInfo)
            .WithMany()
            .HasForeignKey(e => e.MetaInfoId)
            .OnDelete(DeleteBehavior.Restrict);

        // SetNull: Faction is optional — can be unaffiliated
        builder.HasOne(e => e.Faction)
            .WithMany(f => f.CharacterStates)
            .HasForeignKey(e => e.FactionId)
            .OnDelete(DeleteBehavior.SetNull);

        // SetNull: Location is optional — whereabouts unknown
        builder.HasOne(e => e.Location)
            .WithMany(l => l.CharacterStates)
            .HasForeignKey(e => e.LocationId)
            .OnDelete(DeleteBehavior.SetNull);

        // SetNull: TriggerScene is optional — state created outside a scene context
        builder.HasOne(e => e.TriggerScene)
            .WithMany()
            .HasForeignKey(e => e.TriggerSceneId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

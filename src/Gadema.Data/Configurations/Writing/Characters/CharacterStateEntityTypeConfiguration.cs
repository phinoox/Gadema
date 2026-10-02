using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Writing.Characters;

namespace Gadema.Data.Configurations.Writing.Characters;

/// <summary>
/// Configuration for CharacterState entity in game development management system.
/// </summary>
public class CharacterStateEntityTypeConfiguration : IEntityTypeConfiguration<CharacterState>
{
    /// <summary>
    /// Configure CharacterState entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<CharacterState> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);

        // Indexes for performance
        builder.HasIndex(e => e.MetaInfoId).HasDatabaseName("IX_CharacterState_MetaInfoId");
        builder.HasIndex(e => e.FactionId).HasDatabaseName("IX_CharacterState_FactionId");
        builder.HasIndex(e => e.LocationId).HasDatabaseName("IX_CharacterState_LocationId");
        builder.HasIndex(e => e.Role).HasDatabaseName("IX_CharacterState_Role");
        builder.HasIndex(e => e.LifeStatus).HasDatabaseName("IX_CharacterState_LifeStatus");
        builder.HasIndex(e => e.TriggerSceneId).HasDatabaseName("IX_CharacterState_TriggerSceneId");

        // Relationship: ContentMetaInfo (Cascade delete)
        builder.HasOne(e => e.ContentMetaInfo)
            .WithMany()
            .HasForeignKey(e => e.MetaInfoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship: Faction (SetNull)
        builder.HasOne(e => e.Faction)
            .WithMany(f => f.CharacterStates)
            .HasForeignKey(e => e.FactionId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relationship: Location (SetNull)
        builder.HasOne(e => e.Location)
            .WithMany(l => l.CharacterStates)
            .HasForeignKey(e => e.LocationId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relationship: TriggerScene (SetNull)
        builder.HasOne(e => e.TriggerScene)
            .WithMany()
            .HasForeignKey(e => e.TriggerSceneId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

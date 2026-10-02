using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Writing.Characters;

namespace Gadema.Data.Configurations.Writing.Narrative;

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

        builder.ToTable("CharacterStates");

        // Owned by Scene
        builder.HasOne(e => e.TriggerScene)
            .WithMany(s => s.CharacterStates)
            .HasForeignKey(e => e.TriggerSceneId)
            .OnDelete(DeleteBehavior.Cascade);

        // Character Link
        builder.HasOne(e => e.Character)
            .WithMany()
            .HasForeignKey(e => e.CharacterId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => e.TriggerSceneId).HasDatabaseName("IX_CharacterState_SceneId");
        builder.HasIndex(e => e.CharacterId).HasDatabaseName("IX_CharacterState_CharacterId");
    }
}

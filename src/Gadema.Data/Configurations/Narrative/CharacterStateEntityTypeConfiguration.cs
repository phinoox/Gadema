// src/Gadema.Data/Configurations/Writing/CharacterStateEntityTypeConfiguration.cs
using Gadema.Core.Models.Characters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CharacterStateEntityTypeConfiguration : IEntityTypeConfiguration<CharacterState>
{
    public void Configure(EntityTypeBuilder<CharacterState> builder)
    {
        builder.ToTable("CharacterStates");
        builder.HasKey(e => e.Id);

        // Owned by Scene
        builder.HasOne(e => e.TriggerScene)
               .WithMany(e => e.CharacterStates)
               .HasForeignKey(e => e.TriggerSceneId)
               .OnDelete(DeleteBehavior.Cascade);

        // Character Link
        builder.HasOne(e => e.Character)
               .WithMany() // Adjust if Character has a collection
               .HasForeignKey(e => e.CharacterId)
               .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => e.TriggerSceneId).HasDatabaseName("IX_CharacterState_SceneId");
        builder.HasIndex(e => e.CharacterId).HasDatabaseName("IX_CharacterState_CharacterId");
       
    }
}
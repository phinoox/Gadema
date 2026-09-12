// src/Gadema.Data/Configurations/Writing/CharacterRelationEntityTypeConfiguration.cs
using Gadema.Core.Models.Characters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CharacterRelationEntityTypeConfiguration : IEntityTypeConfiguration<CharacterRelation>
{
    public void Configure(EntityTypeBuilder<CharacterRelation> builder)
    {
        builder.ToTable("CharacterRelations");
        builder.HasKey(e => e.Id);

        // Owned by Scene
        builder.HasOne(e => e.TriggerScene)
               .WithMany(e => e.CharacterRelations)
               .HasForeignKey(e => e.TriggerSceneId)
               .OnDelete(DeleteBehavior.Cascade);

        // Character Links (Self-referencing via FKs)
        builder.HasOne(e => e.SourceCharacter)
               .WithMany() // Adjust if Character has a collection
               .HasForeignKey(e => e.SourceCharacterId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TargetCharacter)
               .WithMany() // Adjust if Character has a collection
               .HasForeignKey(e => e.TargetCharacterId)
               .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => e.TriggerSceneId).HasDatabaseName("IX_CharacterRelation_SceneId");
        builder.HasIndex(e => e.RelationType).HasDatabaseName("IX_CharacterRelation_RelationType");
    }
}
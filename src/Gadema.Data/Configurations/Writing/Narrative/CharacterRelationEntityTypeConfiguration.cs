using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Writing.Characters;

namespace Gadema.Data.Configurations.Writing.Narrative;

/// <summary>
/// Configuration for CharacterRelation entity in game development management system.
/// </summary>
public class CharacterRelationEntityTypeConfiguration : IEntityTypeConfiguration<CharacterRelation>
{
    /// <summary>
    /// Configure CharacterRelation entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<CharacterRelation> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);

        builder.ToTable("CharacterRelations");

        // Relationship to Scene (Owned by Scene)
        builder.HasOne(e => e.TriggerScene)
            .WithMany(s => s.CharacterRelations)
            .HasForeignKey(e => e.TriggerSceneId)
            .OnDelete(DeleteBehavior.Cascade);

        // Character Links (Self-referencing via FKs)
        builder.HasOne(e => e.SourceCharacter)
            .WithMany()
            .HasForeignKey(e => e.SourceCharacterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.TargetCharacter)
            .WithMany()
            .HasForeignKey(e => e.TargetCharacterId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(e => e.TriggerSceneId).HasDatabaseName("IX_CharacterRelation_SceneId");
        builder.HasIndex(e => e.RelationType).HasDatabaseName("IX_CharacterRelation_RelationType");
    }
}

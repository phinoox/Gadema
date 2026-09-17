// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================
using Gadema.Core.Models.Writing.Characters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Writing.Characters;

/// <summary>
/// Configuration for CharacterRelation entity in game development management system.
/// Tracks evolving relationships between characters throughout the story.
/// Event-driven junction table that allows relationships to change over time.
/// </summary>
public class CharacterRelationEntityTypeConfiguration : IEntityTypeConfiguration<CharacterRelation>
{
    public void Configure(EntityTypeBuilder<CharacterRelation> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.SourceCharacterId);
        builder.HasIndex(e => e.TargetCharacterId);
        builder.HasIndex(e => e.TriggerSceneId);
        builder.HasIndex(e => e.RelationType);
        builder.HasIndex(e => e.CreatedAt);
        
        // Navigation property: Source Character
        builder.HasOne(cr => cr.SourceCharacter)
            .WithMany()
            .HasForeignKey(cr => cr.SourceCharacterId)
            .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade delete through characters
        
        // Navigation property: Target Character
        builder.HasOne(cr => cr.TargetCharacter)
            .WithMany()
            .HasForeignKey(cr => cr.TargetCharacterId)
            .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade delete through characters
        
        // Navigation property: Trigger Scene
        builder.HasOne(cr => cr.TriggerScene)
            .WithMany(s => s.CharacterRelations)
            .HasForeignKey(cr => cr.TriggerSceneId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete relations when scene deleted
        
        // Properties configuration
        builder.Property(e => e.RelationType).HasDefaultValue(RelationTypeEnum.Alien);
        builder.Property(e => e.Description).HasMaxLength(1024);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}

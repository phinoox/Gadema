// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================
using Gadema.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;
using Gadema.Core.Models.Narrative;

namespace Gadema.Data.Configurations.Narrative;

/// <summary>
/// Configuration for Scene entity in game development management system.
/// The "Unit of Work" for writing - contains RawText and links to MetaInfo, Beats, and Segments.
/// </summary>
public class SceneEntityTypeConfiguration : IEntityTypeConfiguration<Scene>
{
    public void Configure(EntityTypeBuilder<Scene> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.MetaInfoId);
        builder.HasIndex(e => e.StoryOutlineId);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.LastModifiedAt);
        
        // Navigation property: MetaInfo (MetaInfo) - The metadata wrapper
        builder.HasOne(s => s.MetaInfo)
            .WithMany()
            .HasForeignKey(s => s.MetaInfoId)
            .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade delete through MetaInfo
        
        // Navigation property: StoryOutline (The Map)
        builder.HasOne(s => s.StoryOutline)
            .WithMany(o => o.Scenes)
            .HasForeignKey(s => s.StoryOutlineId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete scenes when outline deleted
        
        // Properties configuration
        builder.Property(e => e.RawText).IsRequired();
        builder.Property(e => e.Status).HasDefaultValue(ContentStatusEnum.Draft);
        builder.Property(e => e.HasGameLogic).HasDefaultValue(false);
        builder.Property(e => e.OrderIndex).HasDefaultValue(0);
        
        // Collection navigation: ContentSegments (Unique Token markers)
        builder.HasMany(s => s.ContentSegments)
            .WithOne(cs => cs.Scene)
            .HasForeignKey(cs => cs.SceneId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Collection navigation: StoryBeats (Many-to-Many via junction table)
        builder.HasMany(s => s.StoryBeats)
            .WithMany(sb => sb.Scenes)
            .UsingEntity<SceneStoryBeatMapping>(
                j => j.HasOne(ssbm => ssbm.StoryBeat)
                    .WithMany()
                    .HasForeignKey(ssbm => ssbm.StoryBeatId)
                    .OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne(ssbm => ssbm.Scene)
                    .WithMany()
                    .HasForeignKey(ssbm => ssbm.SceneId)
                    .OnDelete(DeleteBehavior.Cascade)
            );
        
        // Collection navigation: CharacterRelations
        builder.HasMany(s => s.CharacterRelations)
            .WithOne(cr => cr.TriggerScene)
            .HasForeignKey(cr => cr.TriggerSceneId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

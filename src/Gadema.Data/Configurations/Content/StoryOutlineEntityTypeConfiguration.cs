// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;
using Gadema.Core.Enums;

namespace Gadema.Data.Configurations.Content;

/// <summary>
/// Configuration for StoryOutline entity in game development management system.
/// The "Map" of the project's narrative intent - a distinct document with RawText and StoryBeats.
/// Hierarchy: Project → StoryOutline → StoryBeat → Scene
/// </summary>
public class StoryOutlineEntityTypeConfiguration : IEntityTypeConfiguration<StoryOutline>
{
    public void Configure(EntityTypeBuilder<StoryOutline> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.OutlineStatus);
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.LastModifiedAt);
        
        // Navigation property: Project (direct link, no intermediate Sequence)
        builder.HasOne(s => s.Project)
            .WithMany()
            .HasForeignKey(s => s.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete outline when project deleted
        
        // Properties configuration
        builder.Property(e => e.RawText).IsRequired();
        builder.Property(e => e.Summary).HasMaxLength(4096);  // Legacy field, kept for backward compatibility
        builder.Property(e => e.CharacterSnapshot).HasMaxLength(512);  // Legacy field
        builder.Property(e => e.ThemeStatement).HasMaxLength(1024);  // Legacy field
        builder.Property(e => e.OutlineStatus).HasDefaultValue(OutlineStatusEnum.DraftOutline);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.LastModifiedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        // Collection navigation: StoryBeats (The Landmarks)
        builder.HasMany(o => o.StoryBeats)
            .WithOne(sb => sb.StoryOutline)
            .HasForeignKey(sb => sb.StoryOutlineId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Collection navigation: Scenes (The Execution)
        builder.HasMany(o => o.Scenes)
            .WithOne(s => s.StoryOutline)
            .HasForeignKey(s => s.StoryOutlineId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

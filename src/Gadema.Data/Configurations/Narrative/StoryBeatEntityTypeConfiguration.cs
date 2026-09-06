// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Narrative;

/// <summary>
/// Configuration for StoryBeat entity in game development management system.
/// A "Landmark" or major keypoint within a StoryOutline - becomes draggable "Cards" in the Right Panel.
/// Hierarchy: StoryOutline → StoryBeat ↔ Scene (Many-to-Many via junction table)
/// </summary>
public class StoryBeatEntityTypeConfiguration : IEntityTypeConfiguration<StoryBeat>
{
    public void Configure(EntityTypeBuilder<StoryBeat> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.StoryOutlineId);
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.OrderIndex);
        builder.HasIndex(e => e.Published);
        builder.HasIndex(e => e.CreatedAt);
        
        // Navigation property: StoryOutline (The Map)
        builder.HasOne(sb => sb.StoryOutline)
            .WithMany(o => o.StoryBeats)
            .HasForeignKey(sb => sb.StoryOutlineId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete beats when outline deleted
        
        // Properties configuration
        builder.Property(e => e.BeatTitle).IsRequired().HasMaxLength(128);
        builder.Property(e => e.Slug).HasMaxLength(128);
        builder.Property(e => e.Description).HasMaxLength(4096);
        builder.Property(e => e.OrderIndex).HasDefaultValue(0);
        builder.Property(e => e.Published).HasDefaultValue(false);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(e => e.LastModifiedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
    }
}

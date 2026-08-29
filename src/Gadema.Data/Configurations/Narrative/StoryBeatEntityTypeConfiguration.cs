// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Narrative;

/// <summary>
/// Configuration for StoryBeat entity in game development management system.
/// </summary>
public class StoryBeatEntityTypeConfiguration : IEntityTypeConfiguration<StoryBeat>
{
    /// <summary>
    /// Configure StoryBeat entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<StoryBeat> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.SequenceId);
        builder.HasIndex(e => e.Slug).IsUnique();
        
        // Navigation property: StorySequence (Cascade delete)
        builder.HasOne(sb => sb.StorySequence)
            .WithMany(s => s.Beats)
            .HasForeignKey(sb => sb.SequenceId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete beats when sequence deleted
        
        // Properties configuration
        builder.Property(e => e.BeatTitle).IsRequired().HasMaxLength(128);
    }
}

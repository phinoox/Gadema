// =============================================================================

// GameDev.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameDev.Core.Models;

namespace GameDev.Core.Configurations.Content;

/// <summary>
/// Configuration for StoryOutline entity in game development management system.
/// </summary>
public class StoryOutlineEntityTypeConfiguration : IEntityTypeConfiguration<StoryOutline>
{
    /// <summary>
    /// Configure StoryOutline entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<StoryOutline> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.SequenceId);
        
        // Navigation property: StorySequence (Cascade delete)
        builder.HasOne(s => s.StorySequence)
            .WithMany()
            .HasForeignKey(s => s.SequenceId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete outline when sequence deleted
        
        // Properties configuration
        builder.Property(e => e.Summary).HasMaxLength(4096);
        builder.Property(e => e.CharacterSnapshot).HasMaxLength(512);
    }
}

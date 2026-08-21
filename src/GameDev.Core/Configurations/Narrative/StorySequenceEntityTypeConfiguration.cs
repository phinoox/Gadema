// =============================================================================

// GameDev.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameDev.Core.Models;

namespace GameDev.Core.Configurations.Narrative;

/// <summary>
/// Configuration for StorySequence entity in game development management system.
/// </summary>
public class StorySequenceEntityTypeConfiguration : IEntityTypeConfiguration<StorySequence>
{
    /// <summary>
    /// Configure StorySequence entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<StorySequence> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.Published);
        
        // Navigation property: Project (Cascade delete)
        builder.HasOne(s => s.Project)
            .WithMany()
            .HasForeignKey(s => s.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete sequences when project deleted
        
        // Optional: Navigation to StorySequences (self-referencing for chapter ordering)
        builder.HasOptional(s => s.ParentSequence)
            .WithMany()
            .HasForeignKey(e => e.ParentSequenceId)
            .OnDelete(DeleteBehavior.Restrict);  // Don't cascade delete, maintain historical data
    }
}

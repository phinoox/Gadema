// =============================================================================

// GameDev.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameDev.Core.Models;

namespace GameDev.Core.Configurations.Tasks;

/// <summary>
/// Configuration for ProjectTask entity in game development management system.
/// </summary>
public class ProjectTaskEntityTypeConfiguration : IEntityTypeConfiguration<ProjectTask>
{
    /// <summary>
    /// Configure ProjectTask entity properties and relationships.
    /// Supports ADHD-friendly task filtering with difficulty and quick win indexes.
    /// </summary>
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns (ADHD-friendly filters)
        builder.HasIndex(e => e.ProjectId);
        builder.HasIndex(e => e.Status);          // Filter by task status
        builder.HasIndex(e => e.Difficulty);       // Filter by difficulty level
        builder.HasIndex(e => e.IsQuickWin);       // ADHD-friendly filter for quick wins
        
        // Navigation property: ContentItem (Optional FK)
        builder.HasOne(pt => pt.ContentItem)  // ContentItemId is nullable
            .WithMany(ci => ci.Tasks)
            .HasForeignKey(pt => pt.ContentItemId)
            .OnDelete(DeleteBehavior.Restrict);  // Don't cascade delete, allow task history
        
        // Properties configuration
        builder.Property(e => e.TaskTitle).IsRequired();
    }
}

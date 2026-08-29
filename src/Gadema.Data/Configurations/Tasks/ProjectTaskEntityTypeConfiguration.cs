// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models;

namespace Gadema.Data.Configurations.Tasks;

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
        
        
        // Navigation property: Comments (Collection) - Fixed FK to match ProjectTaskId pattern
        builder.HasMany(pt => pt.Comments)
            .WithOne(pct => pct.ProjectTask)
            .HasForeignKey(pct => pct.ProjectTaskId)
            .OnDelete(DeleteBehavior.Cascade);

        // Properties configuration
        builder.Property(e => e.TaskTitle).IsRequired();
    }
}


// =============================================================================

// GameDev.Core - Shared Domain Models & Interfaces

// =============================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using GameDev.Core.Models;

namespace GameDev.Core.Configurations.Tasks;

/// <summary>
/// Configuration for ProjectTaskComments entity in game development management system.
/// </summary>
public class ProjectTaskCommentsEntityTypeConfiguration : IEntityTypeConfiguration<ProjectTaskComments>
{
    /// <summary>
    /// Configure ProjectTaskComments entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<ProjectTaskComments> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectTaskId);
        builder.HasIndex(e => e.CommentedByUserId);
        builder.HasIndex(e => e.CreatedAt);
        
        // Navigation property: ProjectTask (Cascade delete)
        builder.HasOne(pct => pct.ProjectTask)
            .WithMany(pt => pt.Comments)
            .HasForeignKey(pct => pct.ProjectTaskId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete comments when task deleted
        
        // Properties configuration
        builder.Property(e => e.CommentText).IsRequired().HasMaxLength(4096);
    }
}

// =============================================================================

// Gadema.Core - Shared Domain Models & Interfaces

// =============================================================================

using Gadema.Core.Models.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Tasks;

/// <summary>
/// Configuration for ProjectTaskComments entity in game development management system.
/// </summary>
public class ProjectTaskCommentsEntityTypeConfiguration : IEntityTypeConfiguration<ProjectTaskComment>
{
    /// <summary>
    /// Configure ProjectTaskComments entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<ProjectTaskComment> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.ProjectTaskId).HasDatabaseName("IX_ProjectTaskComment_ProjectTaskId");
        builder.HasIndex(e => e.CommentedByUserId).HasDatabaseName("IX_ProjectTaskComment_CommentedByUserId");
        builder.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_ProjectTaskComment_CreatedAt");
        
        // Navigation property: ProjectTask (Cascade delete)
        builder.HasOne(pct => pct.ProjectTask)
            .WithMany(pt => pt.Comments)
            .HasForeignKey(pct => pct.ProjectTaskId) // Aligned with the model property name
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete comments when task deleted
        
        // Properties configuration
        builder.Property(e => e.CommentText).IsRequired().HasMaxLength(4096);
    }
}
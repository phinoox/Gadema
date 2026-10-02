using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Tasks;

namespace Gadema.Data.Configurations.Tasks;

/// <summary>
/// Configuration for ProjectTaskComment entity in game development management system.
/// </summary>
public class ProjectTaskCommentsEntityTypeConfiguration : IEntityTypeConfiguration<ProjectTaskComment>
{
    /// <summary>
    /// Configure ProjectTaskComment entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<ProjectTaskComment> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);

        // Indexes for performance
        builder.HasIndex(e => e.ProjectTaskId).HasDatabaseName("IX_ProjectTaskComment_ProjectTaskId");
        builder.HasIndex(e => e.CommentedByUserId).HasDatabaseName("IX_ProjectTaskComment_CommentedByUserId");
        builder.HasIndex(e => e.CreatedAt).HasDatabaseName("IX_ProjectTaskComment_CreatedAt");

        // Navigation property: ProjectTask (Cascade delete)
        builder.HasOne(pct => pct.ProjectTask)
            .WithMany(pt => pt.Comments)
            .HasForeignKey(pct => pct.ProjectTaskId)
            .OnDelete(DeleteBehavior.Cascade);

        // Properties configuration
        builder.Property(e => e.CommentText).IsRequired().HasMaxLength(4096);
    }
}

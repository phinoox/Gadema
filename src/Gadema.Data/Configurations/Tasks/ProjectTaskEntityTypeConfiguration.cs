using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Tasks;

namespace Gadema.Data.Configurations.Tasks;

public class ProjectTaskEntityTypeConfiguration : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);

        // Indexes for performance
        builder.HasIndex(e => e.ProjectId).HasDatabaseName("IX_ProjectTask_ProjectId");
        builder.HasIndex(e => e.AssignedToUserId).HasDatabaseName("IX_ProjectTask_AssignedToUserId");
        builder.HasIndex(e => e.DueDate).HasDatabaseName("IX_ProjectTask_DueDate");

        // Relationship: Link to the Project (The parent context)
        builder.HasOne(e => e.Project)
            .WithMany() // You can add ICollection<ProjectTask> to Project later if desired
            .HasForeignKey(e => e.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship: Link to the MetaInfo anchor (The "Soul")
        builder.HasOne(e => e.MetaInfo)
            .WithMany() // One MetaInfo per task component
            .HasForeignKey(e => e.MetaInfoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Tasks;

namespace Gadema.Data.Configurations.Tasks;

/// <summary>
/// Configuration for ProjectTaskMetaInfo entity in game development management system.
/// </summary>
public class ProjectTaskMetaInfoEntityTypeConfiguration : IEntityTypeConfiguration<ProjectTaskMetaInfo>
{
    public void Configure(EntityTypeBuilder<ProjectTaskMetaInfo> builder)
    {
        // Primary key (inherited from BaseMetaInfo)
        builder.HasKey(e => e.Id);

        // Indexes for performance and filtering
        builder.HasIndex(e => e.ProjectId).HasDatabaseName("IX_ProjectTaskMetaInfo_ProjectId");
        builder.HasIndex(e => e.Status).HasDatabaseName("IX_ProjectTaskMetaInfo_Status");
        builder.HasIndex(e => e.Priority).HasDatabaseName("IX_ProjectTaskMetaInfo_Priority");
        builder.HasIndex(e => e.Difficulty).HasDatabaseName("IX_ProjectTaskMetaInfo_Difficulty");

        // Properties configuration
        builder.Property(e => e.Status).IsRequired();
        builder.Property(e => e.Priority).IsRequired();
        builder.Property(e => e.Difficulty).IsRequired();
        builder.Property(e => e.EstimatedMinutes).HasPrecision(18, 2);
        builder.Property(e => e.IsQuickWin).IsRequired();

        // Relationship: Link to the Project (The parent context)
        builder.HasOne(e => e.Project)
            .WithMany()
            .HasForeignKey(e => e.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

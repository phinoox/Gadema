using Gadema.Core.Models.Base.MetaInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gadema.Data.Configurations.Base.MetaInfo;

/// <summary>
/// Configuration for ProjectMetaInfo entity, which acts as the identity anchor for a Project.
/// </summary>
public class ProjectMetaInfoEntityTypeConfiguration : IEntityTypeConfiguration<ProjectMetaInfo>
{
    public void Configure(EntityTypeBuilder<ProjectMetaInfo> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);

        // Identity properties
        builder.Property(e => e.Status).IsRequired();
        builder.Property(e => e.ViewMode).IsRequired();

        // Relationship to Project (1:1)
        // This ensures that the MetaInfo is tied to its parent Project via ProjectId
        builder.HasOne(mi => mi.Project)
            .WithOne(p => p.MetaInfo)
            .HasForeignKey<ProjectMetaInfo>(mi => mi.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance and uniqueness
        builder.HasIndex(e => e.ProjectId).IsUnique();
    }
}

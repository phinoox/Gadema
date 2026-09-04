using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Projects;

namespace Gadema.Data.Configurations.Projects;

public class ProjectTagRelationEntityTypeConfiguration : IEntityTypeConfiguration<ProjectTagRelation>
{
    public void Configure(EntityTypeBuilder<ProjectTagRelation> builder)
    {
        builder.HasKey(e => new { e.ProjectId, e.ProjectTagId });

        builder.HasIndex(e => e.ProjectTagId);

        builder.HasOne(pt => pt.Project)
            .WithMany(p => p.ProjectTags) // Requires adding this collection to Project.cs
            .HasForeignKey(pt => pt.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pt => pt.ProjectTag)
            .WithMany(t => t.ProjectTags)
            .HasForeignKey(pt => pt.ProjectTagId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
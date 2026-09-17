using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Projects;

namespace Gadema.Data.Configurations.Projects;

public class ProjectTagRelationEntityTypeConfiguration : IEntityTypeConfiguration<ProjectTagRelation>
{
    public void Configure(EntityTypeBuilder<ProjectTagRelation> builder)
    {
        // Composite Primary Key: A project can only have a specific tag once
        builder.HasKey(r => new { r.ProjectMetaInfoId, r.TagId });

        // Relationship to Project (The Root Anchor)
        builder.HasOne(r => r.ProjectMetaInfo)
            .WithMany()
            .HasForeignKey(r => r.ProjectMetaInfoId)
            .OnDelete(DeleteBehavior.Cascade); // If project is deleted, its tag relations are gone

        // Relationship to MetaTag
        builder.HasOne(r => r.Tag)
            .WithMany() // Tag doesn't need a collection of relations back to projects
            .HasForeignKey(r => r.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
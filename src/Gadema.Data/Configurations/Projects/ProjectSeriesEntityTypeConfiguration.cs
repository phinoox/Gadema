using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Projects;

namespace Gadema.Data.Configurations.Projects;

public class ProjectSeriesEntityTypeConfiguration : IEntityTypeConfiguration<ProjectSeries>
{
    public void Configure(EntityTypeBuilder<ProjectSeries> builder)
    {
        builder.HasKey(e => e.Id);

        // Note: Title, Slug, and Description are no longer on the ProjectSeries entity itself.
        // They are now managed by ProjectSeriesMetaInfo.

        // Relationships
        builder.HasOne(e => e.MetaInfo)
               .WithOne() 
               .HasForeignKey<ProjectSeriesMetaInfo>(m => m.ProjectSeriesId)
               .OnDelete(DeleteBehavior.Cascade);

        // The inverse relationship (Projects -> ProjectSeries) is configured in ProjectEntityTypeConfiguration
    }
}
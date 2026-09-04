using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Projects;

namespace Gadema.Data.Configurations.Projects;

public class ProjectSeriesEntityTypeConfiguration : IEntityTypeConfiguration<ProjectSeries>
{
    public void Configure(EntityTypeBuilder<ProjectSeries> builder)
    {
        builder.HasKey(e => e.Id);

        // Indexes
        builder.HasIndex(e => e.Slug).IsUnique();

        // Relationships
        // Note: The inverse relationship (Projects -> ProjectSeries) is configured in ProjectEntityTypeConfiguration
    }
}
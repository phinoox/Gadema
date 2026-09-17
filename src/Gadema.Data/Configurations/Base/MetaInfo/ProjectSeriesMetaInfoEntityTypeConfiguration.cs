using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gadema.Core.Models.Projects;

namespace Gadema.Data.Configurations.Base.MetaInfo;

public class ProjectSeriesMetaInfoEntityTypeConfiguration : IEntityTypeConfiguration<ProjectSeriesMetaInfo>
{
    public void Configure(EntityTypeBuilder<ProjectSeriesMetaInfo> builder)
    {
        builder.HasKey(e => e.Id);

        // Indexes (Inherited properties from BaseMetaInfo)
        builder.HasIndex(e => e.Slug).IsUnique();

        // Relationships
        builder.HasOne(e => e.ProjectSeries)
               .WithOne() // One-to-one relationship with ProjectSeries
               .HasForeignKey<ProjectSeriesMetaInfo>(e => e.ProjectSeriesId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}